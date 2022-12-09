using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPanel : PanelManager
{
    [Header("Panel Items")]
    public List<Image> UIImages;
    public List<TextMeshProUGUI> UITexts;
    public TMP_InputField PlayerInput;

    #region Custom Panel Methods
    // Check if player name is empty or not
    public bool CheckIfPlayerNameValid()
    {
        string playerName = PlayerInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            NetworkManager.instance.Login(playerName);
            return true;
        }
        else
        {
            Debug.Log("Player name invalid");
            return false;
        }
    }
    #endregion

    #region Sequence Methods
    public override void Intro()
    {
        base.Intro();
        Fade(true, 1);
    }

    public override void Outro()
    {
        switch (NextPanelID)
        {
            case Constants.CONNECTING_PANEL:
                SetVisible(false);
                Sequence = 3;
                break;

            default:
                Fade(false, 3);
                break;
        }
    }
    #endregion

    public override void NextSequence(int nextID)
    {
        if(Sequence == 1)
        {
            switch (nextID)
            {
                case Constants.CONNECTING_PANEL:
                    if (CheckIfPlayerNameValid())
                    {
                        NextPanelID = nextID;
                        Sequence++;
                    }
                    break;
                default:
                    NextPanelID = nextID;
                    Sequence++;
                    break;
            }
        }
        
    }

    public override void SetAlpha(float a)
    {
        foreach (Image item in UIImages)
            item.color = new Color(item.color.r, item.color.g, item.color.b, a);
        foreach (TextMeshProUGUI item in UITexts)
            item.color = new Color(item.color.r, item.color.g, item.color.b, a);
    }

    
}
