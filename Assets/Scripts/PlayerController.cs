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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nbJumps = 2;
        nbJumpsMax = 2;
        runRatio = 2.5f;
    }

    private void Update()
    {
        //on met la gestion du saut dans update car c'est un event buttondown
        //(car peut repasser sur false avant la prochaine fixed update)
        if(Input.GetButtonDown("Jump" + playerID) && nbJumps > 0) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            nbJumps -= 1;
        }
    }

    private void FixedUpdate() {
        //vitesse max
        float ratio = 1f;
        if(Input.GetAxis("Sprint" + playerID) == 1) {
            ratio = runRatio;
        }
        if(Mathf.Abs(rb.velocity.x) < maxHSpeed * ratio) {
            float dir = ratio * Input.GetAxis("Horizontal" + playerID) * acceleration * Time.deltaTime;         

            if(dir > 0.0f) {
                rb.transform.eulerAngles = new Vector3(
                    rb.transform.eulerAngles.x,
                    -70,
                    rb.transform.eulerAngles.z
                );
            }
            else if(dir < 0.0f) {
                rb.transform.eulerAngles = new Vector3(
                    rb.transform.eulerAngles.x,
                    +70,
                    rb.transform.eulerAngles.z
                );
            } else {
                rb.transform.eulerAngles = new Vector3(
                    rb.transform.eulerAngles.x,
                    0,
                    rb.transform.eulerAngles.z
                );
            }

            rb.AddForce(new Vector2(dir, 0));
        }

        //freine le joueur si on relache la direction
        if(Mathf.Abs(Input.GetAxis("Horizontal" + playerID)) < 0.4f) {
            rb.AddForce(new Vector2(-rb.velocity.x * 10, 0));
        }
    }

    private void OnTriggerEnter2D(Collider2D coll) {
        if(coll.CompareTag("Attack")) {
            //affiche dans la console Unity
            Debug.Log("Je suis " + gameObject.name + " et je me suis fait taper par " + coll.transform.root.name);
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        Component oCall = coll.otherCollider;        
        if((coll.gameObject.tag == "Floor" || coll.gameObject.tag == "Player") && oCall == compt) {
            nbJumps = nbJumpsMax;
        }
    }
    private void OnTriggerExit2D(Collider2D sortie)
    {
        if (sortie.gameObject.tag == "zone" ) {
            Debug.Log(gameObject.name + " est sorti de la zone");
            transform.position = new Vector3(0, 0, 0);
        }
    }
}
