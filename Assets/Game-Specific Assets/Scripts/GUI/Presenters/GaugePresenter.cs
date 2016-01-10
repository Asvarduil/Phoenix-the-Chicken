using UnityEngine.UI;

public class GaugePresenter : DebuggableBehavior
{
    #region Variables / Properties

    public Image GaugeBackground;
    public Gauge Gauge;
    public Image GaugeStartLegend;
    public Image GaugeEndLegend;

    #endregion Variables / Properties

    #region Hooks

    public void ShowGauge()
    {
        FormattedDebugMessage(LogLevel.Info, "Showing Gauge {0}.", gameObject.name);

        GaugeBackground.enabled = true;
        Gauge.ShowGauge();
        GaugeStartLegend.enabled = true;
        GaugeEndLegend.enabled = true;
    }

    public void HideGauge()
    {
        FormattedDebugMessage(LogLevel.Info, "Hiding Gauge {0}.", gameObject.name);

        GaugeBackground.enabled = false;
        Gauge.HideGauge();
        GaugeStartLegend.enabled = false;
        GaugeEndLegend.enabled = false;
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
