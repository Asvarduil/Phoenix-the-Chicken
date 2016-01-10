using System.Collections.Generic;
using UnityEngine;

public class SensorBase : DebuggableBehavior
{
    #region Variables / Properties

    public float Radius = 5.0f;
    public Lockout DetectionLockout;
    public List<string> AffectedTags;

    public List<GameObject> SensedEntities = new List<GameObject>();

    private int PriorEntityCount = 0;

    public bool HasSensedNothing
    {
        get { return SensedEntities.Count == 0; }
    }

    public bool HasSensedSomething
    {
        get { return SensedEntities.Count > 0; }
    }

    public bool HasEntered
    {
        get { return SensedEntities.Count > PriorEntityCount; }
    }

    public bool HasLeft
    {
        get { return SensedEntities.Count < PriorEntityCount; }
    }

    #endregion Variables / Properties

    #region Hooks

    public virtual void Update()
    {
        DetectEntities();
    }

    #endregion Hooks

    #region Methods

    public virtual bool HasDetectedEntities()
    {
        DetectEntities();
        return SensedEntities.Count > 0;
    }

    public void DetectEntities()
    {
        if (!DetectionLockout.CanAttempt())
            return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        PriorEntityCount = SensedEntities.Count;
        SensedEntities = GetInterestingObjects(hitColliders, AffectedTags);

        DetectionLockout.NoteLastOccurrence();
    }

    public List<GameObject> GetInterestingObjects(Collider[] colliders, List<string> affectedTags)
    {
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider current = colliders[i];
            if (!affectedTags.Contains(current.tag))
                continue;

            result.Add(current.gameObject);
        }

        return result;
    }

    #endregion Methods
}
