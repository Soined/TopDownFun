using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Main;

    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        }
        else Main = this;
    }
    #endregion

    private State state;

    [SerializeField]
    private GameObject HUD;
    [SerializeField]
    private GameObject PausePanel;

    public void ChangeUIState(State newState)
    {
        DisableAllPanels();
        switch(newState)
        {
            case State.HUD:
                HUD.SetActive(true);
                break;
            case State.Pause:
                PausePanel.SetActive(true);
                break;
        }
    }

    private void DisableAllPanels()
    {
        HUD.SetActive(false);
        PausePanel.SetActive(false);
    }

    public enum State
    {
        HUD,
        Pause
    }
}
