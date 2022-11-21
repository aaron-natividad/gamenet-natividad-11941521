using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;

    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;
    public List<GameObject> lapTriggers = new List<GameObject>();

    [Header("UI Components")]
    public GameObject[] finisherTextUI;
    public Text timeText;

    [Header("Race Order")]
    public int finishOrder = 0;
    public int deathOrder;

    [Header("Spectator")]
    public List<Camera> playerCameras;

    // Turn into Singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        playerCameras = new List<Camera>();
    }

    protected void InstantiatePlayer()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                Quaternion instantiateRotation = startingPositions[actorNumber - 1].rotation;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, instantiateRotation);
            }
        }

        foreach (GameObject go in finisherTextUI)
        {
            go.SetActive(false);
        }
    }
}
