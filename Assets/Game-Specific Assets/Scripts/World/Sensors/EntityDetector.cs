using System.Collections.Generic;
using UnityEngine;

public class EntityDetector : DebuggableBehavior
{
    #region Variables / Properties

    public float Radius = 5.0f;
    public Lockout DetectionLockout;
    public List<string> AffectedTags;

    public List<GameObject> SensedEntities;

    #endregion Variables / Properties

    #region Hooks

    public void Update()
    {
        DetectEntities();
    }

    #endregion Hooks

    #region Methods

    public void DetectEntities()
    {
        if (!DetectionLockout.CanAttempt())
            return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        SensedEntities = GetInterestingObjects(hitColliders);

        DetectionLockout.NoteLastOccurrence();
    }

    public List<GameObject> GetInterestingObjects(Collider[] colliders)
    {
        List<GameObject> result = new List<GameObject>();

        for(int i = 0; i < colliders.Length; i++)
        {
            Collider current = colliders[i];
            if (!AffectedTags.Contains(current.tag))
                continue;

            result.Add(current.gameObject);
        }

        return result;
    }

    #endregion Methods


}