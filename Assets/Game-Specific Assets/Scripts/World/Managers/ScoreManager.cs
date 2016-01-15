using System;
using UnityEngine;

[Serializable]
public class Score
{
    #region Variables / Properties

    public DateTime ScoreDate;
    public int ChickensSaved;

    private float StartTime;
    private float EndTime;

    public float SurvivalTime
    {
        get { return EndTime - StartTime; }
    }

    #endregion Variables / Properties

    #region Methods

    public void StartReckoning()
    {
        StartTime = Time.time;
    }

    public void EndReckoning()
    {
        EndTime = Time.time;
    }

    public void ChickenSaved()
    {
        ChickensSaved++;
    }

    #endregion Methods

    #region Operators

    public bool IsBetterThan(Score rhs)
    {
        return SurvivalTime > rhs.SurvivalTime
               && ChickensSaved > rhs.ChickensSaved;
    }

    #endregion Operators
}

public class ScoreManager : ManagerBase<ScoreManager>
{
    #region Variables / Properties

    public Score HighScore;
    public Score CurrentScore;

    public bool HighScoreBeaten
    {
        get { return CurrentScore.IsBetterThan(HighScore); }
    }

    #endregion Variables / Properties

    #region Hooks

    public void Reset()
    {
        CurrentScore = new Score();
    }

    public void NoteChickenRescue()
    {
        CurrentScore.ChickenSaved();
    }

    public void BeginReckoning()
    {
        CurrentScore.StartReckoning();
    }

    public void EndReckoning()
    {
        CurrentScore.EndReckoning();
    }

    #endregion Hooks
}
