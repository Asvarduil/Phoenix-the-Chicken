using System.Collections.Generic;
using UnityEngine;

public class EntityMotion : DebuggableBehavior
{
    #region Variables / Properties

    public bool CanMove = true;
    public bool CanTurn = true;

    public List<DirectionVectorPair> MovementDirections;
    public float MovementSpeed;

    private Vector3 _frameVelocity = Vector3.zero;

    #endregion Variables / Properties

    #region Hooks

    public void Update()
    {
        PerformMotion();
    }

    #endregion Hooks

    #region Methods

    public void HaltEntity()
    {
        _frameVelocity = Vector3.zero;
    }

    public void MoveEntity()
    {
        _frameVelocity = Vector3.forward * MovementSpeed * Time.deltaTime;
    }

    public void PerformMotion()
    {
        if (!CanMove)
            return;

        transform.Translate(_frameVelocity);
    }

    public void RotateEntity(MotionDirection direction)
    {
        if (!CanTurn)
            return;

        Vector3 rotation = GetDirectionByMotionDirection(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 0.5f);
    }

    private Vector3 GetDirectionByMotionDirection(MotionDirection direction)
    {
        Vector3 rotation = Vector3.zero;
        for(int i = 0; i < MovementDirections.Count; i++)
        {
            DirectionVectorPair current = MovementDirections[i];
            if (current.Direction != direction)
                continue;

            rotation = current.EulerAngles;
            break;
        }

        return rotation;
    }

    #endregion Methods
}
