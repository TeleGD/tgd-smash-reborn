using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{

    private void Update()
    {
        //on met la gestion du saut dans update car c'est un event buttondown
        //(car peut repasser sur false avant la prochaine fixed update)
        if(Input.GetButtonDown("Cancel")) {
            ButtonClick();
        }
    }

    public void ButtonClick() {
        GameMaster.ClearPlayers();
        SceneManager.LoadScene("Main Menu");
    }

}