using System;
using System.Collections.Generic;
using UnityEngine;

public class MobActuator : DebuggableBehavior
{
    #region Variables / Properties

    public string MoveAnimation;
    public List<RankedTagReaction> Reactions;
    public Lockout AttentionSpanDecayLockout;
    public List<ModifiableStat> Stats;

    private RankedTagReaction _lastReaction;
    private GameObject _currentTarget;

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

    private Animator _animator;
    private Animator Animator
    {
        get
        {
            return _animator ?? (_animator = GetComponentInChildren<Animator>());
        }
    }

    private MatchWaypointManager _matchWaypointManager;
    private MatchWaypointManager MatchWaypointManager
    {
        get
        {
            return _matchWaypointManager ?? (_matchWaypointManager = MatchWaypointManager.Instance);
        }
    }

    #endregion Variables / Properties

    #region Hooks

    public void Update()
    {
        EvaluateAttentionSpan();
        EvaluateSenses();
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
        MoveAnimation = model.MoveAnimation;
        Reactions = model.Reactions;
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

    private void MoveAwayFromTarget(Vector3 targetPoint)
    {
        MotionDirection direction = GetDirectionToPoint(targetPoint);
        direction = InvertDirection(direction);

        Motion.RotateEntity(direction);
        Motion.MoveEntity();
    }

    private void MoveTowardsTarget(Vector3 targetPoint)
    {
        MotionDirection direction = GetDirectionToPoint(targetPoint);

        Motion.RotateEntity(direction);
        Motion.MoveEntity();
    }

    private MotionDirection GetDirectionToPoint(Vector3 targetPoint)
    {
        MotionDirection direction = MotionDirection.None;

        bool targetToTheRight = targetPoint.x > transform.position.x;
        bool targetToTheLeft = targetPoint.x < transform.position.x;

        bool targetVerticallyAligned = Mathf.Abs(transform.position.z - targetPoint.z) < 0.001f;
        bool targetAhead = targetPoint.z > transform.position.z;
        bool targetBehind = targetPoint.z < transform.position.z;

        if (targetAhead)
        {
            direction = MotionDirection.North;

            if (targetToTheLeft)
                direction = MotionDirection.NorthWest;
            else if (targetToTheRight)
                direction = MotionDirection.NorthEast;
        }
        else if (targetBehind)
        {
            direction = MotionDirection.South;

            if (targetToTheLeft)
                direction = MotionDirection.SouthWest;
            else if (targetToTheRight)
                direction = MotionDirection.SouthEast;
        }
        else if (targetVerticallyAligned)
        {
            if (targetToTheLeft)
                direction = MotionDirection.West;
            else if (targetToTheRight)
                direction = MotionDirection.East;
        }

        return direction;
    }

    private MotionDirection InvertDirection(MotionDirection direction)
    {
        switch(direction)
        {
            case MotionDirection.None:
                return MotionDirection.None;

            case MotionDirection.North:
                return MotionDirection.South;

            case MotionDirection.South:
                return MotionDirection.North;

            case MotionDirection.East:
                return MotionDirection.West;

            case MotionDirection.West:
                return MotionDirection.East;

            case MotionDirection.NorthEast:
                return MotionDirection.SouthWest;

            case MotionDirection.NorthWest:
                return MotionDirection.SouthEast;

            case MotionDirection.SouthEast:
                return MotionDirection.NorthWest;

            case MotionDirection.SouthWest:
                return MotionDirection.NorthEast;

            default:
                throw new ApplicationException("Unexpected direction " + direction);
        }
    }

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

    private void EvaluateSenses()
    {
        if (_currentTarget == null)
        {
            _currentTarget = MatchWaypointManager.FindNearestWaypoint(transform.position);
            Animator.Play(MoveAnimation);
        }

        if(_currentTarget.tag == "Waypoint"
           && Vector3.Distance(transform.position, _currentTarget.transform.position) < 1.0f)
        {
            // TODO: Wait for a couple of seconds in the wait animation, then move again!
            _currentTarget = MatchWaypointManager.FindNearestWaypoint(transform.position, _currentTarget);
        }

        List<GameObject> sensedEntities = Detector.SensedEntities;
        _lastReaction = GetStrongestReactionToSensedEntities(sensedEntities);
        MoveEntity(_lastReaction);

    }

    private RankedTagReaction GetStrongestReactionToSensedEntities(List<GameObject> sensedEntities)
    {
        int winningPriority = 0;
        RankedTagReaction winningReaction = null;

        for (int i = 0; i < sensedEntities.Count; i++)
        {
            GameObject current = sensedEntities[i];
            RankedTagReaction reaction;
            try
            {
                reaction = GetReactionForTag(current.tag);
            }
            catch(MissingReferenceException)
            {
                // The object no longer exists.  Choose something else.
                continue;
            }

            if (reaction == null)
                continue;

            if (reaction.Rank <= winningPriority)
                continue;

            winningReaction = reaction;
            winningReaction.ReactToObject = current;
        }

        return winningReaction;
    }

    private RankedTagReaction GetReactionForTag(string tag)
    {
        RankedTagReaction result = null;

        for(int i = 0; i < Reactions.Count; i++)
        {
            RankedTagReaction current = Reactions[i];
            if (tag != current.Tag)
                continue;

            result = current;
        }

        return result;
    }

    private void MoveEntity(RankedTagReaction reaction)
    {
        // If not reacting to anything, keep moving to the current target.
        if (reaction == null)
        {
            MoveTowardsTarget(_currentTarget.transform.position);
            return;
        }

        // If the behavior is to avoid or purse, change targets.
        if (reaction.Behavior == AIBehavior.Avoid
           || reaction.Behavior == AIBehavior.Pursue)
        {
            _currentTarget = reaction.ReactToObject;
        }

        // Move away from an object we want to avoid.  Otherwise,
        // Move towards a pursued object, or my current target position.
        switch (reaction.Behavior)
        {
            case AIBehavior.Avoid:
                MoveAwayFromTarget(_currentTarget.transform.position);
                break;

            default:
                MoveTowardsTarget(_currentTarget.transform.position);
                break;
        }
    }

    #endregion Methods
}
