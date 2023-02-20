using ActionManager;
using DecisionMake;
using UnityEngine;
using VehicleEqipment.Lidar;

public class VecicleDecisionPlatform
{
    // The target game object in unity
    private GameObject Target;
    // Get access of hardware (lidar, camera, etc.)
    public VehicleHardWare motherBoard;
    // Main Decision determination model
    public MovementStep stepManager;

    public VecicleDecisionPlatform(GameObject _car, int _layer, float _maxSpeed, float _MaxRayDistance)
    {
        Target = _car;
        motherBoard = new VehicleHardWare(Target, _MaxRayDistance, _layer);
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
                    Target.GetComponent<LightCar>().Speed,
                    motherBoard.DistanceToObstacle());

        /*// If meet wall
        if(motherBoard.WhetherNeedTurning())
        {
            if(motherBoard.LeftLidarDetectation())
            {
                stepManager.RandomTurning(60);
                return;
            }

            stepManager.RandomTurning(-60);
        }*/
    }

    /// <summary>
    /// Generate turning operations using sensors data from mother board
    /// </summary>
    public void GenerateTurningMovement()
    {
        // Process lidar detection first
        motherBoard.LeftLidarDetectation();
        motherBoard.RightLidarDetectation();

        stepManager.TurningDecisionMaker(Target.GetComponent<LightCar>().Speed,
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
