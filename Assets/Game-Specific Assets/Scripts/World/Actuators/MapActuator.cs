using System;
using System.Collections.Generic;
using UnityEngine;

public class MapActuator : BaseActuator<MapModel>
{
    #region Variables / Properties

    public GameObject SpawnPointPrototype;
    public GameObject WaypointPrototype;

    private MapRepository _mapRepository;
    private MapRepository MapRepository
    {
        get { return _mapRepository ?? (_mapRepository = MapRepository.Instance); }
    }

    private SpawnPointRepository _spawnPointRepository;
    private SpawnPointRepository SpawnPointRepository
    {
        get { return _spawnPointRepository ?? (_spawnPointRepository = SpawnPointRepository.Instance); }
    }

    #endregion Variables / Properties

    #region Hooks

    public void Start()
    {
        // TODO: Default code
        MapModel model = MapRepository.GetMapModelByName("Default");
        RealizeModel(model);
    }

    #endregion Hooks

    #region Realization Methods

    public override void RealizeModel(MapModel model)
    {
        RealizeMeshDetails(model.MeshDetail);
        RealizeMapObjects(model.MapObjects);
    }

    public override void ResetActuator(MapModel model)
    {
        throw new NotImplementedException("Maps do not need to be reset.");
    }

    private void RealizeMapObjects(List<MapObjectModel> mapObjects)
    {
        for(int i = 0; i < mapObjects.Count; i++)
        {
            MapObjectModel current = mapObjects[i];
            RealizeMapObject(current);
        }
    }

    private void RealizeMapObject(MapObjectModel model)
    {
        GameObject result;
        switch (model.MapObjectType)
        {
            case MapObjectType.SpawnPoint:
                result = (GameObject)Instantiate(SpawnPointPrototype, model.Position, Quaternion.Euler(model.Rotation));
                SpawnPointActuator actuator = result.GetComponent<SpawnPointActuator>();
                if (actuator == null)
                    throw new ApplicationException("Spawn Point Prototype does not have a Spawn Point Actuator behavior on it!");

                SpawnPointModel spawnPointModel = SpawnPointRepository.GetSpawnPointByName(model.ModelName);
                if (spawnPointModel == null)
                    throw new DataException("Spawn Point Model " + model.ModelName + " isn't defined in the SpawnPoint data file.");

                actuator.RealizeModel(spawnPointModel);
                break;

            case MapObjectType.Waypoint:
                result = (GameObject)Instantiate(WaypointPrototype, model.Position, Quaternion.Euler(model.Rotation));
                break;

            default:
                throw new ApplicationException("Unexpected Map Object type:" + model.MapObjectType);
        }
    }

    protected override void RealizeMesh(MeshDetail meshDetail)
    {
        if (string.IsNullOrEmpty(meshDetail.MeshPath))
            return;

        gameObject.transform.localScale = meshDetail.ObjectScale;

        GameObject meshObject = Resources.Load<GameObject>(meshDetail.MeshPath);
        if (meshObject == null)
            throw new ApplicationException("Could not find a mesh object at path " + meshDetail.MeshPath);

        GameObject instance = (GameObject)Instantiate(meshObject, transform.position, transform.rotation);
        instance.name = "Mesh";
        instance.transform.position = transform.position + meshDetail.MeshOffset;
        instance.transform.localScale = meshDetail.MeshScale;
        instance.transform.parent = gameObject.transform;

        MeshCollider collider = instance.AddComponent<MeshCollider>();
        Mesh collisionMesh = Resources.Load<Mesh>(meshDetail.MeshPath);
        collider.sharedMesh = Instantiate(collisionMesh);

        Animator animator = instance.GetComponent<Animator>();
        if (animator == null)
            return;

        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(meshDetail.AnimationControllerPath);
        animator.runtimeAnimatorController = controller;
    }

    #endregion Realization Methods

}
