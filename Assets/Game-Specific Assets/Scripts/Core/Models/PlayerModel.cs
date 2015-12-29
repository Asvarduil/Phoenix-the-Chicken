using System;
using System.Collections.Generic;

[Serializable]
public class PlayerModel
{
    #region Fields

    public string Name;
    public string Tag;
    public PlayerState PlayerState;
    public int AscensionLevelForNextPhase;
    public string AscensionEffectPath;
    public string NextPlayerModelName;
    public List<string> TagsThatCauseDeath;
    public string DeathModelName;
    public List<ModifiableStat> Stats;
    public MeshDetail MeshDetail;

    #endregion Fields
}
