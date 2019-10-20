using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void startGame() {
        SceneManager.LoadScene("Arene 1");
    }

    public void doExitGame() {
        Application.Quit();
    }
}
