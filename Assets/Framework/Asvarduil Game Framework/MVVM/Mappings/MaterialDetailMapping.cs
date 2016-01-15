using System;
using System.Collections.Generic;
using SimpleJSON;

public class MaterialDetailMapping : SimpleJsonMapper<MaterialDetail>
{
    public override List<MaterialDetail> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new InvalidOperationException("MeshDetailMappings can only be JSONNode objects.");

        return MapFromJson(rawSource as JSONNode);
    }

    public override object UnMap(MaterialDetail sourceObject)
    {
        return ExportState(sourceObject);
    }

    public override JSONClass ExportState(MaterialDetail data)
    {
        throw new DataException("Material Details are read-only.");
    }

    public override MaterialDetail ImportState(JSONClass node)
    {
        MaterialDetail model = new MaterialDetail
        {
            MaterialPath = node["MaterialPath"],
            TexturePropertyName = node["TexturePropertyName"],
            BumpPropertyName = node["BumpPropertyName"],
            EmissivePropertyName = node["EmissivePropertyName"],
            TexturePath = node["TexturePath"],
            BumpPath = node["BumpPath"],
            EmissivePath = node["EmissivePath"]
        };

        return model;
    }

    public override List<MaterialDetail> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["MaterialDetails"].AsArray;
        List<MaterialDetail> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }
}
