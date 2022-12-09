using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // SINGLETON INSTANCE
    public static NetworkManager instance = null;

    // PRIVATE VARIABLES
    private string GameMode;

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

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // Sync loading the scene later
    }
    #endregion

    #region Photon Methods
    // Login method when not connected yet
    public void Login(string playerName)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Create new room method
    public void CreateRoom(string gameMode)
    {
        if (GameMode != null)
        {
            // Room name automatic
            string roomName = "Room " + Random.Range(1000, 10000);

            // Room options block
            RoomOptions roomOptions = new RoomOptions();
            string[] roomPropertiesInLobby = { "gm", "map" }; // gm = gamemode

            // Set gamemode and default map
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode }, { "map", 0 } };

            // Create room based on options
            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.MaxPlayers = 2; // 1v1
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    // Find room based on game mode
    public void JoinRandomRoom(string gameMode)
    {
        GameMode = gameMode;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", gameMode } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    // Start game method called by master client in lobby panel
    public void StartGame()
    {
        object index;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("map", out index))
        {
            switch((int)index)
            {
                case 0:
                    PhotonNetwork.LoadLevel("Map1");
                    break;
                case 1:
                    PhotonNetwork.LoadLevel("Map2");
                    break;
            }
        }
    }

    // Predetermines which player is P1 and P2
    public void UpdatePlayerOrder()
    {
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if(PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // Update player order based on index in player list
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_ORDER, i } };
                PhotonNetwork.PlayerList[i].SetCustomProperties(playerProperties);
            }
        }
    }
    #endregion

    #region Overrided Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon");
        TitleUIManager.instance.ActivatePanel(Constants.GAMEMODE_PANEL); // Gamemode selection screen
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined room " + PhotonNetwork.CurrentRoom.Name);
        UpdatePlayerOrder(); // Updated whenever a change in players happens in room
        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_SELECTION_NUMBER, 0 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
        TitleUIManager.instance.ActivatePanel(Constants.LOBBY_PANEL); // Lobby screen
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerOrder(); // Updated whenever a change in players happens in room
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerOrder(); // Updated whenever a change in players happens in room
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateRoom(GameMode);
    }

    // Return to Gamemode Selection after leaving room
    public override void OnLeftRoom()
    {
        TitleUIManager.instance.ActivatePanel(Constants.GAMEMODE_PANEL); // Gamemode selection screen
    }
    #endregion
}
