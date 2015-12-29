using System.Collections.Generic;
using SimpleJSON;

public class PlayerMapping : SimpleJsonMapper<PlayerModel>
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

    #endregion Variables / Properties

    #region Methods

    public override List<PlayerModel> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new DataException("PlayerMapping requires a JSON object imported by SimpleJSON!");

        return MapFromJson(rawSource as JSONNode);
    }

    public override object UnMap(PlayerModel sourceObject)
    {
        return ExportState(sourceObject);
    }

    public override JSONClass ExportState(PlayerModel data)
    {
        JSONClass state = new JSONClass();

        state["Name"] = data.Name;
        state["Tag"] = data.Tag;
        state["PlayerState"] = data.PlayerState.ToString();
        state["AscensionLevelForNextPhase"] = new JSONData(data.AscensionLevelForNextPhase);
        state["AscensionEffectPath"] = data.AscensionEffectPath;
        state["NextPlayerModelName"] = data.NextPlayerModelName;
        state["TagsThatCauseDeath"] = data.TagsThatCauseDeath.FoldPrimitiveList();
        state["DeathModelName"] = data.DeathModelName;
        state["Stats"] = data.Stats.FoldList(ModifiableStatMapper);
        state["MeshDetail"] = MeshDetailMapper.ExportState(data.MeshDetail);

        return state;
    }

    public override PlayerModel ImportState(JSONClass node)
    {
        PlayerModel newModel = new PlayerModel();

        newModel.Name = node["Name"];
        newModel.Tag = node["Tag"];
        newModel.PlayerState = node["PlayerState"].ToEnum<PlayerState>();
        newModel.AscensionLevelForNextPhase = node["AscensionLevelForNextPhase"].AsInt;
        newModel.AscensionEffectPath = node["AscensionEffectPath"];
        newModel.NextPlayerModelName = node["NextPlayerModelName"];
        newModel.TagsThatCauseDeath = node["TagsThatCauseDeath"].AsArray.UnfoldStringJsonArray();
        newModel.DeathModelName = node["DeathModelName"];
        newModel.Stats = node["Stats"].AsArray.MapArrayWithMapper(ModifiableStatMapper);
        newModel.MeshDetail = MeshDetailMapper.ImportState(node["MeshDetail"].AsObject);

        return newModel;
    }

    public override List<PlayerModel> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["Player"].AsArray;
        List<PlayerModel> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }

    #endregion Methods
}
