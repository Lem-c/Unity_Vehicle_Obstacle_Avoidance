using ActionManager;
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
    public MovementStep lidarHelper;

    public string sensorType;

    public VecicleDecisionPlatform(GameObject _car, int _layer, float _maxSpeed, float _maxRayDistance, string _DType="Smooth",
                                   float _startAngle = 0, float[] _weight = null)
    {
        Target = _car;
        // connect to hardware
        motherBoard = new VehicleHardWare(Target, _maxRayDistance, _layer);
        // TODO: Exchange this as switchable/decleared parameter
        sensorType = _DType;
        stepManager = StepController.GenerateStepManager(_DType, _maxSpeed, _maxRayDistance, _startAngle, _weight);
        if (!_DType.Equals("Smooth"))
        {
            // Set _MaxDecisionBias either
            lidarHelper = StepController.GenerateStepManager("Smooth", _maxSpeed, _maxRayDistance, 0, null, 1.5f);
        }
    }

    /************All Initialize/decision making methods***************/
    /// <summary>
    /// Generate straight-movement operations using sensors data from mother board
    /// </summary>
    public void GenerateStraightMovement()
    {
        if (sensorType == "Smooth")
        {
            // Process lidar detection first
            motherBoard.StraightLidarDetctation();
            // Get distance and speed information
            stepManager.StrightMovementDecisionMaker(
                        Target.GetComponent<LightCar>().GetCurrentSpeed(),
                        (float)motherBoard.DistanceToObstacle());
        }
        else if (sensorType == "DWA")
        {
            motherBoard.StraightLidarDetctation();
            lidarHelper.StrightMovementDecisionMaker(
                        Target.GetComponent<DWACar>().GetCurrentSpeed(),
                        (float)motherBoard.DistanceToObstacle());
        }
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
            // Lidar Assistance
            // Process lidar detection first : obstacle avoidance
            motherBoard.LeftLidarDetectation();
            motherBoard.RightLidarDetectation();
            lidarHelper.TurningDecisionMaker((float)Target.GetComponent<DWACar>().GetCurrentSpeed(),
                                             motherBoard.DistanceToObstacle(2),
                                             motherBoard.DistanceToObstacle(3),
                                             motherBoard.GetIsForwardBlocked());

            // Process camera sensor => is close to the target
            var closingState = motherBoard.CameraDetect();

            // Generate decisions
            stepManager.SetStepSize(motherBoard.GetLengthOfCameraData());   // Update the windows size
            // Do window loop
            for (int i = 0; i < motherBoard.GetLengthOfCameraData(); i++)
            {
                // data array contains angle and distance
                var data = motherBoard.GetCameraData(i);                    

                // catch when without no mouse click action
                if (data == null || data.Count < 1) {
                    continue;                                               
                }

                // Debug.Log("Angle: " + data[0] + " dis2obs: " + data[1]);

                // Process DM to add data into lists
                stepManager.TurningDecisionMaker(Target.GetComponent<DWACar>().GetCurrentSpeed(),
                                                data[0], data[1], closingState);
                // stepManager.PrintMessage();
            }

            // Debug.Log(stepManager.GetLengthOfRecord());

            // clean storage of a window
            motherBoard.CleanCameraData();
        }
    }

}
