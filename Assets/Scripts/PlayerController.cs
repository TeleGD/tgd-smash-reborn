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

    private Animator animator;
    public GameObject model; //c'est le GameObject "Model" du perso
    private int punchingFrame=0;

	// Player attaqué par ce joueur
	private GameObject attackedPlayer;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		nbJumps = 2;
		nbJumpsMax = 2;
		runRatio = 2.5f;
        animator = model.GetComponent<Animator>();
        
    }

    private void Update()
	{
		//on met la gestion du saut dans update car c'est un event buttondown
		//(car peut repasser sur false avant la prochaine fixed update)
		if (Input.GetButtonDown("Jump" + playerID))
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
            punchingFrame++;
        }
        print(punchingFrame);
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
		if (Mathf.Abs(Input.GetAxis("Horizontal" + playerID)) < 0.4f)
		{
			rb.AddForce(new Vector2(-rb.velocity.x * 10, 0));
		}
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		Component oCall = coll.otherCollider;
		if (coll.gameObject.tag == "Floor" && oCall == compt)
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

	private void OnTriggerExit2D(Collider2D coll)
	{

		if (coll.CompareTag("Attack"))
		{
			attackedPlayer = null;
		}

		if (coll.gameObject.tag == "zone")
		{
			Debug.Log(gameObject.name + " est sorti de la zone");
			transform.position = new Vector3(0, 0, 0);
		}

	}

	// Attack another player
	private void Attack()
	{
		if (attackedPlayer != null)
		{
			PlayerController otherPlayerController = attackedPlayer.GetComponent<PlayerController>();
            
            

			// On calcule où se situe le joueur par rapport à l'autre
			float playersPositionDiff = gameObject.transform.position.x - otherPlayerController.transform.position.x;
			float xDirection;
			if (playersPositionDiff < 0)
				xDirection = -1;
			else if (playersPositionDiff > 0)
				xDirection = 1;
			else
				xDirection = 0;
			Vector3 movement = new Vector3(-xDirection, 0.0f, 0);

			// Bouge l'autre joueur
			otherPlayerController.rb.AddForce(movement * 6000f);
		}
	}
}
