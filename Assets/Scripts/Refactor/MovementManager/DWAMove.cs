using ActionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VehicleEqipment.Camera;

public class DWAMove : StepController, DynamicWindow
{
    // Default Data processer
    private readonly float MaxSpeed;
    // saved angle modified last time
    private float lastAngle;
    private float lastDis = 100f;
    // The weight list {SpeedWeight, DistanceWeight}
    private float[] weightList;
    // Evaluation grades list
    private List<float> evalSet;
    private List<float> angleSet;
    private List<float> distanceSet;  


    /// <param name="_maxSpeed"></param>
    /// <param name="_initAngle"></param>
    /// <param name="_weight">{speedWeight, destinationWeight, obstacleBias}</param>
    /// <param name="_bias">MaxDecisionBias = _bias</param>
    /// <param name="_size">The size of windows</param>
    public DWAMove(float _maxSpeed, float _initAngle, float[] _weight, float _bias = 0.05f, int _size=10) : base(_bias)
    {
        // KalmanFilter = new KalmanFilter();
        MaxSpeed = _maxSpeed;
        StepSize = _size;

        lastAngle = _initAngle;
        weightList = _weight;

        // Dataset saves the evaluate result and angle which decides the turning degree
        evalSet = new List<float>();
        angleSet = new List<float>();
        distanceSet = new List<float>();
    }
    
    public override void StrightMovementDecisionMaker(float _speed, float _dist)
    {
        AddNewRecord(MoveMent.MoveForward);
    }

    /// <summary>
    /// Overrode method generates turning operations
    /// </summary>
    public override void TurningDecisionMaker(float _speed, float _angle, float _dis2obs, bool _isClose)
    {
        try {
            if (evalSet.Count >= StepSize && evalSet.Count > 1)
            {
                var tempAngle = angleSet[GetMaxObjectiveInList()];
                var farObs = distanceSet[GetMaxObjectiveInList()];
                // Add new movement to avoid obstacles
                TurningDegreeAdd(tempAngle, farObs, _isClose);
                // Debug.Log(_isClose);

                evalSet.Clear();
                angleSet.Clear();
                distanceSet.Clear();

                lastDis = farObs;
                return;
            }

            // Debug.Log(DwaObjective(_speed, _angle, _isClose, _dis2obs));

            // Add evaluated parameters and obstacle angles into list
            evalSet.Add(DwaObjective(_speed, _angle, _isClose, _dis2obs));
            angleSet.Add(_angle);
            distanceSet.Add(_dis2obs);
        }
        catch { 
            throw new ArgumentException();
        }
    }

    /// <summary>
    ///  Method used to evaluate situation from data array
    /// </summary>
    public float DwaObjective(float _speed, float _angle, bool _isClose, float _obsL)
    {
        // TODO: array check || bias set

        /*Debug.Log(SpeedGain(_speed, weightList[0]) + ", " + DestinationGain(_angle, weightList[1], _isClose) + ", " +
            ObstaclePenalty(_obsL, weightList[2]));*/

        return SpeedGain(_speed, weightList[0])
             + DestinationGain(_angle, weightList[1], _isClose)
             + ObstaclePenalty(_obsL, weightList[2]);
    }
    public float DwaObjective(float _speedGain, float _destinationGain, float _obsPenalty)
    {
        return _speedGain + _destinationGain + _obsPenalty;
    }


    /****************** Tuning method *************************************/

    // TODO
    // Decrease one when encountered different situation
    public void UpdateWeight()
    {
        weightList[0] += 1;
    }

    /// <summary>
    /// Get the largest objective value in evaluation list
    /// </summary>
    /// <returns>the index of max value in: evalSet</returns>
    private int GetMaxObjectiveInList()
    {
        try {
            // Call int perameter using float cause unavoidable system crash 
            int index = evalSet.FindIndex(x => Math.Abs(x - evalSet.Max()) < 0.1f);

            return index;
        }
        catch(Exception) {
            return -1;
        }
    }

    /******************* Methods not used directly ************************/
    /// <summary>
    /// Determine whether close to the destination
    /// Using camera given data
    /// </summary>
    /// <param name="_angle">Current angle between vehicle and the obstacle</param>
    /// <param name="_weight">weight bias</param>
    /// <param name="_isClose">Camera given data: whether close to the destination</param>
    /// <returns></returns>
    public float DestinationGain(float _angle, float _weight, bool _isClose)
    {
        float result = 0;                                           // output result

        // Evaluate the importance of turn angle
        double gap = Math.Sqrt(Math.Pow((Math.Abs(_angle) - Math.Abs(lastAngle)), 2));
        if(gap < MaxDecisionBias)
        {
            gap = 0;
        }

        // Evaluate whether move towards to the destination
        result += (float)gap * _weight * Discriminator(_isClose);

        lastAngle = _angle;                                         // update angle
        return result;
    }

