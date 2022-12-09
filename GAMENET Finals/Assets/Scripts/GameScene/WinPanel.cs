using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class WinPanel : MonoBehaviourPunCallbacks
{
    public GameManager gm;

    public Image PanelImage;
    public TextMeshProUGUI PlayerNumberText;
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI WinsText;
    public TextMeshProUGUI ScoreText;
    public Button ReturnButton;
    public TextMeshProUGUI ButtonText;
    public ScorePanel scorePanel;

    [Header("Panel Stats")]
    public int WinnerNumber;
    public string WinnerName;
    public string ScoreString;

    [Header("Timer Stats")]
    public float FadeInTime;

    private float FadeInCD = 0;
    private float Alpha = 0;

    // Sequence Variables
    private int Sequence = 0;
    private bool SequenceFinished = false;
    private bool LeftRoom = false;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        Sequence = 0;
        SequenceFinished = false;
        LeftRoom = false;
        FadeInCD = 0;
        Alpha = 0;
    }

    public void PreIntro()
    {
        ScoreString = scorePanel.PlayerOneScore.text + " - " + scorePanel.PlayerTwoScore.text;
        SetAlpha(0);
        SetEnabled(false);
    }

    // Intro block
    public void Intro()
    {
        this.SequenceFinished = false;

        SetEnabled(true);
        PlayerNumberText.text = "Player " + (WinnerNumber+1);
        PlayerNameText.text = WinnerName;
        ScoreText.text = ScoreString;

        Alpha = FadeInCD / FadeInTime;
        SetAlpha(Alpha);
        FadeInCD += Time.deltaTime;
        if (FadeInCD >= FadeInTime)
        {
            Sequence = 2;
        }
    }

    // Middle block
    public void Middle()
    {
        SetAlpha(1);
        FadeInCD = FadeInTime;
    }

    // Outro block
    public void Outro()
    {
        Alpha = FadeInCD / FadeInTime;
        SetAlpha(Alpha);
        FadeInCD -= Time.deltaTime;
        if (FadeInCD <= 0)
        {
            Sequence = 4;
        }
    }

    public void ChangeSequence(int seq)
    {
        Sequence = seq;
    }

    // Do something after outro
    public virtual void OnSequenceFinish()
    {
        Sequence = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
        PhotonNetwork.Disconnect();
    }

    void Update()
    {
        switch (Sequence)
        {
            case 0:
                PreIntro();
                break;
            case 1:
                Intro();
                break;
            case 2:
                Middle();
                break;
            case 3:
                Outro();
                break;
            case 4:
                if (!SequenceFinished)
                {
                    OnSequenceFinish();
                    SequenceFinished = true;
                }
                break;
        }
    }

    public void SetAlpha(float a)
    {
        PanelImage.color = new Color(PanelImage.color.r, PanelImage.color.g, PanelImage.color.b, a);
        PlayerNameText.color = new Color(PlayerNameText.color.r, PlayerNameText.color.g, PlayerNameText.color.b, a);
        PlayerNumberText.color = new Color(PlayerNumberText.color.r, PlayerNumberText.color.g, PlayerNumberText.color.b, a);
        WinsText.color = new Color(WinsText.color.r, WinsText.color.g, WinsText.color.b, a);
        ScoreText.color = new Color(ScoreText.color.r, ScoreText.color.g, ScoreText.color.b, a);
        ReturnButton.GetComponent<Image>().color = new Color(ReturnButton.GetComponent<Image>().color.r, ReturnButton.GetComponent<Image>().color.g, ReturnButton.GetComponent<Image>().color.b, a);
        ButtonText.color = new Color(ButtonText.color.r, ButtonText.color.g, ButtonText.color.b, a);
    }

    public void SetEnabled(bool enabled)
    {
        PanelImage.enabled = enabled;
        PlayerNameText.enabled = enabled;
        PlayerNumberText.enabled = enabled;
        WinsText.enabled = enabled;
        ScoreText.enabled = enabled;
        ReturnButton.enabled = enabled;
        ButtonText.enabled = enabled;
    }
}
