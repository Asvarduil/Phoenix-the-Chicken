public class GameOverPresenter : UGUIPresenterBase
{
    #region Variables / Properties

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
