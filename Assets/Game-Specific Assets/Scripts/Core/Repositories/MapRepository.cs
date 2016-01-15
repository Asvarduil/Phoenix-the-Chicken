public class MapRepository : RepositoryBase<MapRepository, MapModel>
{
    #region Variables / Properties

    public MapDataAccessor MapDataAccessor;

    #endregion Variables / Properties

    #region Hooks

    public override void Awake()
    {
        _mapper = new MapMapping();
        _dataAccessor = MapDataAccessor;

        base.Awake();
    }

    #endregion Hooks

    #region Methods

    public MapModel GetMapModelByName(string modelName)
    {
        MapModel model = Contents.FindItemByName(modelName);
        if (model == default(MapModel))
            return null;

        return model;
    }

    #endregion Methods
}

