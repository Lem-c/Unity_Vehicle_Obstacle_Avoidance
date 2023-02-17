using ActionManager;
using System;
using System.Collections;
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

    /// <summary>
    /// Hardwasre list
    /// </summary>
    ArrayList equipmentList = new();


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

        equipmentList.Add(StrightLidar);
    }

    public bool StraightLidarDetctation()
    {
        return StrightLidar.RayDetection(Target.GetComponent<Transform>());
    }

    public bool LeftLidarDetectation()
    {
        return LeftLidar.RangRayDetection(Target.GetComponent<Transform>(), -1);
    }

    public bool RightLidarDetectation()
    {
        return RightLidar.RangRayDetection(Target.GetComponent<Transform>(), 1);
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

    public void AddNewEquipment(Detector _new)
    {
        equipmentList.Add(_new);
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
            return true;
        }

        return false;
    }

}
