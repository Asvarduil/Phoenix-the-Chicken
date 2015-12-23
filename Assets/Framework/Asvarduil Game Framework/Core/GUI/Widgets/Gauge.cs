using UnityEngine;
using UnityEngine.UI;

public class Gauge : DebuggableBehavior
{
    #region Constants

    private const float _gaugeTheta = 0.01f;

    #endregion Constants

    #region Variables / Properties

    public float TweenRate = 0.1f;
    public int MaxGaugeSize = 100;

    private float _gaugeSize;
    private float _targetGaugeSize;

    private Image _image;
    private RectTransform _rectTransform;

    #endregion Variables / Properties

    #region Hooks

    public void Start()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Update()
    {
        _gaugeSize = Mathf.Lerp(_gaugeSize, _targetGaugeSize, TweenRate);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _gaugeSize);
    }

    public void RecalculateGaugeSize(int current, int max)
    {
        float gaugeSize = ((float)current) / max;
        if (gaugeSize - 1.0f > _gaugeTheta)
            gaugeSize = 1.0f;

        _targetGaugeSize = gaugeSize * MaxGaugeSize;

        DebugMessage("Given a current value of " + current 
                     + ", and a max value of " + max 
                     + ", Gauge " + gameObject.name 
                     + "should fill " + (gaugeSize * 100) + "% of the gauge,"
                     + "and should be " + _targetGaugeSize + "px. wide.");
    }

    public void ShowGauge()
    {
        _image.enabled = true;
    }

    public void HideGauge()
    {
        _image.enabled = false;
    }

    #endregion Hooks

    #region Methods

    #endregion Methods
}
