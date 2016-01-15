using System;
using System.Collections.Generic;
using SimpleJSON;

public class MapObjectMapping : SimpleJsonMapper<MapObjectModel>
{
    public override List<MapObjectModel> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new DataException("PlayerMapping requires a JSON object imported by SimpleJSON!");

        return MapFromJson(rawSource as JSONNode);
    }

    public override object UnMap(MapObjectModel sourceObject)
    {
        return ExportState(sourceObject);
    }

    public override JSONClass ExportState(MapObjectModel data)
    {
        throw new DataException("Map Object Models are read-only.");
    }

    public override MapObjectModel ImportState(JSONClass node)
    {
        MapObjectModel model = new MapObjectModel
        {
            MapObjectType = node["MapObjectType"].ToEnum<MapObjectType>(),
            ModelName = node["ModelName"],
            Position = node["Position"].ImportVector3(),
            Rotation = node["Rotation"].ImportVector3()
        };

        return model;
    }

    public override List<MapObjectModel> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["Maps"].AsArray;
        List<MapObjectModel> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }
}

