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
        Controller.NewGame();
    }

    public void Quit()
    {
        Controller.Quit();
    }

    #endregion Hooks

    #region Methods

    #endregion Methods
}
