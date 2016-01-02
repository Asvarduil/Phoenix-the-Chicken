using System.Collections.Generic;

public class RevivableActuator : DebuggableBehavior
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

    public void RealizeModel()
    {
        // TODO: Implement
    }

    #endregion Actualization Methods

    #region Methods

    private void CheckForReviveProximity()
    {
        if (!RevivableSensor.HasDetectedEntities())
        {
            DebugMessage("An object that can revive " + gameObject.name + " is not nearby.");
            GameUIController.ShowRevivingGauge(false);
            return;
        }

        GameUIController.ShowRevivingGauge(true);

        if (!ReviveUpdateLockout.CanAttempt())
            return;

        ReviveStatus.Increase(ReviveRate.Value);
        FormattedDebugMessage(LogLevel.Info, "Reviving {0}: {1}/{2}", gameObject.name, ReviveStatus.Value, ReviveStatus.ValueCap);
        GameUIController.UpdateRevivingGauge(ReviveStatus.Value, ReviveStatus.ValueCap);

        ReviveUpdateLockout.NoteLastOccurrence();

        if(ReviveStatus.IsAtMax)
        {
            FormattedDebugMessage(LogLevel.Info, "Reviving {0} as {1}", gameObject.name, ReviveModelName);
            gameObject.SetActive(false);
            MatchEntityManager.SpawnMob(transform.position, transform.rotation, ReviveModelName);
        }
    }

    #endregion Methods
}
