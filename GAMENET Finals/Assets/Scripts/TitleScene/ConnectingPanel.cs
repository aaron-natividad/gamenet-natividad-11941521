using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectingPanel : PanelManager
{
    [Header("Panel Items")]
    public SpriteRenderer LoadingCircle;

    #region Sequence Methods
    public override void Intro()
    {
        base.Intro();
        Sequence = 1; // Skip intro
    }

    public override void Outro()
    {
        base.Outro();
        Sequence = 3; // Skip Outro
    }
    #endregion

    public override void SetAlpha(float a)
    {
        LoadingCircle.color = new Color(LoadingCircle.color.r, LoadingCircle.color.g, LoadingCircle.color.b, a);
    }
}
