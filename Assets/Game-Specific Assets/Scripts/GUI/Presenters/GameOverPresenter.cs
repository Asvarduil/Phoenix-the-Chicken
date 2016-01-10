using UnityEngine;
using UnityEngine.UI;

public class GameOverPresenter : UGUIPresenterBase
{
    #region Variables / Properties

    public Text SurvivalTime;
    public Text ChickensRevived;
    public Text HighScoreIndicator;

    private GameUIController _controller;
    private GameUIController Controller
    {
        get
        {
            return _controller ?? (_controller = GameUIController.Instance);
        }
    }

    #endregion Variables / Properties

    #region Hooks

    public void PresentScore(float survivalTime, int chickenScore)
    {
        SurvivalTime.text = "Survival Time: " + survivalTime + "sec.";
        ChickensRevived.text = "Chickens Revived: " + chickenScore;

        // TODO: If high score achieved turn on high score indicator.
        HighScoreIndicator.enabled = false;
    }

    public void Retry()
    {
        DebugMessage("Player is retrying...");
        PlayButtonSound();

        Controller.Retry();
    }

    public void TitleScreen()
    {
        DebugMessage("Player is quitting to title screen...");
        PlayButtonSound();

        Controller.TitleScreen();
    }

    #endregion Hooks

    #region Methods

    #endregion Methods
}
