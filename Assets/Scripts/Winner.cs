using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Winner : MonoBehaviour
{
    TextMeshProUGUI textMeshPro; 

    void Start() {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "Player "+GameMaster.GetPlayerID()+" won!";
    }

}