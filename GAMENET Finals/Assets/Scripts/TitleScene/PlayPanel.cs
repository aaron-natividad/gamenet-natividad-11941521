using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayPanel : PanelManager
{
    [Header("Panel Items")]
    public SpriteRenderer Logo;
    public Image ButtonImage;
    public TextMeshProUGUI ButtonText;

    #region Sequence Methods
    public override void Intro()
    {
        base.Intro();
        TitleUIManager.instance.cover.SetCover(false);
        Fade(true, 1);
    }

    public override void Outro()
    {
        base.Outro();
        Fade(false, 3);
    }
    #endregion

    public override void OnSequenceFinish()
    {
        if (PhotonNetwork.IsConnected)
            TitleUIManager.instance.ActivatePanel(Constants.GAMEMODE_PANEL); // Skip to Gamemode Selection
        else
            TitleUIManager.instance.ActivatePanel(Constants.LOGIN_PANEL); // Go to Login
    }

    public override void SetAlpha(float a)
    {
        Logo.color = new Color(Logo.color.r, Logo.color.g, Logo.color.b, a);
        ButtonImage.color = new Color(ButtonImage.color.r, ButtonImage.color.g, ButtonImage.color.b, a);
        ButtonText.color = new Color(ButtonText.color.r, ButtonText.color.g, ButtonText.color.b, a);
    }
}
