using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyPanel : PanelManager
{
    [Header("Panel Items")]
    public TextMeshProUGUI GameModeText;
    public TextMeshProUGUI MapBaseText;
    public TextMeshProUGUI MapText;
    public Button MapButtonUp;
    public Button MapButtonDown;
    public PlayerSelection PlayerPanelLeft;
    public PlayerSelection PlayerPanelRight;
    public Button BackButton;
    public Button StartGameButton;

    [Header("Extra Parameters")]
    public int MapCount;

    #region Custom Panel Methods
    // Map changing methods for master client
    public void GoToNextMap()
    {
        object index;
        int choice;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("map", out index))
        {
            choice = (int)index;
            choice++;
            if (choice >= MapCount) // Wrap back to 0
                choice = 0;

            ExitGames.Client.Photon.Hashtable mapProperty = new ExitGames.Client.Photon.Hashtable() { { "map", choice } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(mapProperty);
        }
    }

    public void GoToPrevMap()
    {
        object index;
        int choice;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("map", out index))
        {
            choice = (int)index;
            choice--;
            if (choice < 0) // Wrap back to last map
                choice = MapCount - 1;

            ExitGames.Client.Photon.Hashtable mapProperty = new ExitGames.Client.Photon.Hashtable() { { "map", choice } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(mapProperty);
        }
    }

    // Enable/Disable master client buttons
    public bool CheckIfMasterClient()
    {
        MapButtonUp.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        MapButtonDown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        // Make start button interactable based on player count and player ready
        StartGameButton.interactable = !(PhotonNetwork.CurrentRoom.PlayerCount < 2) && CheckAllPlayerReady();
        return PhotonNetwork.IsMasterClient;
    }

    // Update gamemode and map name
    public void UpdateLobbyInfo()
    {
        // Change Gamemode Name
        object gameModeName;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
                GameModeText.text = "DashRock";
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("sd"))
                GameModeText.text = "Sudden Death";
        }

        // Change Map Name
        object mapName;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("map", out mapName))
        {
            switch ((int)mapName)
            {
                case 0:
                    MapText.text = "Map 1";
                    break;

                case 1:
                    MapText.text = "Map 2";
                    break;
            }
        }
    }

    // Check if all players are ready before start game
    private bool CheckAllPlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;

            if (p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    #region Sequence Methods
    public override void Intro()
    {
        base.Intro();
        TitleUIManager.instance.background.BackgroundIndex = 2;
        UpdateLobbyInfo();
        CheckIfMasterClient();
        if (TitleUIManager.instance.background.MoveToLocation())
        {
            Fade(true, 1);
        }
    }

    public override void Middle()
    {
        SetVisible(true);
        UpdateLobbyInfo();
        CheckIfMasterClient();
    }

    public override void Outro()
    {
        Fade(false, 3);
    }
    #endregion

    public override void NextSequence(int nextID)
    {
        base.NextSequence(nextID);
        if(nextID == 5)
        {
            TitleUIManager.instance.cover.SetCover(true);

            // Default event options
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache };
            SendOptions sendOptions = new SendOptions { Reliability = false };

            // Start Game
            object[] data = new object[] { 0 }; // dummy data
            PhotonNetwork.RaiseEvent(Constants.StartGameEventCode, data, raiseEventOptions, sendOptions);
        }
    }

    public override void OnSequenceFinish()
    {
        if (NextPanelID == 3)
            PhotonNetwork.LeaveRoom(); // Move back to Gamemode Selection
        else
            NetworkManager.instance.StartGame(); // Start game on all clients
    }

    public override void SetAlpha(float a)
    {
        GameModeText.color = new Color(GameModeText.color.r, GameModeText.color.g, GameModeText.color.b, a);
        MapBaseText.color = new Color(MapBaseText.color.r, MapBaseText.color.g, MapBaseText.color.b, a);
        MapText.color = new Color(MapText.color.r, MapText.color.g, MapText.color.b, a);
        MapButtonUp.GetComponent<Image>().color = new Color(MapButtonUp.GetComponent<Image>().color.r, MapButtonUp.GetComponent<Image>().color.g, MapButtonUp.GetComponent<Image>().color.b, a);
        MapButtonDown.GetComponent<Image>().color = new Color(MapButtonDown.GetComponent<Image>().color.r, MapButtonDown.GetComponent<Image>().color.g, MapButtonDown.GetComponent<Image>().color.b, a);
        BackButton.GetComponent<Image>().color = new Color(BackButton.GetComponent<Image>().color.r, BackButton.GetComponent<Image>().color.g, BackButton.GetComponent<Image>().color.b, a);
        BackButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(BackButton.GetComponentInChildren<TextMeshProUGUI>().color.r, BackButton.GetComponentInChildren<TextMeshProUGUI>().color.g, BackButton.GetComponentInChildren<TextMeshProUGUI>().color.b, a);
        StartGameButton.GetComponent<Image>().color = new Color(StartGameButton.GetComponent<Image>().color.r, StartGameButton.GetComponent<Image>().color.g, StartGameButton.GetComponent<Image>().color.b, a);
        StartGameButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(StartGameButton.GetComponentInChildren<TextMeshProUGUI>().color.r, StartGameButton.GetComponentInChildren<TextMeshProUGUI>().color.g, StartGameButton.GetComponentInChildren<TextMeshProUGUI>().color.b, a);
        PlayerPanelLeft.SetAlpha(a);
        PlayerPanelRight.SetAlpha(a);
    }

    
}
