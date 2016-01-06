using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEntityManager : ManagerBase<MatchEntityManager>
{
    #region Variables / Properties

    public string DefaultPlayerModelName;
    public GameObject PlayerTemplate;
    public List<GameObjectPhaseStatePair> CachedPlayerObjects;
    public GameObject MobTemplate;
    public GameObject RevivableTemplate;

    private PlayerRepository _playerRepository;
    private PlayerRepository PlayerRepository
    {
        get { return _playerRepository ?? (_playerRepository = PlayerRepository.Instance); }
    }

    private AIRepository _aiRepository;
    private AIRepository AIRepository
    {
        get { return _aiRepository ?? (_aiRepository = AIRepository.Instance); }
    }

    private RevivableRepository _revivableRepository;
    private RevivableRepository RevivableRepository
    {
        get { return _revivableRepository ?? (_revivableRepository = RevivableRepository.Instance); }
    }

    private RPGCamera _rpgCamera;
    private RPGCamera RPGCamera
    {
        get { return _rpgCamera ?? (_rpgCamera = FindObjectOfType<RPGCamera>()); }
    }

    #endregion Variables / Properties

    #region Hooks

    public void Start()
    {
        StartCoroutine(StartAfterRepositoriesLoaded());
    }

    private IEnumerator StartAfterRepositoriesLoaded()
    {
        while (!PlayerRepository.HasLoaded
               && !AIRepository.HasLoaded)
            yield return 0;

        SwitchToNewPlayerAvatar(Vector3.zero, Quaternion.identity, DefaultPlayerModelName);
    }

    public void InactivatePlayerAvatar(GameObject playerObject, PlayerState state)
    {
        playerObject.SetActive(false);

        GameObjectPhaseStatePair existingObject = FindCachedPlayerObjectByState(state);
        if(existingObject == null)
        {
            AddPlayerObjectToCache(playerObject, state);
        }
    }

    public void SwitchToNewPlayerAvatar(Vector3 position, Quaternion rotation, string playerModelName)
    {
        PlayerModel model = PlayerRepository.GetPlayerModelByName(playerModelName);
        if (model == null)
            throw new ApplicationException("Could not find a model named " + playerModelName + ".");

        PlayerActuator actuator;
        GameObjectPhaseStatePair existingObject = FindCachedPlayerObjectByState(model.PlayerState);
        if(existingObject != null)
        {
            actuator = existingObject.GameObject.GetComponent<PlayerActuator>();
            actuator.ResetActuator(model);

            existingObject.GameObject.SetActive(true);
            return;
        }

        GameObject playerObject = (GameObject) Instantiate(PlayerTemplate, position, rotation);
        actuator = playerObject.GetComponent<PlayerActuator>();
        actuator.RealizeModel(model);

        AddPlayerObjectToCache(playerObject, model.PlayerState);

        // Cause camera to follow the new player object.
        RPGCamera.SetTarget(playerObject);
    }

    public void SpawnMob(Vector3 position, Quaternion rotation, string mobModelName)
    {
        AIModel model = AIRepository.GetAIModelByName(mobModelName);
        if (model == null)
            throw new ApplicationException("Could not find a model named " + mobModelName + ".");

        MobActuator actuator;
        GameObject mobObject = (GameObject)Instantiate(MobTemplate, position, rotation);
        actuator = mobObject.GetComponent<MobActuator>();
        actuator.RealizeModel(model);
    }

    public void InactivateMob(GameObject mob)
    {
        // TODO: Figure out a way to pool mobs.
        //mob.SetActive(false);
        Destroy(mob);
    }

    public void SpawnRevivable(Vector3 position, Quaternion rotation, string revivableModelName)
    {
        RevivableModel model = RevivableRepository.GetRevivableByName(revivableModelName);
        if (model == null)
            throw new ApplicationException("Could not find a model named " + revivableModelName + ".");

        RevivableActuator actuator;
        GameObject revivableObject = (GameObject)Instantiate(RevivableTemplate, position, rotation);
        actuator = revivableObject.GetComponent<RevivableActuator>();
        actuator.RealizeModel(model);
    }

    #endregion Hooks

    #region Player Caching Methods

    private void AddPlayerObjectToCache(GameObject playerGameObject, PlayerState state)
    {
        CachedPlayerObjects.Add(new GameObjectPhaseStatePair
        {
            State = state,
            GameObject = playerGameObject
        });
    }

    private GameObjectPhaseStatePair FindCachedPlayerObjectByState(PlayerState state)
    {
        GameObjectPhaseStatePair result = null;

        for(int i = 0; i < CachedPlayerObjects.Count; i++)
        {
            GameObjectPhaseStatePair current = CachedPlayerObjects[i];
            if (current.State != state)
                continue;

            result = current;
            break;
        }

        return result;
    }

    #endregion Player Caching Methods
}
