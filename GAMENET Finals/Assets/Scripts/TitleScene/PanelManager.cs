using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PanelManager : MonoBehaviourPunCallbacks
{
    // Sequence Variables
    protected int Sequence = 0;
    protected bool SequenceFinished = false;

    [Header("Panel Parameters")]
    public int PanelID;
    public int NextPanelID;

    [Header("Timer Stats")]
    public float FadeTime;
    protected float FadeCD = 0;
    protected float Alpha = 0;

    #region Unity Methods
    // Sequence list
    void Update()
    {
        switch (Sequence)
        {
            case 0:
                Intro();
                break;
            case 1:
                Middle();
                break;
            case 2:
                Outro();
                break;
            case 3:
                if (!SequenceFinished)
                {
                    OnSequenceFinish();
                    SequenceFinished = true;
                }
                break;
        }
    }
    #endregion

    #region Sequence Methods
    // Intro block
    public virtual void Intro()
    {
        this.SequenceFinished = false;
    }

    // Middle block
    public virtual void Middle()
    {
        SetVisible(true);
    }

    // Outro block
    public virtual void Outro()
    {

    }
    #endregion

    #region Sequence Changers
    // Change Sequence and Next Panel ID
    public virtual void NextSequence(int nextID)
    {
        if (Sequence == 1)
        {
            NextPanelID = nextID;
            Sequence++;
        }
    }

    // Do something after outro
    public virtual void OnSequenceFinish()
    {
        TitleUIManager.instance.ActivatePanel(NextPanelID);
    }
    #endregion

    #region Public Methods
    // Reset panel sequence and activate/deactivate
    public virtual void ResetPanel(int panelID)
    {
        SetVisible(false);
        if (PanelID == panelID)
        {
            this.Sequence = 0;
        }
        this.gameObject.SetActive(PanelID == panelID);
    }

    // Fade transition
    public bool Fade(bool fadeIn, int nextSequenceIndex)
    {
        Alpha = FadeCD / FadeTime;
        SetAlpha(Alpha);

        if (fadeIn)
        {
            FadeCD += Time.deltaTime;
            if (FadeCD >= FadeTime)
            {
                Sequence = nextSequenceIndex;
                return true;
            }
        }
        else
        {
            FadeCD -= Time.deltaTime;
            if (FadeCD <= 0)
            {
                Sequence = nextSequenceIndex;
                return true;
            }
        }
        return false;
    }

    // Keep constant visible/invisible
    public void SetVisible(bool isVisible)
    {
        Alpha = isVisible ? 1 : 0;
        FadeCD = isVisible ? FadeTime : 0;
        SetAlpha(isVisible ? 1 : 0);
    }

    // Overrideable set alpha
    public virtual void SetAlpha(float a)
    {

    }
    #endregion



    
}
