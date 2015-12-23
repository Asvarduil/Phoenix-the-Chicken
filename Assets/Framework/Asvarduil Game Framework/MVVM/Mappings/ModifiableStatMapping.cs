using SimpleJSON;
using System;
using System.Collections.Generic;

public class ModifiableStatMapping : SimpleJsonMapper<ModifiableStat>
{
    public override JSONClass ExportState(ModifiableStat data)
    {
        JSONClass state = new JSONClass();

        state["Name"] = data.Name;
        state["Value"] = new JSONData(data.Value);
        state["ValueCap"] = new JSONData(data.ValueCap);
        state["FixedModifier"] = new JSONData(data.FixedModifier);
        state["ScalingModifier"] = new JSONData(data.ScalingModifier);

        return state;
    }

    public override ModifiableStat ImportState(JSONClass node)
    {
        ModifiableStat result = new ModifiableStat
        {
            Name = node["Name"],
            Value = node["Value"].AsInt,
            ValueCap = node["ValueCap"].AsInt,
            FixedModifier = node["FixedModifier"].AsInt,
            ScalingModifier = node["ScalingModifier"].AsFloat
        };

        return result;
    }

    public override List<ModifiableStat> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new InvalidOperationException("ModifiableStatMapping requires the raw source object to be a JSONNode.");

        return MapFromJson(rawSource as JSONNode);
    }

    public override List<ModifiableStat> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["Stats"].AsArray;
        List<ModifiableStat> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }

    public override object UnMap(ModifiableStat sourceObject)
    {
        return ExportState(sourceObject);
    }
}
