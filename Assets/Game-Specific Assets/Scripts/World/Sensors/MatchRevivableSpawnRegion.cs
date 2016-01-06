using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MatchRevivableSpawnRegion : SensorBase
{
    #region Variables / Properties

    public List<string> RevivableModels;
    public List<GameObject> SpawnPoints;
    public List<string> OccupyingTags;
    public float EmptyCheckRadius;
    public Lockout SpawnLockout;

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

    public void Start()
    {
        DetectSpawnPoints();
    }

    new public void Update()
    {
        if (!HasDetectedEntities())
            return;

        SpawnRevivable();
    }

    #endregion Hooks

    #region Methods

    private void DetectSpawnPoints()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Revivable Spawn Point");
        SpawnPoints = spawnPoints.ToList();
    }

    public override bool HasDetectedEntities()
    {
        if (!DetectionLockout.CanAttempt())
            return false;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        DetectionLockout.NoteLastOccurrence();

        var sensedEntities = GetInterestingObjects(hitColliders, AffectedTags);
        return sensedEntities.Count > 0;
    }

    public void SpawnRevivable()
    {
        if (!SpawnLockout.CanAttempt())
            return;

        int modelId = Random.Range(0, RevivableModels.Count);
        string modelName = RevivableModels[modelId];

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
                MatchEntityManager.SpawnRevivable(spawnPoint.transform.position, spawnPoint.transform.rotation, modelName);
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