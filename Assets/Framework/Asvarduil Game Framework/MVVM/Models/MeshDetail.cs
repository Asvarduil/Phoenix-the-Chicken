using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshDetail
{
    #region Fields

    public string MeshPath;
    public Vector3 ObjectScale;
    public Vector3 MeshScale;
    public Vector3 MeshOffset;

    public List<MaterialDetail> MaterialDetails;

    public string AnimationControllerPath;

    #endregion Fields
}
