using System;
using System.Collections.Generic;

[Serializable]
public class AIModel : INamed
{
    #region Fields

    public string Name;
    public string Tag;
    public string MoveAnimation;
    public List<RankedTagReaction> Reactions;
    public List<ModifiableStat> Stats;
    public MeshDetail MeshDetail;

    public string EntityName
    {
        get { return Name; }
    }

    #endregion Fields
}
