using System.Collections.Generic;
using SimpleJSON;

public class RevivableMapping : SimpleJsonMapper<RevivableModel>
{
    #region Mappers

    private ModifiableStatMapping _modifiableStatMapper;
    private ModifiableStatMapping ModifiableStatMapper
    {
        get { return _modifiableStatMapper ?? (_modifiableStatMapper = new ModifiableStatMapping()); }
    }

    private MeshDetailMapping _meshDetailMapper;
    private MeshDetailMapping MeshDetailMapper
    {
        get { return _meshDetailMapper ?? (_meshDetailMapper = new MeshDetailMapping()); }
    }

    #endregion Mappers

    #region Methods

    public override List<RevivableModel> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new DataException("RevivableMapping requires a JSON object imported by SimpleJSON!");

        return MapFromJson(rawSource as JSONNode);
    }

    public override object UnMap(RevivableModel sourceObject)
    {
        return ExportState(sourceObject);
    }

    public override JSONClass ExportState(RevivableModel data)
    {
        throw new DataException("RevivableModels are read-only!");
    }

    public override RevivableModel ImportState(JSONClass node)
    {
        RevivableModel result = new RevivableModel
        {
            Name = node["Name"],
            ReviveModelName = node["ReviveModelName"],
            Stats = node["Stats"].AsArray.MapArrayWithMapper(ModifiableStatMapper),
            MeshDetail = MeshDetailMapper.ImportState(node["MeshDetail"].AsObject)
        };

        return result;
    }

    public override List<RevivableModel> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["Revivable"].AsArray;
        List<RevivableModel> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }

    #endregion Methods
}
