using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameModeSelectionPanel : PanelManager
{
    [Header("Panel Items")]
    public List<Image> UIImages;
    public List<TextMeshProUGUI> UITexts;
    public CartManager Carts;

    [Header("Buttons")]
    public List<HoverCheck> Highlights;

    [Header("Subpanels")]
    public List<GameObject> Subpanels;

    [Header("Extra Parameters")]
    private int SubpanelIndex;
    public string GameMode;

    #region Custom Panel Methods
    // Check if button is highlighted
    public bool checkIsHighlighted()
    {
        foreach (HoverCheck highlight in Highlights)
        {
            if (highlight.isHighlighted)
            {
                SubpanelIndex = highlight.HoverID; // Activate subpanel based on button hovered
                return true;
            }
        }
        return false;
    }

    // Activate/Deactivate descriptions
    public void DisableSubpanels()
    {
        foreach (GameObject subPanel in Subpanels)
            subPanel.SetActive(false);
    }

    public void ActivateSubpanel()
    {
        DisableSubpanels();
        Subpanels[SubpanelIndex].SetActive(true);
    }
    #endregion

    #region Sequence Methods
    public override void Intro()
    {
        base.Intro();
        TitleUIManager.instance.background.BackgroundIndex = 1;     // Change background
        if (TitleUIManager.instance.background.MoveToLocation())    // When background is finished moving
            Fade(true, 1);
    }

    public override void Middle()
    {
        base.Middle();
        // Change minecart based on highlighted button
        if (checkIsHighlighted())
        {
            ActivateSubpanel();
            Carts.CartIndex = SubpanelIndex;
        }
        else
        {
            DisableSubpanels();
            Carts.CartIndex = 2;
        }
    }

    public override void Outro()
    {
        base.Outro();
        Carts.CartIndex = 2; // Put back minecarts
        Fade(false, 3);
    }
    #endregion

    public override void OnSequenceFinish()
    {
        if(NextPanelID == 0)
            GameMode = "dr"; // DashRock Gamemode
        else
            GameMode = "sd"; // SuddenDeath Gamemode
        NetworkManager.instance.JoinRandomRoom(GameMode);
    }

    public override void ResetPanel(int panelID)
    {
        base.ResetPanel(panelID);
        DisableSubpanels(); // Disable descriptions on panel reset
    }

    public override void SetAlpha(float a)
    {
        foreach (Image item in UIImages)
            item.color = new Color(item.color.r, item.color.g, item.color.b, a);
        foreach (TextMeshProUGUI item in UITexts)
            item.color = new Color(item.color.r, item.color.g, item.color.b, a);
    }
}
