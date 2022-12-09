using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;

    [Header("Prefabs")]
    public List<GameObject> playerPrefabs;
    public GameObject pickupPrefab;
    public GameObject deathPickupPrefab;

    [Header("Player Spawnpoints")]
    public Transform SpawnPointL;
    public Transform SpawnPointR;

    [Header("Rock Spawnpoints")]
    public List<Transform> PickupSpawnAreas;

    [Header("Managed UI Elements")]
    public CameraShake camera;
    public Cover cover;
    public WinPanel winPanel;

    [Header("Global Stats")]
    public int maxScore = 3;

    // GAMEPLAY STATE CHECKERS
    private GameObject localPlayer;     // Stored quick reference to local player
    private bool Initialized = false;

    #region Unity Methods
    // Turn into Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        int index;
        object playerOrder;
        object playerSelectionNumber;
        object gm;
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // Get player custom values
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_ORDER, out playerOrder);
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber);

            // Spawn player on the appropriate side and initialize
            SpawnPlayer((int)playerOrder, (int)playerSelectionNumber);
            localPlayer.GetComponent<PlayerThrow>().OwnerOrder = (int)playerOrder; // Update player order
            localPlayer.GetComponent<PlayerThrow>().OwnerName = PhotonNetwork.LocalPlayer.NickName; // Update owner name
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_READY, false } }; // Reset player ready
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

            // SPAWN ROCKS
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < 4; i++)
                {
                    index = Random.Range(0, PickupSpawnAreas.Count);
                    PhotonNetwork.InstantiateRoomObject(pickupPrefab.name, PickupSpawnAreas[index].position, Quaternion.identity);
                    PickupSpawnAreas.RemoveAt(index);
                }

                PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gm);
                index = Random.Range(0, PickupSpawnAreas.Count);
                if ((string)gm == "dr")
                    PhotonNetwork.InstantiateRoomObject(pickupPrefab.name, PickupSpawnAreas[index].position, Quaternion.identity);
                else if ((string)gm == "sd")
                    PhotonNetwork.InstantiateRoomObject(deathPickupPrefab.name, PickupSpawnAreas[index].position, Quaternion.identity);
                PickupSpawnAreas.RemoveAt(index);
            }
        }
    }

    private void Update()
    {
        if (!Initialized)
        {
            if (CheckAllPlayerReady())
            {
                Initialize();
            }
        }
    }
    #endregion

    #region Event Handling Methods
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        // Uncover stage when ready
        if (photonEvent.Code == (byte)Constants.InitializeEventCode)
            cover.SetCover(false);
        // Add and update to score an all clients
        else if (photonEvent.Code == (byte)Constants.AddScoreEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string ownerName = (string)data[0];
            bool isDeathProjectile = (bool)data[1];

            if (PhotonNetwork.LocalPlayer.NickName == ownerName)
            {
                object playerOrder;
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_ORDER, out playerOrder);
                localPlayer.GetComponent<PlayerMovement>().score++;
                UpdateScore(localPlayer.GetComponent<PlayerMovement>().score, (int)playerOrder, isDeathProjectile);
            }
        }
        // End screen on all clients
        else if (photonEvent.Code == (byte)Constants.WinEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string ownerName = (string)data[0];
            int playerOrder = (int)data[1];

            Debug.Log(ownerName + " wins");
            cover.SetCover(true);
            winPanel.WinnerNumber = playerOrder;
            winPanel.WinnerName = ownerName;
            winPanel.ChangeSequence(1);
        }
    }
    #endregion

    #region Event Senders
    public void Initialize()
    {
        Initialized = true;

        // Default raise event options
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache };
        SendOptions sendOptions = new SendOptions { Reliability = false };

        object[] data = new object[] { 0 }; // Dummy data
        PhotonNetwork.RaiseEvent((byte)Constants.InitializeEventCode, data, raiseEventOptions, sendOptions);
    }

    public void AddScore(string ownerName, bool isDeathProjectile)
    {
        // Default raise event options
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache };
        SendOptions sendOptions = new SendOptions { Reliability = false };

        object[] data = new object[] { ownerName, isDeathProjectile };
        PhotonNetwork.RaiseEvent((byte)Constants.AddScoreEventCode, data, raiseEventOptions, sendOptions);
    }

    public void UpdateScore(int score, int playerOrder, bool isDeathProjectile)
    {
        // Default raise event options
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache };
        SendOptions sendOptions = new SendOptions { Reliability = false };

        object[] data = new object[] { score, playerOrder, isDeathProjectile };
        PhotonNetwork.RaiseEvent((byte)Constants.UpdateScoreEventCode, data, raiseEventOptions, sendOptions);
    }

    public void Win(string ownerName, int playerOrder)
    {
        // Default raise event options
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache };
        SendOptions sendOptions = new SendOptions { Reliability = false };

        object[] data = new object[] { ownerName, playerOrder };
        PhotonNetwork.RaiseEvent((byte)Constants.WinEventCode, data, raiseEventOptions, sendOptions);
    }
    #endregion

    #region Overrided Photon Methods
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Left Room");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        SceneManager.LoadScene("TitleScene");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected To Master");
        PhotonNetwork.LoadLevel("TitleScene");
    }
    #endregion

    #region Other Methods
    private void SpawnPlayer(int playerOrder, int playerSelectionNumber)
    {
        // get spawn position
        Vector3 spawnPosition = new Vector3(0, 0, 0);
        if (playerOrder == 0)
            spawnPosition = SpawnPointL.position;
        else if (playerOrder == 1)
            spawnPosition = SpawnPointR.position;

        localPlayer = PhotonNetwork.Instantiate(playerPrefabs[playerSelectionNumber].name, spawnPosition, Quaternion.identity);
    }

    private bool CheckAllPlayerReady()
    {
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey(Constants.PLAYER_READY))
            {
                object x;
                PhotonNetwork.PlayerList[i].CustomProperties.TryGetValue(Constants.PLAYER_READY, out x);
                if ((bool)x)
                {
                    return false;
                }
            }
        }
        return true;
    }
    #endregion
}
