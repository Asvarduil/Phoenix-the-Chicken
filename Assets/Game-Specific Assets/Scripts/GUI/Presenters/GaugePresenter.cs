using UnityEngine.UI;

public class GaugePresenter : DebuggableBehavior
{
    #region Variables / Properties

    public Image GaugeBackground;
    public Gauge Gauge;
    public Image GaugeLegend;

    #endregion Variables / Properties

    #region Hooks

    public void ShowGauge()
    {
        FormattedDebugMessage(LogLevel.Info, "Showing Gauge {0}.", gameObject.name);

        GaugeBackground.enabled = true;
        Gauge.ShowGauge();
        GaugeLegend.enabled = true;
    }

    public void HideGauge()
    {
        FormattedDebugMessage(LogLevel.Info, "Hiding Gauge {0}.", gameObject.name);

        GaugeBackground.enabled = false;
        Gauge.HideGauge();
        GaugeLegend.enabled = false;
    }

    public void RecalculateGaugeSize(int current, int max)
    {
        FormattedDebugMessage(LogLevel.Info, "Recalculating Gauge {0} to {1}/{2}", gameObject.name, current, max);
        Gauge.RecalculateGaugeSize(current, max);
    }

    #endregion Hooks

    #region Methods

    #endregion Methods
}
