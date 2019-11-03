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
    
	private Rigidbody2D rb;
	private int nbJumps;
	private int nbJumpsMax;
	private float runRatio;

	// est en train de se faire tapper (ou d'en sentir les conséquences)
	private bool isPunched;
    private bool isPunching;

    private Animator animator;
    public GameObject model; //c'est le GameObject "Model" du perso
	private int beeingPunchFrame = 0;
    
	private int lives; //nombre de coeurs

    private float lastExitTime; //temps a la derniere sortie de la zone

    public Rect zone; //TODO : lire ces infos sur un objet unique dans la scene

    private void Start()
	{
        GameMaster.AddPlayer(this);
        rb = GetComponent<Rigidbody2D>();
		nbJumps = 2;
		nbJumpsMax = 2;
		runRatio = 2.5f;
        animator = model.GetComponent<Animator>();
        lives = 5;
        ToggleAttackTriggers(false);
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
			if (!isPunching)
                StartCoroutine(Attack());
        }

		// Si on nous attaque
		if (isPunched)
		{
			if(beeingPunchFrame > 15)
			{
				isPunched = false;
				beeingPunchFrame = -1;
			}
			beeingPunchFrame++;
		}

        Vector2 pos = transform.position;
        if (pos.x < zone.xMin || pos.x > zone.xMax || pos.y > zone.yMax || pos.y < zone.yMin)
            KillPlayer();
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

			model.transform.eulerAngles = new Vector3(
					model.transform.eulerAngles.x,
                    Input.GetAxis("Horizontal" + playerID) * -60,
					model.transform.eulerAngles.z
				);
			
			rb.AddForce(new Vector2(dir, 0));
		}
		//freine le joueur si on relache la direction
		if (Mathf.Abs(Input.GetAxis("Horizontal" + playerID)) < 0.4f || isPunched)
		{
			rb.AddForce(new Vector2(-rb.velocity.x * 10, 0));
		}
	}

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.contacts[0].point.y < transform.position.y + 0.1f)
        {
            nbJumps = nbJumpsMax;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
	{
        // Si le collider est un collider d'attaque (le bras de l'adversaire)
        if (coll.CompareTag("Attack"))
            PlayerHit(Mathf.Sign(transform.position.x - coll.transform.parent.position.x));
    }

	// Notre joueur vient de se faire taper
	private void PlayerHit(float xDir)
	{
        if (this.isPunched)
            return;

		SetPunched(true);

		// force du coup ne dépendant pas de la vitesse
		float punchForce = 30f;

		// 10 est un coefficient permettant d'accentuer l'importance de la vélocité dans le calcul de la force du coup
		float playerVel = Mathf.Abs(rb.velocity.x) * 10;

		rb.AddForce(new Vector2(xDir * (playerVel + punchForce), (playerVel + punchForce) * 0.1f),ForceMode2D.Impulse);
	}

    private void KillPlayer()
    {
		if(lastExitTime + 1 < Time.time)
        {
            lastExitTime = Time.time;
            Debug.Log(gameObject.name + " est sorti de la zone");
            transform.position = new Vector3(0, 2, 0);
			rb.velocity = new Vector3(0, 0, 0);
			lives--;
            GameManager.instance.UpdateHearts(playerID, lives);


            if (lives<=0)
            {
                GameMaster.KillPlayer(this);
                Destroy(gameObject);
            }
        }

    }

    //Coroutine a lancer pour l'attaque du joueur
    private IEnumerator Attack()
    {
        isPunching = true;
        animator.SetBool("punching", true);
        yield return new WaitForSeconds(0.1f);
        ToggleAttackTriggers(!isPunched); //active les triggers...
        yield return new WaitForSeconds(0.2f);
        ToggleAttackTriggers(false); //...puis les desactive
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("punching", false);
        isPunching = false;
    }

    //Active ou non les trigger d'attaque de ce joueur
    private void ToggleAttackTriggers(bool actived)
    {
        transform.GetChild(0).gameObject.SetActive(actived);
        transform.GetChild(1).gameObject.SetActive(actived);
    }

	public int GetID()
    {
        return playerID;
    }
    
	public void SetPunched(bool punched)
	{
        StopAllCoroutines();
        animator.SetBool("punching", false);
        isPunching = false;
        ToggleAttackTriggers(false);
        this.isPunched = punched;
	}

    private void OnDrawGizmos()
    {
        //Affiche la zone dans le viewport de la scene
        Gizmos.DrawLine(new Vector3(zone.xMin, zone.yMin, 0), new Vector3(zone.xMax, zone.yMin, 0));
        Gizmos.DrawLine(new Vector3(zone.xMin, zone.yMax, 0), new Vector3(zone.xMax, zone.yMax, 0));
        Gizmos.DrawLine(new Vector3(zone.xMin, zone.yMin, 0), new Vector3(zone.xMin, zone.yMax, 0));
        Gizmos.DrawLine(new Vector3(zone.xMax, zone.yMin, 0), new Vector3(zone.xMax, zone.yMax, 0));
    }
}
