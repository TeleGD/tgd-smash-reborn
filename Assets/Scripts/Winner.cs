using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Winner : MonoBehaviour
{
    TextMeshProUGUI textMeshPro; 

    void Start() {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if (GameMaster.GetPlayerID()==-1) {
            textMeshPro.text = "TeleGame Design won!";
        } else {
            textMeshPro.text = "Player "+ GameMaster.GetPlayerID() +" won!";
        }
    
    }

}