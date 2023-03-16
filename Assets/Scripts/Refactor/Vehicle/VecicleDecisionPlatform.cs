using ActionManager;
using System.Collections.Generic;
using UnityEngine;

public class VecicleDecisionPlatform
{
    /// <summary>
    /// The target game object in unity
    /// </summary>
    private GameObject Target;
    /// <summary>
    /// Get access of hardware (lidar, camera, etc.)
    /// </summary>
    public VehicleHardWare motherBoard;
    /// <summary>
    /// Main Decision determination model
    /// </summary>
    public MovementStep stepManager;

    public string sensorType;

    public VecicleDecisionPlatform(GameObject _car, int _layer, float _maxSpeed, float _MaxRayDistance, string _DType="Smooth",
                                   float _startAngle = 0, float[] _weight = null)
    {
        Target = _car;
        motherBoard = new VehicleHardWare(Target, _MaxRayDistance, _layer);
        // TODO: Exchange this as switchable/decleared parameter
        sensorType = _DType;
        stepManager = StepController.GenerateStepManager(_DType, _maxSpeed, _MaxRayDistance, _startAngle, _weight);
    }

    /************All Initialize/decision making methods***************/
    /// <summary>
    /// Generate straight-movement operations using sensors data from mother board
    /// </summary>
    public void GenerateStraightMovement()
    {
        // Process lidar detection first
        motherBoard.StraightLidarDetctation();
        // Get distance and speed information
        stepManager.StrightMovementDecisionMaker(
                    Target.GetComponent<LightCar>().GetCurrentSpeed(),
                    (float)motherBoard.DistanceToObstacle());
    }

    /// <summary>
    /// Generate turning operations using sensors data from mother board
    /// </summary>
    public void GenerateTurningMovement()
    {
        if (sensorType == "Smooth")
        {
            // Process lidar detection first
            motherBoard.LeftLidarDetectation();
            motherBoard.RightLidarDetectation();

            stepManager.TurningDecisionMaker((float)Target.GetComponent<LightCar>().GetCurrentSpeed(),
                                             motherBoard.DistanceToObstacle(2),
                                             motherBoard.DistanceToObstacle(3),
                                             motherBoard.GetIsForwardBlocked());
        }
        else if (sensorType == "DWA")
        {
            // Process camera sensor
            var state = motherBoard.CameraDetect();

            // Generate decisions
            if (state[1] == true)
            {
                for (int i = 0; i < motherBoard.GetLengthOfCameraData(); i++)
                {
                    var data = motherBoard.GetCameraData(i);                    // data array contains angle and distance
                    // List<float> data = new List<float>{0.1f, 0.2f};

                    if(data == null || data.Count < 1) {
                        continue;                                               // catch when without no mouse click action
                    }

                    // Debug.Log(data[0] + ", " + data[1]);

                    stepManager.TurningDecisionMaker(Target.GetComponent<DWACar>().GetCurrentSpeed(),
                                                    data[0], data[1], state[0]);
                    // stepManager.PrintMessage();
                }
            }

            // clean storage
            motherBoard.CleanCameraData();
        }
    }

}
