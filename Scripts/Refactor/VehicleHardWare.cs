using MathNet.Numerics.LinearAlgebra.Single;
using System;
using UnityEngine;
using VehicleEqipment;
using VehicleEqipment.Camera;
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
    CameraDetector UpperCamera;

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
        UpperCamera = new CameraDetector();

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

    /// <summary>
    /// Using a target camera to get the position of mouse in the SceneWorld
    /// Can only be used when mouse click and a target camera
    /// </summary>
    /// <param name="_cam">The target camera</param>
    public void CheckIsCloseToDestination(Camera _cam)
    {
        if (!Input.GetMouseButtonUp(0)){ return; }

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isCollider = Physics.Raycast(ray, out hit);
        if (isCollider)
        {
            Debug.Log(UpperCamera.IsAngleShrink(hit.point, Target.GetComponent<Transform>().position));
        }
        
    }

    /// <summary>
    /// Get lidar distance measurement result according to the type
    /// </summary>
    /// <param name="_type">
    /// 1:Straight Lidar
    /// 2:Left
    /// 3:Right
    /// </param>
    /// <returns></returns>
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

    public bool WhetherNeedTurning()
    {
        if (LeftLidar.DistanceTo() > 0 || RightLidar.DistanceTo() > 0)
        {
            return true;
        }

        return false;
    }
}
