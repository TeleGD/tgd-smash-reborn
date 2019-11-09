using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int lives = 5;
    public static GameManager instance;

    private void Start()
    {
        instance = this;
        DisplayPLayersLives();
    }

    private void DisplayPLayersLives()
    {
        int positionOffset = 30;
        for(int p = 1; p <= 2; p++)
        {
            Transform heart = transform.Find("Canvas/P" + p).GetChild(1);
            for(int i = 1; i < lives; i++)
            {
                GameObject newHeart = Instantiate(heart.gameObject, heart.position, Quaternion.identity, heart.parent);
                newHeart.transform.Translate(positionOffset * i, 0, 0);
            }
        }
    }

    public void UpdateHearts(int playerID, int health)
    {
        Destroy(transform.Find("Canvas/P" + playerID).GetChild(health+1).gameObject);
    }
}
