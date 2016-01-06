public class RevivableRepository : RepositoryBase<RevivableRepository, RevivableModel>
{
    #region Variables / Properties

    public RevivableDataAccessor DataAccessor;

    #endregion Variables / Properties

    #region Hooks

    public override void Awake()
    {
        _dataAccessor = DataAccessor;
        _mapper = new RevivableMapping();

        base.Awake();
    }

    #endregion Hooks

    #region Methods

    public RevivableModel GetRevivableByName(string modelName)
    {
        RevivableModel result = Contents.FindItemByName(modelName);
        return result;
    }

    #endregion Methods
}
