using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapModel : INamed
{
    #region Fields

    public string Name;

    public MeshDetail MeshDetail;
    public List<MapObjectModel> MapObjects;

    public string PlayerModel;
    public Vector3 PlayerSpawnPoint;

    public string EntityName
    {
        get { return Name; }
    }

    #endregion Fields
}

