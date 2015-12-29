
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

    public AIModel GetAIModelByName(string mobModelName)
    {
        AIModel result = Contents.FindItemByName(mobModelName);
        if (result == default(AIModel))
            return null;

        return result;
    }

    #endregion Methods
}
