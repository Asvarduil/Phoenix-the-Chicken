using System.Collections;
using UnityEngine.SceneManagement;

public class GameUIController : ManagerBase<GameUIController>
{
    #region Variables

    public string TitleSceneName = "Title";
    public string MatchSceneName = "Match";

    #endregion Variables

    #region Properties

    private Fader _fader;
    private Fader Fader
    {
        get
        {
            return _fader ?? (_fader = FindObjectOfType<Fader>());
        }
    }

    private PhoenixGaugePresenter _phoenixGaugePresenter;
    private PhoenixGaugePresenter PhoenixGaugePresenter
    {
        get
        {
            return _phoenixGaugePresenter ?? (_phoenixGaugePresenter = FindObjectOfType<PhoenixGaugePresenter>());
        }
    }

    private RevivingGaugePresenter _revivingGaugePresenter;
    private RevivingGaugePresenter RevivingGaugePresenter
    {
        get
        {
            return _revivingGaugePresenter ?? (_revivingGaugePresenter = FindObjectOfType<RevivingGaugePresenter>());
        }
    }

    private GameOverPresenter _gameOverPresenter;
    private GameOverPresenter GameOverPresenter
    {
        get
        {
            return _gameOverPresenter ?? (_gameOverPresenter = FindObjectOfType<GameOverPresenter>());
        }
    }

    #endregion Properties

    #region Hooks

    public void ShowPhoenixGauge(bool presentGauge)
    {
        PhoenixGaugePresenter.PresentGUI(presentGauge);
    }

    public void ChangePhoenixGaugeState(PlayerState state, int current, int max)
    {
        PhoenixGaugePresenter.ChangePhoenixGaugeState(state);
        PhoenixGaugePresenter.UpdatePhoenixGauge(current, max);
    }

    public void UpdatePhoenixGauge(int current, int max)
    {
        PhoenixGaugePresenter.UpdatePhoenixGauge(current, max);
    }

    public void ShowRevivingGauge(bool presentGauge)
    {
        RevivingGaugePresenter.PresentGUI(presentGauge);
    }

    public void UpdateRevivingGauge(int current, int max)
    {
        RevivingGaugePresenter.UpdateGauge(current, max);
    }

    public void ShowFailureUI()
    {
        ShowRevivingGauge(false);
        ShowPhoenixGauge(false);

        // TODO: Populate the survival time and numbers of chickens revived.
        GameOverPresenter.PresentGUI(true);
    }

    public void Retry()
    {
        StartCoroutine(FadeAndLoadScene(MatchSceneName));
    }

    public void TitleScreen()
    {
        StartCoroutine(FadeAndLoadScene(TitleSceneName));
    }

    #endregion Hooks

    #region Methods

    protected IEnumerator FadeAndLoadScene(string sceneName)
    {
        //Maestro.FadeOut();
        Fader.FadeOut();

        while (!Fader.ScreenHidden)
        {
            yield return 0;
        }

        SceneManager.LoadScene(sceneName);
    }

    #endregion Methods
}
