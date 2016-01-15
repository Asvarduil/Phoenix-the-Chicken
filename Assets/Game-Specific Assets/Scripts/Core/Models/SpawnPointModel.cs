using System;

[Serializable]
public class SpawnPointModel : INamed
{
    #region Fields

    public string Name;
    public MeshDetail MeshDetail;

    public string EntityName
    {
        get { return Name; }
    }

    #endregion Fields
}

