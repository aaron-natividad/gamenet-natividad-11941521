using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class DeathRaceGameManager : GameManager
{
    void Start()
    {
        InstantiatePlayer();
        deathOrder = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void Update()
    {
        if(deathOrder == finishOrder)
        {
            Debug.Log("Game Finish");
        }
    }
}
