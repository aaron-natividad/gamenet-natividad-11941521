using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class TestSceneLoader : MonoBehaviourPunCallbacks
{
    public void TestLoad()
    {
        PhotonNetwork.LeaveRoom();
        
    }

    public override void OnConnectedToMaster()
    {
        //base.OnLeftRoom();
        SceneManager.LoadScene("TitleScene");
    }
}
