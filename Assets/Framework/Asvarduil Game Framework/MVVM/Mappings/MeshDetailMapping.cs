using System;
using System.Collections.Generic;
using SimpleJSON;

public class MeshDetailMapping : SimpleJsonMapper<MeshDetail>
{
    #region Properties

    private MaterialDetailMapping _materialDetailMapper;
    private MaterialDetailMapping MaterialDetailMapper
    {
        get
        {
            return _materialDetailMapper ?? (_materialDetailMapper = new MaterialDetailMapping());
        }
    }

    #endregion Properties

    #region Methods

    public override JSONClass ExportState(MeshDetail data)
    {
        throw new InvalidOperationException("MeshDetails are read-only.");
    }

    public override MeshDetail ImportState(JSONClass node)
    {
        MeshDetail newModel = new MeshDetail();

        newModel.MeshPath = node["MeshPath"];
        newModel.ObjectScale = node["ObjectScale"].ImportVector3();
        newModel.MeshScale = node["MeshScale"].ImportVector3();
        newModel.MeshOffset = node["MeshOffset"].ImportVector3();
        newModel.MaterialDetails = node["MaterialDetails"].AsArray.MapArrayWithMapper(MaterialDetailMapper);
        newModel.AnimationControllerPath = node["AnimationControllerPath"];

        return newModel;
    }

    public override List<MeshDetail> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new InvalidOperationException("MeshDetailMappings can only be JSONNode objects.");

        return MapFromJson(rawSource as JSONNode);
    }

    public override List<MeshDetail> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["MeshDetails"].AsArray;
        List<MeshDetail> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }

    public override object UnMap(MeshDetail sourceObject)
    {
        throw new InvalidOperationException("MeshDetails are read-only.");
    }

    #endregion Methods
}
