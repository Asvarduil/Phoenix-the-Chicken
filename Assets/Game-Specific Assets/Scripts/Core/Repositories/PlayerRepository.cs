public class PlayerRepository : RepositoryBase<PlayerRepository, PlayerModel>
{
    #region Variables / Properties

    public PlayerDataAccessor PlayerDataAccessor;

    #endregion Variables / Properties

    #region Hooks

    public override void Awake()
    {
        _dataAccessor = PlayerDataAccessor;
        _mapper = new PlayerMapping();

        base.Awake();
    }

    #endregion Hooks

    #region Methods

    public PlayerModel GetPlayerModelByName(string playerModelName)
    {
        PlayerModel result = null;

        for(int i = 0; i < Contents.Count; i++)
        {
            PlayerModel current = Contents[i];
            if (current.Name != playerModelName)
                continue;

            result = current;
            break;
        }

        return result;
    }

    #endregion Methods
}
