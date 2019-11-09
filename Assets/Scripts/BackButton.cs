using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{

    private void Update()
    {
        if(Input.GetButtonDown("Cancel")) {
            ButtonClick();
        }
    }

    public void ButtonClick() {
        GameMaster.ClearPlayers();
        SceneManager.LoadScene("Main Menu");
    }

}