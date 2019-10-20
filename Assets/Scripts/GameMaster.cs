using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class GameMaster {

    private static int lastPlayer = -1;

    private static List<PlayerController> players = new List<PlayerController>();

    public static void AddPlayer(PlayerController player) {
        players.Add(player);
    }

    public static int GetPlayerID() {
        return lastPlayer;
    }

    public static void KillPlayer(PlayerController deadPlayer) {
        // Si après un kill il y a moins de 2 joueurs en vie, fin du jeu
        // Ca permet de jouer tout seul pour tester jusqu'à se suicider
        if (players.Count==2) {
            players.Remove(deadPlayer);
            lastPlayer = players[0].GetID();
            SceneManager.LoadScene("DeathMenu");
        } else if (players.Count==1) {
            lastPlayer = players[0].GetID();
            players.Remove(deadPlayer);
            SceneManager.LoadScene("DeathMenu");
        }
    }

    public static void ClearPlayers() {
        players.Clear();
    }

}