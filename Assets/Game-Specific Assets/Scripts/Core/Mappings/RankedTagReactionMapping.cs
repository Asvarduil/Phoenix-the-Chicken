using System;
using System.Collections.Generic;
using SimpleJSON;

public class RankedTagReactionMapping : SimpleJsonMapper<RankedTagReaction>
{
    public override List<RankedTagReaction> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new DataException("JSON mapping requires a JSON object imported by SimpleJSON!");

        return MapFromJson(rawSource as JSONNode);
    }

    public override object UnMap(RankedTagReaction sourceObject)
    {
        return ExportState(sourceObject);
    }

    public override JSONClass ExportState(RankedTagReaction data)
    {
        throw new InvalidOperationException("RankedTagReactions are read-only!");
    }

    public override RankedTagReaction ImportState(JSONClass node)
    {
        RankedTagReaction result = new RankedTagReaction();

        result.Rank = node["Rank"].AsInt;
        result.Tag = node["Tag"];
        result.Behavior = node["Behavior"].ToEnum<AIBehavior>();

        return result;
    }
    
    public override List<RankedTagReaction> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["Reactions"].AsArray;
        List<RankedTagReaction> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }
}

