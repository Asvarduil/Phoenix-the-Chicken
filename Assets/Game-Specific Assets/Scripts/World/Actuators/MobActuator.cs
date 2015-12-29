using System;
using System.Collections.Generic;
using UnityEngine;

public enum AIBehavior
{
    Ignore,
    Pursue,
    Avoid
}

public class MobActuator : DebuggableBehavior
{
    #region Variables / Properties

    public Lockout AttentionSpanDecayLockout;
    public List<ModifiableStat> Stats;

    private ModifiableStat AttentionSpan
    {
        get
        {
            return Stats.FindItemByName("AttentionSpan");
        }
    }

    private ModifiableStat AttentionSpanDecay
    {
        get
        {
            return Stats.FindItemByName("AttentionSpanDecay");
        }
    }

    private ModifiableStat AttentionSpanDecayRate
    {
        get
        {
            return Stats.FindItemByName("AttentionSpanDecayRate");
        }
    }

    private EntityDetector _detector;
    private EntityDetector Detector
    {
        get
        {
            return _detector ?? (_detector = GetComponentInChildren<EntityDetector>());
        }
    }

    private EntityMotion _motion;
    private EntityMotion Motion
    {
        get
        {
            return _motion ?? (_motion = GetComponent<EntityMotion>());
        }
    }

    #endregion Variables / Properties

    #region Hooks

    public void Update()
    {
        EvaluateAttentionSpan();
    }

    #endregion Hooks

    #region Realization Methods

    public void RealizeModel(AIModel model)
    {
        gameObject.name = model.Name;
        gameObject.tag = model.Tag;

        RealizeStats(model);
        RealizeMeshDetails(model.MeshDetail);
    }

    public void ResetActuator(AIModel model)
    {
        RealizeStats(model);
    }

    private void RealizeStats(AIModel model)
    {
        Stats = model.Stats.DeepCopyList();

        ModifiableStat senseRange = Stats.FindItemByName("SensingRange");
        if(senseRange != null)
            Detector.Radius = senseRange.Value;

        ModifiableStat moveSpeed = Stats.FindItemByName("MoveSpeed");
        if(moveSpeed != null)
            Motion.MovementSpeed = moveSpeed.Value;
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

        GameObject meshInstance = (GameObject)Instantiate(meshObject, transform.position, transform.rotation);
        meshInstance.name = "Mesh";
        meshInstance.transform.position = transform.position + meshDetail.MeshOffset;
        meshInstance.transform.localScale = meshDetail.MeshScale;
        meshInstance.transform.parent = gameObject.transform;

        Animator animator = meshInstance.GetComponent<Animator>();
        if (animator == null)
            return;

        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(meshDetail.AnimationControllerPath);
        if(controller != null)
            animator.runtimeAnimatorController = controller;

        // If the path is wrong, perform no animations!
    }

    private void RealizeMeshRenderer(MeshDetail meshDetail)
    {
        //MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        //SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
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

    #endregion Realization Methods

    #region AI Methods

    private void EvaluateAttentionSpan()
    {
        if(AttentionSpan == default(ModifiableStat)
           || AttentionSpanDecay == default(ModifiableStat)
           || AttentionSpanDecayRate == default(ModifiableStat))
        {
            FormattedDebugMessage(LogLevel.Info, "There is no AttentionSpan, AttentionSpanDecay, or AttentionSpanDecayRate stat tied to AI {0}", gameObject.name);
            return;
        }

        AttentionSpanDecayLockout.LockoutRate = AttentionSpanDecayRate.Value;
        if (!AttentionSpanDecayLockout.CanAttempt())
            return;

        if (AttentionSpan.Value <= 0.0f)
            Destroy(gameObject);

        AttentionSpan.Value -= AttentionSpanDecay.Value;
        FormattedDebugMessage(LogLevel.Info, "AI entity {0}'s attention level is now {1}", gameObject.name, AttentionSpan.Value);
        AttentionSpanDecayLockout.NoteLastOccurrence();
    }

    #endregion Methods
}
