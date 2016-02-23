public class RevivableSpawnPointRepository : RepositoryBase<RevivableSpawnPointRepository, SpawnPointModel>
{
    #region Variables / Properties

    public RevivableSpawnPointDataAccessor DataAccessor;

    #endregion Variables / Properties

    #region Hooks

    public override void Awake()
    {
        _dataAccessor = DataAccessor;
        _mapper = new RevivableSpawnPointMapping();

        base.Awake();
    }

    #endregion Hooks

    #region Methods

    public SpawnPointModel GetSpawnPointByName(string modelName)
    {
        SpawnPointModel model = Contents.FindItemByName(modelName);
        if (model == default(SpawnPointModel))
            return null;

        return model;
    }

    #endregion Methods
}

