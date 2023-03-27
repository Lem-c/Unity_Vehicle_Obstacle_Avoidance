using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using UnityEngine;
using VehicleEqipment.Camera;
using VehicleEqipment.Lidar;

public class VehicleHardWare
{
    private GameObject Target;           // The main vehicle

    private int DetectiveLayer;          // The target layer where obstacles located
    private float RayMaxDistance;
    private float StraightBias = 7.1f;     // Normally the distance estimated ahead is further, decrease

    // Default hardware
    LidarDetector StrightLidar;
    LidarDetector LeftLidar;
    LidarDetector RightLidar;
    
    CameraDetector UpperCamera;


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
        UpperCamera = new CameraDetector(_target, DetectiveLayer, _rayDistance, 25);
    }

    /**************** Lidar detection methods **********************/
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

    /************* Camera detection result ***********************/
    public bool CameraDetect()
    {
        var cam = GameObject.FindWithTag("SceneView").GetComponent<Camera>();
        var isClosing_ = false;
        UpperCamera.ProcessCamera(cam, 0.3f, 0.35f, ref isClosing_);                 // Set weight for angle and distance

        // Debug.Log(isClosing_);

        return isClosing_;
    }

    public List<float> GetCameraData(int index)
    {
        return UpperCamera.GetIndexOfDataMap(index);
    }

    public int GetLengthOfCameraData()
    {
        return UpperCamera.GetLengthOfDataset();
    }

    public void CleanCameraData()
    {
        UpperCamera.CleanDataMap();
    }

    /****************** Get Methods **************************/

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

    /// <summary>
    /// Convert vehicle position to matrix
    /// </summary>
    /// <returns>DenseMatrix</returns>
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
