public class TitleUIPresenter : UGUIPresenterBase
{
    #region Variables / Properties

    private TitleUIController _controller;
    private TitleUIController Controller
    {
        get { return _controller ?? (_controller = TitleUIController.Instance); }
    }

    #endregion Variables / Properties

    #region Hooks

    public void NewGame()
    {
        PlayButtonSound();
        Controller.NewGame();
    }

    public void Quit()
    {
        PlayButtonSound();
        Controller.Quit();
    }

    #endregion Hooks

    #region Methods

    #endregion Methods
}
