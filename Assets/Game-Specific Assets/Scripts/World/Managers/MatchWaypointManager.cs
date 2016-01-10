using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MatchWaypointManager : ManagerBase<MatchWaypointManager>
{
    #region Variables / Properties

    public List<GameObject> Waypoints;

    #endregion Variables / Properties

    #region Hooks

    public void Start()
    {
        FindWaypoints();
    }

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

    private void FindWaypoints()
    {
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        Waypoints = waypoints.ToList();
    }

    #endregion Methods
}
