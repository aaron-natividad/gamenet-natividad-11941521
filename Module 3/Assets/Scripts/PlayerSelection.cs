using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviourPunCallbacks
{
    public List<GameObject> RacingGamePlayers;
    public List<GameObject> DeathRacePlayers;

    private List<GameObject> SelectablePlayers;

    public int playerSelectionNumber;

    private void ActivatePlayer(int x)
    {
        // UI selection block
        foreach (GameObject go in SelectablePlayers)
        {
            go.SetActive(false);
        }
        SelectablePlayers[x].SetActive(true);

        // Set the player selection for the vehicle
        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_SELECTION_NUMBER, x } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
    }

    public void goToNextPlayer()
    {
        playerSelectionNumber++;

        if(playerSelectionNumber >= SelectablePlayers.Count)
        {
            playerSelectionNumber = 0;
        }

        ActivatePlayer(playerSelectionNumber);
    }

    public void goToPrevPlayer()
    {
        playerSelectionNumber--;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = SelectablePlayers.Count - 1;
        }

        ActivatePlayer(playerSelectionNumber);
    }

    public void SetGameModePlayers(string gameMode)
    {
        foreach (GameObject go in RacingGamePlayers)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in DeathRacePlayers)
        {
            go.SetActive(false);
        }

        playerSelectionNumber = 0;

        if (gameMode == "rc")
        {
            SelectablePlayers = new List<GameObject>(RacingGamePlayers);
        }
        else if (gameMode == "dr")
        {
            SelectablePlayers = new List<GameObject>(DeathRacePlayers);
        }
        ActivatePlayer(playerSelectionNumber);
    }
}
