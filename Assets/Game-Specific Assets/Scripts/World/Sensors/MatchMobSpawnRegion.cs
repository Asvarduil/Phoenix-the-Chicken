using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MatchMobSpawnRegion : SensorBase
{
    #region Variables / Properties

    public List<string> MobModels;
    public List<string> OccupyingTags;
    public float EmptyCheckRadius;
    public Lockout SpawnLockout;

    private List<GameObject> _spawnPoints;
    private List<GameObject> SpawnPoints
    {
        get
        {
            if (!(_spawnPoints.IsNullOrEmpty()))
                return _spawnPoints;

            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
            _spawnPoints = spawnPoints.ToList();

            return _spawnPoints;
        }
    }

    private MatchEntityManager _matchEntityManager;
    private MatchEntityManager MatchEntityManager
    {
        get
        {
            return _matchEntityManager ?? (_matchEntityManager = MatchEntityManager.Instance);
        }
    }

    #endregion Variables / Properties

    #region Hooks

    new public void Update()
    {
        if (!HasDetectedEntities())
            return;

        SpawnMob();
    }

    #endregion Hooks

    #region Methods

    public override bool HasDetectedEntities()
    {
        if (!DetectionLockout.CanAttempt())
            return false;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        DetectionLockout.NoteLastOccurrence();

        var sensedEntities = GetInterestingObjects(hitColliders, AffectedTags);
        return sensedEntities.Count > 0;
    }

    public void SpawnMob()
    {
        if (!SpawnLockout.CanAttempt())
            return;

        int modelId = Random.Range(0, MobModels.Count);
        string modelName = MobModels[modelId];

        int spawnAttempts = 0;
        bool pointSelected = false;
        do
        {
            int pointId = Random.Range(0, SpawnPoints.Count);
            GameObject spawnPoint = SpawnPoints[pointId];

            bool isOccupied = IsSpawnPointOccupied(spawnPoint);
            if (!isOccupied)
            {
                pointSelected = true;
                MatchEntityManager.SpawnMob(spawnPoint.transform.position, spawnPoint.transform.rotation, modelName);
            }
            else
            {
                spawnAttempts++;
            }
        } while (!pointSelected && spawnAttempts < 5);

        SpawnLockout.NoteLastOccurrence();
    }

    private bool IsSpawnPointOccupied(GameObject spawnPoint)
    {
        Collider[] hitColliders = Physics.OverlapSphere(spawnPoint.transform.position, EmptyCheckRadius);
        var sensedEntities = GetInterestingObjects(hitColliders, OccupyingTags);
        return sensedEntities.Count > 0;
    }

    #endregion Methods
}
