using System.Collections.Generic;
using SimpleJSON;

public class MapMapping : SimpleJsonMapper<MapModel>
{
    #region Variables / Properties

    private MeshDetailMapping _meshDetailMapping;
    private MeshDetailMapping MeshDetailMapper
    {
        get { return _meshDetailMapping ?? (_meshDetailMapping = new MeshDetailMapping()); }
    }

    private MapObjectMapping _mapObjectMapper;
    private MapObjectMapping MapObjectMapper
    {
        get { return _mapObjectMapper ?? (_mapObjectMapper = new MapObjectMapping()); }
    }

    #endregion Variables / Properties

    #region Methods

    public override List<MapModel> Map(object rawSource)
    {
        if (!(rawSource is JSONNode))
            throw new DataException("PlayerMapping requires a JSON object imported by SimpleJSON!");

        return MapFromJson(rawSource as JSONNode);
    }

    public override object UnMap(MapModel sourceObject)
    {
        return ExportState(sourceObject);
    }

    public override JSONClass ExportState(MapModel data)
    {
        throw new DataException("MapModels are read-only!");
    }

    public override MapModel ImportState(JSONClass node)
    {
        MapModel model = new MapModel
        {
            Name = node["Name"],
            MeshDetail = MeshDetailMapper.ImportState(node["MeshDetail"].AsObject),
            MapObjects = node["MapObjects"].AsArray.MapArrayWithMapper(MapObjectMapper),
            PlayerModel = node["PlayerModel"],
            PlayerSpawnPoint = node["PlayerSpawnPoint"].ImportVector3()
        };

        return model;
    }

    public override List<MapModel> MapFromJson(JSONNode parsed)
    {
        JSONArray jsonModels = parsed["Maps"].AsArray;
        if (jsonModels == null)
            throw new DataException("No data block named 'Maps' was found.");

        List<MapModel> result = jsonModels.MapArrayWithMapper(this);
        return result;
    }

    #endregion Methods
}

