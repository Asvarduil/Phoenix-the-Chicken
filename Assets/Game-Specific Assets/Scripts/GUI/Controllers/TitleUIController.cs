using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIController : ManagerBase<TitleUIController>
{
    #region Constants

    private const string MatchScene = "Match";

    #endregion Constants

    #region Variables / Properties

    private Fader _fader;
    private Fader Fader
    {
        get { return _fader ?? (_fader = FindObjectOfType<Fader>()); }
    }

    private TitleUIPresenter _titleUIPresenter;
    private TitleUIPresenter TitleUIPresenter
    {
        get { return _titleUIPresenter ?? (_titleUIPresenter = FindObjectOfType<TitleUIPresenter>()); }
    }

    #endregion Variables / Properties

    #region Hooks

    public void NewGame()
    {
        // TODO: Choose an arena...
        StartCoroutine(FadeAndLoadScene(MatchScene));
    }

    public void Quit()
    {
        StartCoroutine(FadeAndQuit());
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

    protected IEnumerator FadeAndQuit()
    {
        Fader.FadeOut();

        while (!Fader.ScreenHidden)
        {
            yield return 0;
        }

        Application.Quit();
    }

    #endregion Methods
}
