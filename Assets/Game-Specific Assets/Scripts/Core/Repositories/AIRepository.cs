
public class AIRepository : RepositoryBase<AIRepository, AIModel>
{
    #region Variables / Properties

    public AIDataAccessor AIDataAccessor;

    #endregion Variables / Properties

    #region Hooks

    public override void Awake()
    {
        _dataAccessor = AIDataAccessor;
        _mapper = new AIMapping();

        base.Awake();
    }

    #endregion Hooks

    #region Methods

    #endregion Methods
}
