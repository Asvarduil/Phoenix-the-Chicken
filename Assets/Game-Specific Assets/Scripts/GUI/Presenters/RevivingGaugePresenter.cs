public class RevivingGaugePresenter : UGUIPresenterBase
{
    #region Variables / Properties

    public GaugePresenter RevivingChickenGauge;

    #endregion Variables / Properties

    #region Hooks

    public void UpdateGauge(int current, int max)
    {
        RevivingChickenGauge.RecalculateGaugeSize(current, max);
    }

    #endregion Hooks
}
