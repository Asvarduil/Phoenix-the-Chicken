using System;

public class SpawnPointActuator : BaseActuator<SpawnPointModel>
{
    #region Hooks

    #endregion Hooks

    #region Realization Methods

    public override void RealizeModel(SpawnPointModel model)
    {
        gameObject.name = model.Name;

        RealizeMeshDetails(model.MeshDetail);
    }

    public override void ResetActuator(SpawnPointModel model)
    {
        throw new NotImplementedException("Spawn Points do not need to be reset.");
    }

    #endregion Realization Methods
}
