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


	private bool punched;

    private Animator animator;
    public GameObject model; //c'est le GameObject "Model" du perso
    private int punchingFrame=0;

	// This est attaqué par ce joueur
	private GameObject attackedPlayer;
	private int nbmort;
    public GameObject textMeshPro;

    private void Start()
	{
        GameMaster.AddPlayer(this);
        rb = GetComponent<Rigidbody2D>();
		nbJumps = 2;
		nbJumpsMax = 2;
		runRatio = 2.5f;
        animator = model.GetComponent<Animator>();
        nbmort = 5;
        TMPro.TextMeshProUGUI textmesh = textMeshPro.GetComponent<TMPro.TextMeshProUGUI>();
        textmesh.SetText("Player " + playerID + " : " + nbmort);
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

        if (animator.GetBool("punching"))
        {

            if (punchingFrame == 30)
            {
                animator.SetBool("punching", false);
                punchingFrame = -1;
                
            }
			if(punchingFrame == 15  && attackedPlayer != null)
			{
				PlayerController otherPlayerController = attackedPlayer.GetComponent<PlayerController>();
				otherPlayerController.setPunched(false);
				attackedPlayer = null;
			}
				
            punchingFrame++;
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
	}

    private void OnCollisionEnter2D(Collision2D coll)
    {
        Component oCall = coll.otherCollider;
        if ((coll.gameObject.tag == "Floor" || coll.gameObject.tag == "Player") && oCall == compt)
        {
            nbJumps = nbJumpsMax;
        }
    }

    private void OnTriggerStay2D(Collider2D coll)
	{
		// Si le collider est un collider d'attaque, donc le bras de l'adversaire
		if (coll.CompareTag("Attack"))
		{
			// On récupère le joueur attaqué
			attackedPlayer = coll.transform.root.gameObject;
		}
	}

	// Attack another player
	private void Attack()
	{
		if (attackedPlayer != null)
		{
			PlayerController otherPlayerController = attackedPlayer.GetComponent<PlayerController>();
			otherPlayerController.setPunched(true);

			// force du coup ne dépendant pas de la vitesse
			float punchForce = 30f;

			// 17 est un coefficient permettant d'accentuer l'importance de la vélocité dans le calcul de la force du coup
			float playerVel = Mathf.Abs(rb.velocity.x) * 17;


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
			otherPlayerController.rb.AddForce(new Vector3(xDirection * (playerVel + punchForce), 0f,0f),ForceMode2D.Impulse);

		}
	}

    private void OnTriggerExit2D(Collider2D coll)
    {
        // Sortie de la zone : elle est représentée par un rectangle comprenant toute la zone.
        // La sortie du trigger correspond à une sortie de la zone.
        if (coll.gameObject.tag == "zone")
        {
            Debug.Log(gameObject.name + " est sorti de la zone");
            transform.position = new Vector3(0, 0, 0);
			rb.velocity = new Vector3(0, 0, 0);
            nbmort = nbmort>0 ? nbmort-1 : nbmort;
            Debug.Log(nbmort);
            TMPro.TextMeshProUGUI textmesh = textMeshPro.GetComponent<TMPro.TextMeshProUGUI>();
            textmesh.SetText("Player " + playerID + " : " + nbmort);

            if (nbmort==0) {
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
}
