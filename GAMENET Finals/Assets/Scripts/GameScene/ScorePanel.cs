using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class ScorePanel : MonoBehaviourPunCallbacks
{
    public GameManager gm;

    [Header("UI Elements")]
    public TextMeshProUGUI PlayerOneName;
    public TextMeshProUGUI PlayerTwoName;
    public TextMeshProUGUI PlayerOneScore;
    public TextMeshProUGUI PlayerTwoScore;

    #region Event Handling Methods
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        // Update score on all clients
        if (photonEvent.Code == (byte)Constants.UpdateScoreEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int score = (int)data[0];
            int playerOrder = (int)data[1];
            bool isDeathProjectile = (bool)data[2];

            switch ((int)playerOrder)
            {
                case 0:
                    PlayerOneScore.text = score.ToString();
                    break;
                case 1:
                    PlayerTwoScore.text = score.ToString();
                    break;
            }

            if((score >= gm.maxScore || isDeathProjectile) && PhotonNetwork.IsMasterClient)
            {
                gm.Win(PhotonNetwork.PlayerList[playerOrder].NickName, playerOrder);
            }
        }
    }
    #endregion

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        PlayerOneName.text = PhotonNetwork.PlayerList[0].NickName;
        PlayerTwoName.text = PhotonNetwork.PlayerList[1].NickName;
    }

    
}
