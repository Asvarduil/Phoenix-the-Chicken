using System.Collections.Generic;
using SimpleJSON;

public class RevivableSpawnPointMapping : SimpleJsonMapper<SpawnPointModel>
{
    #region Variables / Properties

    private MeshDetailMapping _meshDetailMapping;
    private MeshDetailMapping MeshDetailMapper
    {
        get { return _meshDetailMapping ?? (_meshDetailMapping = new MeshDetailMapping()); }
    }

    #endregion Variables / Properties

    #region Methods

    public override List<SpawnPointModel> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new DataException("RevivableSpawnPointMapping requires a JSON object imported by SimpleJSON!");

        return MapFromJson(rawSource as JSONNode);
    }

    public override object UnMap(SpawnPointModel sourceObject)
    {
        return ExportState(sourceObject);
    }

    public override JSONClass ExportState(SpawnPointModel data)
    {
        throw new DataException("RevivableSpawnPoints are read-only!");
    }

    public override SpawnPointModel ImportState(JSONClass node)
    {
        SpawnPointModel newModel = new SpawnPointModel
        {
            Name = node["Name"],
            MeshDetail = MeshDetailMapper.ImportState(node["MeshDetail"].AsObject)
        };

        return newModel;
    }

    public override List<SpawnPointModel> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["RevivableSpawnPoints"].AsArray;
        List<SpawnPointModel> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }

    #endregion Methods
}
