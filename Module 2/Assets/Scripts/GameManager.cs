using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public static GameManager instance;
    private GameObject localPlayer;

    public int maxScore = 10;

    public List<string> killHistory;
    public int maxEntries = 3;

    public Transform[] RespawnPoints;

    private void Reset()
    {
        GameObject respawnParent = GameObject.Find("RespawnParent");
        RespawnPoints = new Transform[respawnParent.transform.childCount];
        int index = 0;

        while (index < RespawnPoints.Length)
        {
            respawnParent.transform.GetChild(index).name = index.ToString();
            RespawnPoints[index] = respawnParent.transform.GetChild(index).transform;
            index++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        killHistory = new List<string>();

        // Have one instance of GameManager
        if (instance != null) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }

        // Spawn on random point
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomIndex = Random.Range(0, RespawnPoints.Length);
            localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, RespawnPoints[randomIndex].position, Quaternion.identity);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void DeclareWinner(string winnerName)
    {
        localPlayer.GetComponent<Shooting>().GameOver(winnerName);
    }

    public void UpdateKillHistory(string entry)
    {
        killHistory.Add(entry);
        if(killHistory.Count > maxEntries)
        {
            killHistory.RemoveAt(0);
        }
    }
}
