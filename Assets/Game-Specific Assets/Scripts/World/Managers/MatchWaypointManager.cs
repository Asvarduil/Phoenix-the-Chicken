using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MatchWaypointManager : ManagerBase<MatchWaypointManager>
{
    #region Variables / Properties

    private List<GameObject> _waypoints;
    public List<GameObject> Waypoints
    {
        get
        {
            if (! _waypoints.IsNullOrEmpty())
                return _waypoints;

            GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
            _waypoints = waypoints.ToList();

            return _waypoints;
        }
    }

    #endregion Variables / Properties

    #region Hooks

    public GameObject FindRandomWaypointInRange(Vector3 position, GameObject excludeWaypoint = null, float seekDistance = 10.0f)
    {
        List<GameObject> nearbyWaypoints = new List<GameObject>();
        for(int i = 0; i < Waypoints.Count; i++)
        {
            GameObject waypoint = Waypoints[i];

            if (excludeWaypoint != null
               && waypoint == excludeWaypoint)
                continue;

            if (Vector3.Distance(position, waypoint.transform.position) > seekDistance)
                continue;

            nearbyWaypoints.Add(waypoint);
        }

        int index = Random.Range(0, nearbyWaypoints.Count);
        return nearbyWaypoints[index];
    }

    public GameObject FindNearestWaypoint(Vector3 position, GameObject excludeWaypoint = null, float seekDistance = 100.0f)
    {
        GameObject nearest = null;

        for (int i = 0; i < Waypoints.Count; i++)
        {
            GameObject waypoint = Waypoints[i];

            if (excludeWaypoint != null 
                && waypoint == excludeWaypoint)
                continue;

            float currentDistance = Vector3.Distance(position, waypoint.transform.position);
            if (currentDistance >= seekDistance)
                continue;

            seekDistance = currentDistance;
            nearest = waypoint;
        }

        return nearest;
    }

    #endregion Hooks

    #region Methods

    #endregion Methods
}
