using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelection : MonoBehaviourPunCallbacks
{
    public int PlayerIndex;
    
    public GameObject CharacterChoice;
    public TextMeshProUGUI PlayerNumberText;
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI CharacterText;
    public Image ButtonLeft;
    public Image ButtonRight;
    public Button ReadyButton;
    public TextMeshProUGUI ReadyText;
    public Image PanelImage;

    public int ChoiceCount;
    public bool IsPlayerReady;

    #region Update Methods
    private void Update()
    {
        UpdatePanels();
    }

    public void UpdatePanels()
    {
        object index;
        object isReady;
        if (CheckIfValid())
        {
            // Set Player name
            PlayerNameText.text = PhotonNetwork.PlayerList[PlayerIndex].NickName;

            // Change character choice name
            if (PhotonNetwork.PlayerList[PlayerIndex].CustomProperties.ContainsKey(Constants.PLAYER_SELECTION_NUMBER))
            {
                PhotonNetwork.PlayerList[PlayerIndex].CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out index);
                CharacterChoice.GetComponent<Animator>().SetInteger("CharacterIndex", (int)index);
                switch ((int)index)
                {
                    case 0:
                        CharacterText.text = "Placeholder";
                        break;
                    case 1:
                        CharacterText.text = "Quandale";
                        break;
                    case 2:
                        CharacterText.text = "Gort";
                        break;
                    default:
                        CharacterText.text = "Placeholder";
                        break;
                }
            }
            // Change ready text
            if (PhotonNetwork.PlayerList[PlayerIndex].CustomProperties.ContainsKey(Constants.PLAYER_READY))
            {
                PhotonNetwork.PlayerList[PlayerIndex].CustomProperties.TryGetValue(Constants.PLAYER_READY, out isReady);
                IsPlayerReady = (bool)isReady;
                if (IsPlayerReady)
                    ReadyText.text = "Ready";
                else
                    ReadyText.text = "Not Ready";
            }
        }
    }
    #endregion

    #region Condition Checks
    // Enable/Disable if player exists
    public bool CheckIfValid()
    {
        if (PlayerIndex + 1 <= PhotonNetwork.PlayerList.Length)
        {
            // Set Panel Activated
            PanelImage.color = new Color(1, 1, 1, PanelImage.color.a);
            PlayerNameText.gameObject.SetActive(true);
            CharacterChoice.gameObject.SetActive(true);
            CharacterText.gameObject.SetActive(true);
            CheckIfLocal();
            return true;
        }
        else
        {
            // Set Panel Disabled
            PanelImage.color = new Color(0.5f, 0.5f, 0.5f, PanelImage.color.a);
            PlayerNameText.gameObject.SetActive(false);
            CharacterChoice.gameObject.SetActive(false);
            CharacterText.gameObject.SetActive(false);
            ButtonLeft.gameObject.SetActive(false);
            ButtonRight.gameObject.SetActive(false);
            ReadyButton.gameObject.SetActive(false);
            ReadyText.gameObject.SetActive(false);
            return false;
        }
    }

    // Enable/Disable if panel is for local player
    public bool CheckIfLocal()
    {
        bool isPlayer = (PhotonNetwork.PlayerList[PlayerIndex].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
        ButtonLeft.gameObject.SetActive(isPlayer);
        ButtonRight.gameObject.SetActive(isPlayer);
        ReadyButton.gameObject.SetActive(isPlayer);
        ReadyText.gameObject.SetActive(isPlayer);
        return isPlayer;
    }
    #endregion

    #region Choice Changers
    // Choice changing methods for local client
    public void GoToNextChoice()
    {
        object index;
        int choice = 0;
        if (PhotonNetwork.PlayerList[PlayerIndex].CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out index))
            choice = (int)index;

        choice++;
        if (choice >= ChoiceCount) // Wrap back to 0
            choice = 0;

        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_SELECTION_NUMBER, choice } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
        Debug.Log("Choice: " + choice);
    }

    public void GoToPrevChoice()
    {
        object index;
        int choice = 0;
        if (PhotonNetwork.PlayerList[PlayerIndex].CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out index))
            choice = (int)index;

        choice--;
        if (choice < 0)
            choice = ChoiceCount - 1;

        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_SELECTION_NUMBER, choice } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
        Debug.Log("Choice: " + choice);
    }
    #endregion

    #region Public Methods
    public void SetPlayerReady()
    {
        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_READY, !IsPlayerReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
    }

    public void SetAlpha(float a)
    {
        PlayerNumberText.color = new Color(PlayerNumberText.color.r, PlayerNumberText.color.g, PlayerNumberText.color.b, a);
        PlayerNameText.color = new Color(PlayerNameText.color.r, PlayerNameText.color.g, PlayerNameText.color.b, a);
        CharacterChoice.GetComponent<SpriteRenderer>().color = new Color(CharacterChoice.GetComponent<SpriteRenderer>().color.r, CharacterChoice.GetComponent<SpriteRenderer>().color.g, CharacterChoice.GetComponent<SpriteRenderer>().color.b, a);
        CharacterText.color = new Color(CharacterText.color.r, CharacterText.color.g, CharacterText.color.b, a);
        ButtonLeft.color = new Color(ButtonLeft.color.r, ButtonLeft.color.g, ButtonLeft.color.b, a);
        ButtonRight.color = new Color(ButtonRight.color.r, ButtonRight.color.g, ButtonRight.color.b, a);
        ReadyButton.GetComponent<Image>().color = new Color(ReadyButton.GetComponent<Image>().color.r, ReadyButton.GetComponent<Image>().color.g, ReadyButton.GetComponent<Image>().color.b, a);
        ReadyText.color = new Color(ReadyText.color.r, ReadyText.color.g, ReadyText.color.b, a);
        PanelImage.color = new Color(PanelImage.color.r, PanelImage.color.g, PanelImage.color.b, a);
    }
    #endregion
}
