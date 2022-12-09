using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class TitleUIManager : MonoBehaviourPunCallbacks
{
    // SINGLETON INSTANCE
    public static TitleUIManager instance = null;

    [Header("Scene Transition Cover")]
    public Cover cover;

    [Header("UI Elements")]
    public BackgroundSwitchManager background;

    [Header("Panels")]
    public PlayPanel playPanel;
    public LoginPanel loginPanel;
    public ConnectingPanel connectingPanel;
    public GameModeSelectionPanel gameModeSelectionPanel;
    public LobbyPanel lobbyPanel;

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
        if (photonEvent.Code == Constants.StartGameEventCode)
            TitleUIManager.instance.cover.SetCover(true);
    }
    #endregion

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

    // Activate Play Panel
    private void Start()
    {
        ActivatePanel(Constants.PLAY_PANEL);
    }
    #endregion

    #region Public Methods
    // Activate panel based on Panel ID, reset all other panels
    public void ActivatePanel(int panelNumber)
    {
        playPanel.ResetPanel(panelNumber);
        loginPanel.ResetPanel(panelNumber);
        connectingPanel.ResetPanel(panelNumber);
        gameModeSelectionPanel.ResetPanel(panelNumber);
        lobbyPanel.ResetPanel(panelNumber);
    }
    #endregion
}
