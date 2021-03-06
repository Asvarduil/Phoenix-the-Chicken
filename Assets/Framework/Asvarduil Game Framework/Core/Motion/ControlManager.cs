﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class ControlManager : ManagerBase<ControlManager>
{
    #region Variables / Properties

    public List<AsvarduilControlAxis> ControlAxes;
    public AsvarduilAccelerometer Accelerometer;

    public bool NoControlsPressed
    {
        get
        {
            bool anyPressed = true;
            for(int i = 0; i < ControlAxes.Count; i++)
            {
                AsvarduilControlAxis current = ControlAxes[i];
                if (!current.IsPressed())
                    continue;

                anyPressed = false;
                break;
            }

            return anyPressed;
        }
    }

    #endregion Variables / Properties

    #region Engine Hooks

    public void Start()
    {
        Accelerometer.Initialize();
    }

    #endregion Engine Hooks

    #region Methods

    public void RadiateSuspendCommand()
    {
        SendMessageToAllGameObjects("Suspend");
    }

    public void RadiateResumeCommand()
    {
        SendMessageToAllGameObjects("Resume");
    }

    public Vector3 GetAccelerometer()
    {
        return Accelerometer.GetAccelerometerSmooth();
    }

    public bool GetPositiveAxis(string axisName)
    {
        if (string.IsNullOrEmpty(axisName))
            throw new ArgumentException("Unexpected axis: " + axisName);

        AsvarduilControlAxis axis = ControlAxes.FirstOrDefault(a => a.Name == axisName);
        return axis.IsPositive();
    }

    public bool GetNegativeAxis(string axisName)
    {
        if (string.IsNullOrEmpty(axisName))
            throw new ArgumentException("Unexpected axis: " + axisName);

        AsvarduilControlAxis axis = ControlAxes.FirstOrDefault(a => a.Name == axisName);
        return axis.IsNegative();
    }

    public bool GetAxisDown(string axisName)
    {
        if (string.IsNullOrEmpty(axisName))
            throw new ArgumentException("Unexpected axis: " + axisName);

        AsvarduilControlAxis axis = ControlAxes.FirstOrDefault(a => a.Name == axisName);
        return axis.IsPressed();
    }

    public bool GetAxisUp(string axisName)
    {
        if (string.IsNullOrEmpty(axisName))
            throw new ArgumentException("Unexpected axis: " + axisName);

        AsvarduilControlAxis axis = ControlAxes.FirstOrDefault(a => a.Name == axisName);
        return axis.IsReleased();
    }

    public float GetAxis(string axisName)
    {
        if (string.IsNullOrEmpty(axisName))
            throw new ArgumentException("Unexpected axis: " + axisName);

        AsvarduilControlAxis axis = ControlAxes.FirstOrDefault(a => a.Name == axisName);
        return axis.GetAxis();
    }

    private static void SendMessageToAllGameObjects(string message)
    {
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < allGameObjects.Length; i++)
        {
            GameObject thisObject = allGameObjects[i];
            thisObject.SendMessage(message, SendMessageOptions.DontRequireReceiver);
        }
    }

    #endregion Methods
}
