using ActionManager;
using DecisionMake;
using UnityEngine;
using VehicleEqipment.Lidar;

public class VecicleDecisionProcess
{
    // The target game object in unity
    private GameObject Target;
    // Get access of hardware (lidar, camera, etc.)
    public VehicleHardWare motherBoard;
    // Main Decision determination model
    public MovementStep stepManager;

    public VecicleDecisionProcess(GameObject _car, int _layer, float _maxSpeed=20f, float _MaxRayDistance=15f)
    {
        Target = _car;
        motherBoard = new VehicleHardWare(Target, _MaxRayDistance, _layer);
        stepManager = new SmoothMovement(_maxSpeed, _MaxRayDistance);
    }

    /************All Initialize/decision making methods***************/
    public void GenerateStraightMovement()
    {
        // Process lidar detection first
        motherBoard.StraightLidarDetctation();
        // Get distance and speed information
        stepManager.StrightMovementDecisionMaker(
                    Target.GetComponent<LightCar>().Speed,
                    motherBoard.DistanceToObstacle());
    }

    public void GenerateTurningMovement()
    {
        // Process lidar detection first
        motherBoard.LeftLidarDetectation();
        motherBoard.RightLidarDetectation();

        stepManager.TurningDecisionMaker(motherBoard.DistanceToObstacle(2),
                                         motherBoard.DistanceToObstacle(3),
                                         motherBoard.GetIsForwardBlocked());
    }


    /********************Debug methods****************************/
    public void PrintCurrentSituation()
    {
        throw new System.NotImplementedException();
    }
}
