using System;
using System.Collections.Generic;
using UnityEngine;

public class RevivableActuator : DebuggableBehavior
{
    #region Variables / Properties

    public string ReviveModelName;
    public Lockout ReviveUpdateLockout;

    public List<ModifiableStat> Stats;
    
    private ModifiableStat ReviveRate
    {
        get { return Stats.FindItemByName("ReviveRate"); }
    }

    private ModifiableStat ReviveStatus
    {
        get { return Stats.FindItemByName("ReviveStatus"); }
    }

    private RevivableSensor _revivableSensor;
    private RevivableSensor RevivableSensor
    {
        get
        {
            return _revivableSensor ?? (_revivableSensor = GetComponentInChildren<RevivableSensor>());
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

    private GameUIController _gameUIController;
    private GameUIController GameUIController
    {
        get
        {
            return _gameUIController ?? (_gameUIController = GameUIController.Instance);
        }
    }

    #endregion Variables / Properties

    #region Hooks

    public void Update()
    {
        CheckForReviveProximity();
    }

    #endregion Hooks

    #region Actualization Methods

    public void RealizeModel(RevivableModel model)
    {
        gameObject.name = model.Name;
        gameObject.tag = "Dead Chicken";

        RealizeStats(model);
        RealizeMeshDetails(model.MeshDetail);
    }

    private void RealizeStats(RevivableModel model)
    {
        Stats = model.Stats.DeepCopyList();

        RevivableSensor.Radius = Stats.FindItemByName("DetectionRange").Value;
        RevivableSensor.DetectionLockout.LockoutRate = Stats.FindItemByName("DetectionRate").Value;
    }

    private void RealizeMeshDetails(MeshDetail meshDetail)
    {
        RealizeMesh(meshDetail);
        RealizeMeshRenderer(meshDetail);
    }

    private void RealizeMesh(MeshDetail meshDetail)
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

        Animator animator = instance.GetComponent<Animator>();
        if (animator == null)
            return;

        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(meshDetail.AnimationControllerPath);
        animator.runtimeAnimatorController = controller;
    }

    private void RealizeMeshRenderer(MeshDetail meshDetail)
    {
        Renderer renderer = GetRendererInChildren();
        if (renderer == null)
            throw new InvalidOperationException("Could not find a Renderer in any children of game object " + gameObject.name);

        if (meshDetail.MaterialDetails.IsNullOrEmpty())
            return;

        List<Material> materials = new List<Material>();
        for (int i = 0; i < meshDetail.MaterialDetails.Count; i++)
        {
            MaterialDetail current = meshDetail.MaterialDetails[i];
            Material newMaterial = ParseMaterial(current);
            materials.Add(newMaterial);
        }

        renderer.materials = materials.ToArray();
    }

    private Renderer GetRendererInChildren()
    {
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
            return meshRenderer;

        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
            return skinnedMeshRenderer;

        return null;
    }

    private Material ParseMaterial(MaterialDetail detail)
    {
        Material material = Resources.Load<Material>(detail.MaterialPath);
        if (material == null)
            throw new ApplicationException("Could not find a material at path " + detail.MaterialPath);

        RealizeTextureOnMaterial(material, detail.TexturePath, detail.TexturePropertyName);
        RealizeTextureOnMaterial(material, detail.BumpPath, detail.BumpPropertyName);
        RealizeTextureOnMaterial(material, detail.EmissivePath, detail.EmissivePropertyName);

        return material;
    }

    private void RealizeTextureOnMaterial(Material material, string texturePath, string propertyName)
    {
        if (material == null)
            throw new InvalidOperationException("Cannot realize textures on a null material.");

        if (string.IsNullOrEmpty(texturePath))
            return;

        Texture2D diffuseTexture = Resources.Load<Texture2D>(texturePath);
        material.SetTexture(propertyName, diffuseTexture);
    }

    #endregion Actualization Methods

    #region Methods

    private void CheckForReviveProximity()
    {
        if (!RevivableSensor.HasDetectedEntities())
        {
            DebugMessage("An object that can revive " + gameObject.name + " is not nearby.");
            GameUIController.ShowRevivingGauge(false);
            return;
        }

        GameUIController.ShowRevivingGauge(true);

        if (!ReviveUpdateLockout.CanAttempt())
            return;

        ReviveStatus.Increase(ReviveRate.Value);
        FormattedDebugMessage(LogLevel.Info, "Reviving {0}: {1}/{2}", gameObject.name, ReviveStatus.Value, ReviveStatus.ValueCap);
        GameUIController.UpdateRevivingGauge(ReviveStatus.Value, ReviveStatus.ValueCap);

        ReviveUpdateLockout.NoteLastOccurrence();

        if(ReviveStatus.IsAtMax)
        {
            FormattedDebugMessage(LogLevel.Info, "Reviving {0} as {1}", gameObject.name, ReviveModelName);
            gameObject.SetActive(false);
            MatchEntityManager.SpawnMob(transform.position, transform.rotation, ReviveModelName);
        }
    }

    #endregion Methods
}
