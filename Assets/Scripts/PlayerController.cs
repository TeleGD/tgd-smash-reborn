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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //on met la gestion du saut dans update car c'est un event buttondown
        //(car peut repasser sur false avant la prochaine fixed update)
        if(Input.GetButtonDown("Jump" + playerID))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(rb.velocity.x) < maxHSpeed) //vitesse max
        {
            float dir = Input.GetAxis("Horizontal" + playerID) * acceleration * Time.deltaTime;
            rb.AddForce(new Vector2(dir, 0));
        }

        if(Mathf.Abs(Input.GetAxis("Horizontal" + playerID)) < 0.4f) //freine le joueur si on relache la direction
        {
            rb.AddForce(new Vector2(-rb.velocity.x * 10, 0));
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("Attack"))
        {
            //affiche dans la console Unity
            Debug.Log("Je suis " + gameObject.name + " et je me suis fait taper par " + coll.transform.root.name);
        }
    }
}
