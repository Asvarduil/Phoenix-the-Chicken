using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActuator : DebuggableBehavior
{
    #region Variables

    public PlayerState PlayerState;
    public Lockout AscensionLockout;
    public int AscensionLevelForNextPhase;
    public string AscensionEffectPath;
    public string NextPlayerModelName;
    public List<string> TagsThatCauseDeath;
    public string DeathModelName;

    public string VerticalAxis;
    public string HorizontalAxis;

    #endregion Variables

    #region Properties

    public List<ModifiableStat> Stats;

    private ModifiableStat AscensionLevel
    {
        get
        {
            return Stats.FindItemByName("AscensionLevel");
        }
    }

    private ModifiableStat AscensionLockoutRate
    {
        get
        {
            return Stats.FindItemByName("AscensionLockoutRate");
        }
    }

    private ModifiableStat AscensionRate
    {
        get
        {
            return Stats.FindItemByName("AscensionRate");
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

    private ControlManager _controlManager;
    private ControlManager ControlManager
    {
        get
        {
            return _controlManager ?? (_controlManager = ControlManager.Instance);
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

    #endregion Properties

    #region Hooks

    public void Start()
    {
        GameUIController.ChangePhoenixGaugeState(PlayerState, AscensionLevel.Value, AscensionLevel.ValueCap);
    }

    public void Update()
    {
        ReadControlAxes();
        UpdatePhaseGauge();
    }

    public void OnCollisionEnter(Collision collision)
    {
        CheckForFailure(collision.collider);
    }

    #endregion Hooks

    #region Realization Methods

    public void RealizeModel(PlayerModel model)
    {
        gameObject.name = model.Name;
        gameObject.tag = model.Tag;
        
        RealizeStats(model);
        RealizeMeshDetails(model.MeshDetail);
    }

    public void ResetActuator(PlayerModel model)
    {
        RealizeStats(model);
        GameUIController.ChangePhoenixGaugeState(PlayerState, AscensionLevel.Value, AscensionLevel.ValueCap);
    }

    private void RealizeStats(PlayerModel model)
    {
        PlayerState = model.PlayerState;
        AscensionLevelForNextPhase = model.AscensionLevelForNextPhase;
        AscensionEffectPath = model.AscensionEffectPath;
        NextPlayerModelName = model.NextPlayerModelName;
        TagsThatCauseDeath = model.TagsThatCauseDeath.DeepCopyList();
        DeathModelName = model.DeathModelName;

        Stats = model.Stats.DeepCopyList();

        ModifiableStat moveSpeed = Stats.FindItemByName("MoveSpeed");
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

        GameObject instance = (GameObject) Instantiate(meshObject, transform.position, transform.rotation);
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
            throw new InvalidOperationException("Could not find a MeshRenderer in any children of game object " + gameObject.name);

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
        if(material == null)
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

    #region Control Methods

    private void CheckForFailure(Collider who)
    {
        if (!TagsThatCauseDeath.Contains(who.tag))
            return;

        // The thing that collided with us does, in fact, cause death.
        MatchEntityManager.InactivatePlayerAvatar(gameObject, PlayerState);
        MatchEntityManager.SpawnMob(transform.position, transform.rotation, DeathModelName);

        // TODO: Short delay, music change, then show failure UI.
        GameUIController.ShowFailureUI();
    }

    private void ReadControlAxes()
    {
        if (ControlManager.NoControlsPressed)
        {
            Motion.HaltEntity();
            return;
        }

        float verticalDirection = ControlManager.GetAxis(VerticalAxis);
        float horizontalDirection = ControlManager.GetAxis(HorizontalAxis);

        MotionDirection direction = MotionDirection.None;
        if (verticalDirection > 0.0f)
        {
            if (Mathf.Abs(horizontalDirection - 0.0f) < 0.001f)
                direction = MotionDirection.North;
            else if (horizontalDirection > 0.0f)
                direction = MotionDirection.NorthEast;
            else if (horizontalDirection < 0.0f)
                direction = MotionDirection.NorthWest;
        }
        else if (verticalDirection < 0.0f)
        {
            if (Mathf.Abs(horizontalDirection - 0.0f) < 0.001f)
                direction = MotionDirection.South;
            else if (horizontalDirection > 0.0f)
                direction = MotionDirection.SouthEast;
            else if (horizontalDirection < 0.0f)
                direction = MotionDirection.SouthWest;
        }
        else if (Mathf.Abs(verticalDirection - 0.0f) < 0.001f)
        {
            if (horizontalDirection > 0.0f)
                direction = MotionDirection.East;
            else if (horizontalDirection < 0.0f)
                direction = MotionDirection.West;
        }

        FormattedDebugMessage(LogLevel.Info, "Vertical: {0} Horizontal: {1} D-Value: {2}", verticalDirection, horizontalDirection, direction.ToString());

        Motion.RotateEntity(direction);
        Motion.MoveEntity();
    }

    private void UpdatePhaseGauge()
    {
        if(AscensionLevel == default(ModifiableStat)
           || AscensionRate == default(ModifiableStat)
           || AscensionLockoutRate == default(ModifiableStat))
        {
            FormattedDebugMessage(LogLevel.Info, "There is no AscensionLevel, AscensionRate, or AscensionLockoutRate stat tied to Player {0}", gameObject.name);
            return;
        }

        if (AscensionLockout.LockoutRate != AscensionLockoutRate.Value)
            AscensionLockout.LockoutRate = AscensionLockoutRate.Value;

        if (!AscensionLockout.CanAttempt())
            return;

        AscensionLevel.Value += AscensionRate.Value;
        if (AscensionLevel.Value > AscensionLevel.ValueCap)
            AscensionLevel.Value = AscensionLevel.ValueCap;
        else if (AscensionLevel.Value < 0)
            AscensionLevel.Value = 0;

        GameUIController.UpdatePhoenixGauge(AscensionLevel.Value, AscensionLevel.ValueCap);
        AscensionLockout.NoteLastOccurrence();

        CheckForPhaseChange();
    }

    private void CheckForPhaseChange()
    {
        if (AscensionLevel.Value != AscensionLevelForNextPhase)
        {
            DebugMessage("Cannot ascend; AscensionLevel = " + AscensionLevel.Value + ", AscensionLevelForNextPhase = " + AscensionLevelForNextPhase);
            return;
        }

        DebugMessage("Ascension occurring!");

        // Instantiate the ascension effect...
        if (!string.IsNullOrEmpty(AscensionEffectPath))
        {
            GameObject ascensionEffect = Resources.Load<GameObject>(AscensionEffectPath);
            Instantiate(ascensionEffect, transform.position, transform.rotation);
        }

        // TODO: Implement a player object pooling manager...
        MatchEntityManager.InactivatePlayerAvatar(gameObject, PlayerState);

        if(!string.IsNullOrEmpty(NextPlayerModelName))
        {
            MatchEntityManager.SwitchToNewPlayerAvatar(transform.position, transform.rotation, NextPlayerModelName);
        }
    }

    #endregion Control Methods
}
