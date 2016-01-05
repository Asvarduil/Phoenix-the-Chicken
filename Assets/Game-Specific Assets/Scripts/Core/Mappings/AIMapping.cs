using System;
using System.Collections.Generic;
using SimpleJSON;

public class AIMapping : SimpleJsonMapper<AIModel>
{
    #region Variables / Properties

    private ModifiableStatMapping _modifiableStatMapper;
    private ModifiableStatMapping ModifiableStatMapper
    {
        get
        {
            return _modifiableStatMapper ?? (_modifiableStatMapper = new ModifiableStatMapping());
        }
    }

    private MeshDetailMapping _meshDetailMapper;
    private MeshDetailMapping MeshDetailMapper
    {
        get
        {
            return _meshDetailMapper ?? (_meshDetailMapper = new MeshDetailMapping());
        }
    }

    private RankedTagReactionMapping _rankedTagReactionMapper;
    private RankedTagReactionMapping RankedTagReactionMapper
    {
        get
        {
            return _rankedTagReactionMapper ?? (_rankedTagReactionMapper = new RankedTagReactionMapping());
        }
    }

    #endregion Variables / Properties

    #region Methods

    public override List<AIModel> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new DataException("AIMapping requires a JSON object imported by SimpleJSON!");

        return MapFromJson(rawSource as JSONNode);
    }

    public override object UnMap(AIModel sourceObject)
    {
        return ExportState(sourceObject);
    }

    public override JSONClass ExportState(AIModel data)
    {
        JSONClass state = new JSONClass();

        state["Name"] = data.Name;
        // TODO: More stats.

        return state;
    }

    public override AIModel ImportState(JSONClass node)
    {
        AIModel newModel = new AIModel();

        newModel.Name = node["Name"];
        newModel.Tag = node["Tag"];
        newModel.MoveAnimation = node["MoveAnimation"];
        newModel.Reactions = node["Reactions"].AsArray.MapArrayWithMapper(RankedTagReactionMapper);
        newModel.Stats = node["Stats"].AsArray.MapArrayWithMapper(ModifiableStatMapper);
        newModel.MeshDetail = MeshDetailMapper.ImportState(node["MeshDetail"].AsObject);

        return newModel;
    }

    public override List<AIModel> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["AI"].AsArray;
        List<AIModel> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }

    #endregion Methods
}
