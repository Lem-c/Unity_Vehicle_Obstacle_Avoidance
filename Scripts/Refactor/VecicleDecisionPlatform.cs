using ActionManager;
using DecisionMake;
using UnityEngine;
using VehicleEqipment.Lidar;

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

    public VecicleDecisionPlatform(GameObject _car, int _layer, float _maxSpeed, float _MaxRayDistance)
    {
        Target = _car;
        motherBoard = new VehicleHardWare(Target, _MaxRayDistance, _layer);
        // TODO: Exchange this as switchable/decleared parameter
        stepManager = new SmoothMovement(_maxSpeed, _MaxRayDistance);
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
                    (float)Target.GetComponent<LightCar>().dashboard.Speed,
                    (float)motherBoard.DistanceToObstacle());
    }

    /// <summary>
    /// Generate turning operations using sensors data from mother board
    /// </summary>
    public void GenerateTurningMovement()
    {
        // Process lidar detection first
        motherBoard.LeftLidarDetectation();
        motherBoard.RightLidarDetectation();

        stepManager.TurningDecisionMaker((float)Target.GetComponent<LightCar>().dashboard.Speed,
                                         motherBoard.DistanceToObstacle(2),
                                         motherBoard.DistanceToObstacle(3),
                                         motherBoard.GetIsForwardBlocked());
    }


    /********************Debug methods****************************/
    public void PrintCurrentSituation()
    {
        throw new System.NotImplementedException();
    }
}
