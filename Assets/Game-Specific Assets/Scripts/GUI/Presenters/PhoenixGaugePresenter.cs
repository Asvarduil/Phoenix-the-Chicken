using System;

public class PhoenixGaugePresenter : UGUIPresenterBase
{
    #region Variables / Properties

    public PlayerState PhoenixGaugeState;

    public GaugePresenter AscendingPhoenixGauge;
    public GaugePresenter DescendingPhoenixGauge;
    public GaugePresenter RevivingPhoenixGauge;

    #endregion Variables / Properties

    #region Hooks

    public override void Start()
    {
        base.Start();

        AscendingPhoenixGauge.HideGauge();
        DescendingPhoenixGauge.HideGauge();
        RevivingPhoenixGauge.HideGauge();
    }

    #endregion Hooks

    #region Methods

    public void ChangePhoenixGaugeState(PlayerState newState)
    {
        GaugePresenter oldGauge = GetGaugeByCurrentState();
        oldGauge.HideGauge();

        PhoenixGaugeState = newState;

        GaugePresenter newGauge = GetGaugeByCurrentState();
        newGauge.ShowGauge();
    }

    public void UpdatePhoenixGauge(int phoenixCounter, int maxCounter)
    {
        GaugePresenter gauge = GetGaugeByCurrentState();
        gauge.RecalculateGaugeSize(phoenixCounter, maxCounter);
    }

    private GaugePresenter GetGaugeByCurrentState()
    {
        switch (PhoenixGaugeState)
        {
            case PlayerState.Ascending:
                return AscendingPhoenixGauge;

            case PlayerState.Phoenix:
                return DescendingPhoenixGauge;

            case PlayerState.Reviving:
                return RevivingPhoenixGauge;

            default:
                throw new InvalidOperationException("Unexpected Phoenix Gauge State: " + PhoenixGaugeState);
        }
    }


    #endregion Methods
}
