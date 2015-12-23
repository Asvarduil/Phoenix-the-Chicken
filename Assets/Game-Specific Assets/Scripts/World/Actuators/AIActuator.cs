using System.Collections.Generic;
using UnityEngine;

public enum AIBehavior
{
    Ignore,
    Pursue,
    Avoid
}

public class AIActuator : DebuggableBehavior
{
    #region Variables / Properties

    public List<ModifiableStat> Stats;

    private ModifiableStat AttentionSpan
    {
        get
        {
            return Stats.FindItemByName("AttentionSpan");
        }
    }

    private ModifiableStat AttentionSpanDecay
    {
        get
        {
            return Stats.FindItemByName("AttentionSpanDecay");
        }
    }

    private EntityDetector _detector;
    private EntityDetector Detector
    {
        get
        {
            return _detector ?? (_detector = GetComponentInChildren<EntityDetector>());
        }
    }

    private EntityMotion _motion;
    private EntityMotion Motion
    {
        get
        {
            return _motion ?? (_motion = GetComponent<EntityMotion>());
        }
    }

    #endregion Variables / Properties

    #region Hooks

    public void Update()
    {
        EvaluateAttentionSpan();
    }

    public void RealizeModel()
    {

    }

    #endregion Hooks

    #region Realization Methods



    #endregion Realization Methods

    #region AI Methods

    private void EvaluateAttentionSpan()
    {
        if(AttentionSpan == default(ModifiableStat)
           || AttentionSpanDecay == default(ModifiableStat))
        {
            FormattedDebugMessage(LogLevel.Info, "There is no AttentionSpan or AttentionSpanDecay stat tied to AI {0}", gameObject.name);
            return;
        }

        if (AttentionSpan.Value <= 0.0f)
            Destroy(gameObject);

        AttentionSpan.Value -= AttentionSpanDecay.Value;
        FormattedDebugMessage(LogLevel.Info, "AI entity {0}'s attention level is now {1}", gameObject.name, AttentionSpan.Value);
    }

    #endregion Methods
}
