using MathNet.Numerics.LinearAlgebra.Single;
using System;
using UnityEngine;
using VehicleEqipment;
using VehicleEqipment.Lidar;

public class VehicleHardWare
{
    private GameObject Target;           // The main vehicle

    private int DetectiveLayer;          // The target layer where obstacles located
    private float RayMaxDistance;
    private float StraightBias = 7.1f;     // Normally the distance estimated ahead is further

    // Default hardware
    LidarDetector StrightLidar;
    LidarDetector LeftLidar;
    LidarDetector RightLidar;

    // Default Data processer
    public KalmanFilter KalmanFilter;


    public VehicleHardWare(GameObject _target, float _rayDistance, int _detectiveLayer)
    {
        if (_target == null)
        {
            throw new ArgumentException("Null object reference! Set tag as: 'Player'");
        }

        Target = _target;
        DetectiveLayer = 1 << _detectiveLayer;
        RayMaxDistance = _rayDistance;

        StrightLidar = new LidarDetector(DetectiveLayer, RayMaxDistance);
        LeftLidar = new LidarDetector(DetectiveLayer, RayMaxDistance - StraightBias, 20);
        RightLidar = new LidarDetector(DetectiveLayer, RayMaxDistance - StraightBias, 20);

        KalmanFilter = new KalmanFilter();
    }

    public DenseMatrix KalmanEstimation()
    {
        return KalmanFilter.ProcessKalmanFilter(UpdateVehiclePosition());
    }

    public bool StraightLidarDetctation()
    {
        var isBlocked = StrightLidar.RayDetection(Target.GetComponent<Transform>());
        RecoverLidarAngle();
        return isBlocked;
    }

    public bool LeftLidarDetectation()
    {
        var isBlocked = LeftLidar.RangRayDetection(Target.GetComponent<Transform>(), -1);
        LeftLidar.ShrinkAngle(10);

        return isBlocked;
    }

    public bool RightLidarDetectation()
    {
        var isBlocked = RightLidar.RangRayDetection(Target.GetComponent<Transform>(), 1);
        RightLidar.ShrinkAngle(10);

        return isBlocked;
    }

    public float DistanceToObstacle(int _type=1)
    {
        if(_type == 1)
        {
            return StrightLidar.DistanceTo();
        }
        else if(_type == 2)
        {
            return LeftLidar.DistanceTo();
        }
        else
        {
            return RightLidar.DistanceTo();
        }
    }

    /// <summary>
    /// Return radar detected result
    /// Whether there is obstacle in front of.
    /// </summary>
    /// <returns>True means blocked</returns>
    public bool GetIsForwardBlocked()
    {
        if (StrightLidar.RayDetection(Target.GetComponent<Transform>()))
        {
            if(StrightLidar.DistanceTo() <= 0.25f * RayMaxDistance)
            {
                return true;
            }
            return false;
        }

        return false;
    }

    public void RecoverLidarAngle()
    {
        LeftLidar.RecoverAngle();
        RightLidar.RecoverAngle();
    }

    private DenseMatrix UpdateVehiclePosition()
    {
        float[,] tempPos = new float[,] { { Target.GetComponent<Transform>().position.x,
                                            Target.GetComponent<Transform>().position.z} };

        return DenseMatrix.OfArray(tempPos);
    }

    /*public bool WhetherNeedTurning()
    {
        if(!GetIsForwardBlocked())
        {
            return false;
        }

        if(LeftLidar.DistanceTo()<0 || RightLidar.DistanceTo() < 0)
        {
            return true;
        }

        return false;
    }*/
}
