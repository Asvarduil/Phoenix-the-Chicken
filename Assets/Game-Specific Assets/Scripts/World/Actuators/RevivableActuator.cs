using System.Collections.Generic;

public class RevivableActuator : BaseActuator<RevivableModel>
{
    #region Variables / Properties

    public string ReviveModelName;
    public Lockout ReviveUpdateLockout;

    public List<ModifiableStat> Stats;
    
    private ModifiableStat ReviveRate
    {
        get { return Stats.FindItemByName("ReviveRate"); }
    }

    private ModifiableStat ReviveStatus
    {
        get { return Stats.FindItemByName("ReviveStatus"); }
    }

    private ScoreManager _scoreManager;
    private ScoreManager ScoreManager
    {
        get
        {
            return _scoreManager ?? (_scoreManager = ScoreManager.Instance);
        }
    }

    private RevivableSensor _revivableSensor;
    private RevivableSensor RevivableSensor
    {
        get
        {
            return _revivableSensor ?? (_revivableSensor = GetComponentInChildren<RevivableSensor>());
        }
    }

    private MatchEntityManager _matchEntityManager;
    private MatchEntityManager MatchEntityManager
    {
        get
        {
            return _matchEntityManager ?? (_matchEntityManager = MatchEntityManager.Instance);
        }
    }

    private GameUIController _gameUIController;
    private GameUIController GameUIController
    {
        get
        {
            return _gameUIController ?? (_gameUIController = GameUIController.Instance);
        }
    }

    #endregion Variables / Properties

    #region Hooks

    public void Update()
    {
        CheckForReviveProximity();
    }

    #endregion Hooks

    #region Actualization Methods

    public override void RealizeModel(RevivableModel model)
    {
        gameObject.name = model.Name;
        gameObject.tag = "Dead Chicken";

        RealizeStats(model);
        RealizeMeshDetails(model.MeshDetail);
    }

    public override void ResetActuator(RevivableModel model)
    {
        RealizeStats(model);
    }

    private void RealizeStats(RevivableModel model)
    {
        Stats = model.Stats.DeepCopyList();

        RevivableSensor.Radius = Stats.FindItemByName("DetectionRange").Value;
        RevivableSensor.DetectionLockout.LockoutRate = Stats.FindItemByName("DetectionRate").Value;
    }

    #endregion Actualization Methods

    #region Methods

    private void CheckForReviveProximity()
    {
        if (!ReviveUpdateLockout.CanAttempt())
            return;

        ReviveUpdateLockout.NoteLastOccurrence();
        RevivableSensor.DetectEntities();

        // If nothing has been sensed, do nothing else.
        // If nothing has been sensed, but something is no longer detected, hide the Revive guage.
        if (RevivableSensor.HasSensedNothing)
        {
            if (RevivableSensor.HasLeft)
            {
                DebugMessage("An object that can revive has left the sensor, and nothing else that can is nearby.");
                GameUIController.ShowRevivingGauge(false);
            }

            return;
        }

        // If something has just been sensed, show the revive guage.
        if (RevivableSensor.HasEntered)
        {
            GameUIController.ShowRevivingGauge(true);
        }

        // If something is being sensed, increment the gauge
        ReviveStatus.Increase(ReviveRate.Value);
        FormattedDebugMessage(LogLevel.Info, "Reviving {0}: {1}/{2}", gameObject.name, ReviveStatus.Value, ReviveStatus.ValueCap);
        GameUIController.UpdateRevivingGauge(ReviveStatus.Value, ReviveStatus.ValueCap);

        // If the revive is complete, hide the gauge, spawn the revived object.
        if(ReviveStatus.IsAtMax)
        {
            FormattedDebugMessage(LogLevel.Info, "Reviving {0} as {1}", gameObject.name, ReviveModelName);

            gameObject.SetActive(false);
            GameUIController.ShowRevivingGauge(false);
            ScoreManager.NoteChickenRescue();

            MatchEntityManager.SpawnMob(transform.position, transform.rotation, ReviveModelName);
        }
    }

    #endregion Methods
}