    public float ObstaclePenalty(float _dis2obs, float _bias=0.1f)
    {
        if(_dis2obs < 0)
        {
            return 10 * _bias;                                     // Add as compensation
        }

        return -_bias * (float)Math.Pow(_dis2obs/10,2) + MaxDecisionBias*10;
    }

    /// <summary>
    /// Determine whether vehicle moved too fast to reach the destination
    /// </summary>
    /// <param name="_speed"></param>
    /// <param name="_weight"></param>
    /// <param name="_deceleration"></param>
    /// <returns></returns>
    public float SpeedGain(float _speed, float _weight, float _deceleration=1)
    {
        float result = 0;                           // The output result

        // speed gain
        result += -1 * (_speed * _speed) + (MaxSpeed/10 - MaxDecisionBias*10) * _speed;

        // Restriction
        if(result < -0.4f)
        {
            result = -0.5f;
        }

        // break penalty
        result -= (_speed % _deceleration + 0.1f) * (_weight-MaxDecisionBias*10);

        return result;
    }

    /// <summary>
    /// Add turning operations according to the angle
    /// </summary>
    private void TurningDegreeAdd(float _angle, float _dis2obs=0, bool _isClosing=false)
    {
        var viewAngle = Math.Abs(_angle/10)+1;

        if(_isClosing)
        {
            if (viewAngle < 0 && (lastDis - _dis2obs) > 0)
            {
                AddNumOfTurning((int)viewAngle, -1);
            }
            else if (viewAngle > 0 && (lastDis - _dis2obs) > 0)
            {
                AddNumOfTurning((int)viewAngle, 1);
            }
            else
            {
                // Random move
                RandomTurning();
            }
        }
        else
        {
            // Debug.Log("Last: " + lastAngle + " Current: " + _angle);

            if ( lastAngle*_angle > 0 && lastAngle > _angle)
            {
                if (GetParallelVectorDistance() > 0)
                {
                    AddNumOfTurning((int)viewAngle + UnityEngine.Random.Range(0, 2), 1);
                }
                else
                {
                    AddNumOfTurning((int)viewAngle + UnityEngine.Random.Range(0, 2), -1);
                }
            }
        }
    }

    private void AddNumOfTurning(int _time, int _side)
    {
        var count = _time;                              // How many times an instruction would be added

        while(count > 0)
        {
            if(_side > 0)
            {
                AddNewRecord(MoveMent.TurnRight);
            }
            else
            {
                AddNewRecord(MoveMent.TurnLeft);
            }

            count--;
        }
    }

    /// <summary>
    /// Generate two vectors from left and right side of vehicle
    /// Compare the distance that from them to the destination
    /// Take the smaller one
    /// </summary>
    /// <param name="_bias"></param>
    /// <returns></returns>
    private float GetParallelVectorDistance(float _bias=5)
    {
        float x = CameraDetector.Target.transform.position.x;
        float z = CameraDetector.Target.transform.position.z;
        float[] pointLeft = { x - _bias, 0, z};
        float[] pointRight = { x + _bias, 0, z};

        float d_x = CameraDetector.destination.x;
        float d_z = CameraDetector.destination.z;
        float[] pointDes = { d_x, 0, d_z };

        // Get two line vectors
        Vector3 left = CameraDetector.GetVectorFromTwoPoint(pointDes, pointLeft);
        Vector3 right = CameraDetector.GetVectorFromTwoPoint(pointDes, pointRight);
        // Calculate magnitude  of vector
        // float L_dis = (float)Math.Sqrt(left.x * left.x + left.z*left.z);
        // float R_dis = (float)Math.Sqrt(right.x * right.x + right.z * right.z);
        float L_dis = left.magnitude;
        float R_dis = right.magnitude;

        return L_dis - R_dis;
    }

    /************************ Internal/External import methods ****************/
    /// <summary>
    /// Is the value reach the fire value
    /// </summary>
    /// <param name="_val">The checked value</param>
    /// <returns>bool</returns>
    public static bool ActiveFunction(float _val, float _threshold = 0)
    {

        if (_val < _threshold)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Used to calculate whether add/minus the gain
    /// </summary>
    /// <param name="_val">boolvalue</param>
    /// <returns>+1 / -1</returns>
    public static int Discriminator(bool _val)
    {
        if (_val)
        {
            return 1;
        }

        return -1;
    }

    /*** Debug methods***/
    public override void PrintMessage()
    {
        if (evalSet.Count > 0 && angleSet.Count > 0)
        {
            Debug.Log(evalSet[0] + ", " + angleSet[0]);
        }
    }
}
