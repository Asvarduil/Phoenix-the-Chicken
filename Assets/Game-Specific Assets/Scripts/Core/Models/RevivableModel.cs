using System;
using System.Collections.Generic;

[Serializable]
public class RevivableModel : INamed
{
    #region Fields

    public string Name;
    public string ReviveModelName;
    public List<ModifiableStat> Stats;
    public MeshDetail MeshDetail;

    public string EntityName
    {
        get { return Name; }
    }

    #endregion Fields
}
