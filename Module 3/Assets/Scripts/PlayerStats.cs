using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    public GameObject playerHealthbar;

    [Header("Healthbar")]
    public Image healthBar;
    public float maxHealth;
    public float health;

    [Header("PlayerName")]
    public TextMeshProUGUI playerNameUI;

    [Header("Spectator Stuff")]
    public bool isSpectator;
    public int spectatorIndex = 0;

    public enum RaiseEventsCode
    {
        DeathEventCode = 1
    }

    void Start()
    {
        playerHealthbar.SetActive(!photonView.IsMine);
        playerNameUI.text = photonView.Owner.NickName;
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        GameManager.instance.playerCameras.Add(this.gameObject.transform.Find("Camera").GetComponent<Camera>());
    }

    void Update()
    {
        if (isSpectator && photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                spectatorIndex++;
                if (spectatorIndex >= GameManager.instance.playerCameras.Count)
                    spectatorIndex = 0;
                UpdateCamera(spectatorIndex);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                spectatorIndex--;
                if (spectatorIndex < 0)
                    spectatorIndex = GameManager.instance.playerCameras.Count - 1;
                UpdateCamera(spectatorIndex);
            }
        }
    }

    #region Health Methods
    [PunRPC]
    public void TakeDamage(string shooter, int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / maxHealth;

        if (health <= 0)
        {
            Die(shooter, info.Sender.ActorNumber);
        }
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }

    public void Die(string shooter, int actorNumber)
    {
        photonView.RPC("ActivatePlayer", RpcTarget.AllBuffered, false);
        
        string nickName = photonView.Owner.NickName;

        // event data
        object[] data = new object[] { nickName, GameManager.instance.deathOrder, actorNumber, shooter};

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };
        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.DeathEventCode, data, raiseEventOptions, sendOptions);
        GameManager.instance.deathOrder--;
    }
    #endregion

    #region Event Methods
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.DeathEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string deadPlayer = (string)data[0];
            int order = (int)data[1];
            int actorNumber = (int)data[2];
            string shooter = (string)data[3];

            Debug.Log(deadPlayer + " " + order);

            GameObject orderUIText = GameManager.instance.finisherTextUI[order - 1];
            orderUIText.SetActive(true);

            if (actorNumber == photonView.OwnerActorNr)
            {
                orderUIText.GetComponent<Text>().text = order + " " + shooter + " killed " + deadPlayer + " (YOU)";
                orderUIText.GetComponent<Text>().color = Color.red;
            }
            else
            {
                orderUIText.GetComponent<Text>().text = order + " " + shooter + " killed " + deadPlayer;
                orderUIText.GetComponent<Text>().color = Color.white;
            }
        }
    }
    #endregion

    #region Spectator Methods
    public void UpdateCamera(int index)
    {
        for (int i = 0; i < GameManager.instance.playerCameras.Count; i++)
        {
            if (i == index)
            {
                GameManager.instance.playerCameras[i].enabled = true;
            }
            else
            {
                GameManager.instance.playerCameras[i].enabled = false;
            }
        }
    }
    #endregion
    [PunRPC]
    public void ActivatePlayer(bool activated)
    {
        this.gameObject.transform.Find("Mesh").gameObject.SetActive(activated);
        this.gameObject.transform.Find("PlayerHealthbar").gameObject.SetActive(activated);
        GetComponent<BoxCollider>().enabled = activated;
        GetComponent<VehicleMovement>().enabled = activated;
        GetComponent<Weapon>().enabled = activated;
        isSpectator = !activated;
    }
}
