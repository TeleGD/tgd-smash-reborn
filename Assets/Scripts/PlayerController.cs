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

	// Player attaqué par ce joueur
	private GameObject attackedPlayer;

	private Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		//on met la gestion du saut dans update car c'est un event buttondown
		//(car peut repasser sur false avant la prochaine fixed update)
		if (Input.GetButtonDown("Jump" + playerID))
		{
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
		}

		// Si le bouton d'attaque est pressé
		if (Input.GetButtonDown("Attack"+ playerID))
		{
			Attack();
		}
	}

	private void FixedUpdate()
	{
		//vitesse max
		if (Mathf.Abs(rb.velocity.x) < maxHSpeed)
		{
			float dir = Input.GetAxis("Horizontal" + playerID) * acceleration * Time.deltaTime;

			if (dir > 0.0f)
			{
				rb.transform.eulerAngles = new Vector3(
					rb.transform.eulerAngles.x,
					-70,
					rb.transform.eulerAngles.z
				);
			}
			else if (dir < 0.0f)
			{
				rb.transform.eulerAngles = new Vector3(
					rb.transform.eulerAngles.x,
					+70,
					rb.transform.eulerAngles.z
				);
			}
			else
			{
				rb.transform.eulerAngles = new Vector3(
					rb.transform.eulerAngles.x,
					0,
					rb.transform.eulerAngles.z
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

	}

	// Attack another player
	private void Attack()
	{
		if (attackedPlayer != null)
		{
			PlayerController otherPlayerController = attackedPlayer.GetComponent<PlayerController>();

			// On calcule où se situe le joueur par rapport à l'autre
			float playersPositionDiff = gameObject.transform.position.x - otherPlayerController.transform.position.x;
			Vector3 movement = new Vector3(-playersPositionDiff, 0.0f, 0);

			// Bouge l'autre joueur
			otherPlayerController.rb.AddForce(movement * 2000f);
		}
	}
}
