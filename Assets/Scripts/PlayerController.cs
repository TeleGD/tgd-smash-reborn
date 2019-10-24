using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

	//les inputs dans l'InputManager sont la concaténation du nom du bouton et de l'ID du joueur
	public byte playerID = 1;

	public float acceleration = 1500;
	public float maxHSpeed = 5; //vitesse horizontale max en m/s
	public float jumpForce = 15; //vitesse d'impulsion du saut en m/s

	public Component compt;
	private Rigidbody2D rb;
	private int nbJumps;
	private int nbJumpsMax;
	private float runRatio;

	// est en train de se faire tapper (ou d'en sentir les conséquences)
	private bool punched;

    private Animator animator;
    public GameObject model; //c'est le GameObject "Model" du perso
    private int punchingFrame=0;
	private int beeingPunchFrame = 0;

	// Joueur a porté d'attaque
	private GameObject closePlayer;
	// Le dernier attaquant dont on subit encore l'attaque
	private GameObject attacker;
	private int lives;
    public GameObject textMeshPro;
	public GameObject heartPrefab;

    private void Start()
	{
        GameMaster.AddPlayer(this);
        rb = GetComponent<Rigidbody2D>();
		nbJumps = 2;
		nbJumpsMax = 2;
		runRatio = 2.5f;
        animator = model.GetComponent<Animator>();
        lives = 5;

		TMPro.TextMeshProUGUI textmesh = textMeshPro.GetComponent<TMPro.TextMeshProUGUI>();
		textmesh.SetText("Player " + playerID + " : ");

		// affichage des vies
		float gap = 15f;

		// Pour le joueur 2 on met les coeurs de droite à gauche et inversement pour le joueur 1
		float sens = Mathf.Pow(-1, (int)playerID);

		float nextXPosition = textmesh.rectTransform.anchoredPosition.x + sens * (textmesh.rectTransform.rect.width / 2 - gap);
		float yPosition = textmesh.rectTransform.anchoredPosition.y - (textmesh.rectTransform.rect.height);

		RectTransform rt = (RectTransform)heartPrefab.transform;

		for (int i = 0; i < lives; i++)
		{
			GameObject heart = Instantiate(heartPrefab, new Vector3(nextXPosition, yPosition, 0), Quaternion.identity) as GameObject;
			heart.transform.SetParent(GameObject.FindGameObjectWithTag("canvas").transform, false);
			heart.tag = "heartsPlayer" + playerID;
			nextXPosition = nextXPosition - sens * (rt.rect.width - gap);
		}


	}

    private void Update()
	{
		//on met la gestion du saut dans update car c'est un event buttondown
		//(car peut repasser sur false avant la prochaine fixed update)
		if (Input.GetButtonDown("Jump" + playerID) && nbJumps > 0)
		{
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			nbJumps -= 1;
		}

		// Si le bouton d'attaque est pressé
		if (Input.GetButtonDown("Attack"+ playerID))
		{
			if (!animator.GetBool("punching"))
            {
                animator.SetBool("punching", true);
                Attack();
                
            }
        }

		// Gestion de l'animation de notre attaque
        if (animator.GetBool("punching"))
        {

            if (punchingFrame == 30)
            {
                animator.SetBool("punching", false);
                punchingFrame = -1;

			}
				
            punchingFrame++;
        }

		// Si on nous attaque
		if (punched)
		{
			if(beeingPunchFrame > 15)
			{
				attacker = null;
				punched = false;
				beeingPunchFrame = -1;
			}
			beeingPunchFrame++;
		}
	}

	private void FixedUpdate()
	{
		//vitesse max
		float ratio = 1f;
		if (Input.GetAxis("Sprint" + playerID) == 1)
		{
			ratio = runRatio;
		}

		if (Mathf.Abs(rb.velocity.x) < maxHSpeed * ratio)
		{
			float dir = ratio * Input.GetAxis("Horizontal" + playerID) * acceleration * Time.deltaTime;

			if (dir > 0.0f)
			{
				model.transform.eulerAngles = new Vector3(
					model.transform.eulerAngles.x,
					-70,
					model.transform.eulerAngles.z
				);
			}
			else if (dir < 0.0f)
			{
				model.transform.eulerAngles = new Vector3(
					model.transform.eulerAngles.x,
					+70,
					model.transform.eulerAngles.z
				);
			}
			else
			{
				model.transform.eulerAngles = new Vector3(
					model.transform.eulerAngles.x,
					0,
					model.transform.eulerAngles.z
				);
			}

			rb.AddForce(new Vector2(dir, 0));
		}
		//freine le joueur si on relache la direction
		if ((Mathf.Abs(Input.GetAxis("Horizontal" + playerID)) < 0.4f || punched ))
		{
			rb.AddForce(new Vector2(-rb.velocity.x * 10, 0));
		}
		if ((punched))
		{
			rb.AddForce(new Vector2(0, -rb.velocity.y * 70));
		}
	}

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.contacts[0].point.y < transform.position.y + 0.1f)
        {
            nbJumps = nbJumpsMax;
        }
    }

    private void OnTriggerStay2D(Collider2D coll)
	{
		// Si le collider est un collider d'attaque (le bras de l'adversaire)
		if (coll.CompareTag("Attack"))
		{
			// On récupère le joueur proche
			closePlayer = coll.transform.root.gameObject;
		}
	}

	// Attack another player
	private void Attack()
	{
		if (closePlayer != null)
		{
			PlayerController otherPlayerController = closePlayer.GetComponent<PlayerController>();
			otherPlayerController.setPunched(true);

			// force du coup ne dépendant pas de la vitesse
			float punchForce = 25f;

			// 13 est un coefficient permettant d'accentuer l'importance de la vélocité dans le calcul de la force du coup
			float playerVel = Mathf.Abs(rb.velocity.x) * 13;


			// On calcule où se situe le joueur par rapport à l'autre
			float playersPositionDiff = transform.position.x - otherPlayerController.transform.position.x;


			float xDirection;
			// l'autre joueur est à droite du joueur
			if (playersPositionDiff > 0)
			{
				xDirection = -1;
			}

			else if (playersPositionDiff < 0)
				xDirection = 1;
			else
				xDirection = 0;

			//Vector3 movement = new Vector3(xDirection, 0.0f, 0);
			otherPlayerController.rb.AddForce(new Vector3(xDirection * (playerVel + punchForce), (playerVel + punchForce), 0f),ForceMode2D.Impulse);

		}
	}

    private void OnTriggerExit2D(Collider2D coll)
    {
		if (coll.CompareTag("Attack"))
		{
			closePlayer = null;
		}
		// Sortie de la zone : elle est représentée par un rectangle comprenant toute la zone.
		// La sortie du trigger correspond à une sortie de la zone.
		if (coll.gameObject.tag == "zone")
        {
            Debug.Log(gameObject.name + " est sorti de la zone");
            transform.position = new Vector3(0, 0, 0);
			rb.velocity = new Vector3(0, 0, 0);
			lives = lives>0 ? lives-1 : lives;
			DisplayPLayersLives();


			if (lives==0) {
                GameMaster.KillPlayer(this);
                Destroy(gameObject);
            }
        }

    }

	public int GetID() {
        return playerID;
    }

	public void setPunched(bool punched)
	{
		this.punched = punched;
	}

	private void DisplayPLayersLives()
	{

		TMPro.TextMeshProUGUI textmesh = textMeshPro.GetComponent<TMPro.TextMeshProUGUI>();

		// Destroy all instances of heartPrefab
		var hearts = GameObject.FindGameObjectsWithTag("heartsPlayer"+playerID);

		foreach (GameObject heart in hearts)
		{
			Destroy(heart);
		}
		// affichage des vies
		float gap = 15f;

		// Pour le joueur 2 on met les coeurs de droite à gauche et inversement pour le joueur 1
		float sens = Mathf.Pow(-1, (int)playerID);

		float nextXPosition = textmesh.rectTransform.anchoredPosition.x + sens * (textmesh.rectTransform.rect.width / 2 - gap);
		float yPosition = textmesh.rectTransform.anchoredPosition.y - (textmesh.rectTransform.rect.height);

		RectTransform rt = (RectTransform)heartPrefab.transform;

		for (int i = 0; i < lives; i++)
		{
			GameObject heart = Instantiate(heartPrefab, new Vector3(nextXPosition, yPosition, 0), Quaternion.identity) as GameObject;
			heart.transform.SetParent(GameObject.FindGameObjectWithTag("canvas").transform, false);
			heart.tag = "heartsPlayer" + playerID;
			nextXPosition = nextXPosition - sens * (rt.rect.width - gap);
		}
	}
}
