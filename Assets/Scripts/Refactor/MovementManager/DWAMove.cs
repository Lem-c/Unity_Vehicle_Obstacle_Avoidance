using ActionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DWAMove : StepController, DynamicWindow
{
    // Default Data processer
    private readonly float MaxSpeed;
    private readonly float WSize;

    // saved angle modified last time
    private float lastAngle;
    // The weight list {SpeedWeight, DistanceWeight}
    private float[] weightList;
    // Evaluation grades list
    private List<float> evalSet;
    private List<float> angleSet;


    /// <param name="_maxSpeed"></param>
    /// <param name="_initAngle"></param>
    /// <param name="_weight">{speedWeight, destinationWeight}</param>
    /// <param name="_bias">MaxDecisionBias = _bias</param>
    /// <param name="_size">The size of windows</param>
    public DWAMove(float _maxSpeed, float _initAngle, float[] _weight, float _bias = 0.05f, float _size=10) : base(_bias)
    {
        // KalmanFilter = new KalmanFilter();
        MaxSpeed = _maxSpeed;
        WSize = _size;

        lastAngle = _initAngle;
        weightList = _weight;

        // Dataset saves the evaluate result and angle which decides the turning degree
        evalSet = new List<float>();
        angleSet = new List<float>();
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
            if (evalSet.Count >= WSize && evalSet.Count > 1)
            {
                var tempAngle = angleSet[GetMaxObjectiveInList()];
                // Add new movement
                TurningDegreeAdd(tempAngle);

                evalSet.Clear();
                angleSet.Clear();
                return;
            }

            evalSet.Add(DwaObjective(_speed, _angle, _isClose, _dis2obs));
            angleSet.Add(_angle);
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

        var _bias = 0.1f;

        return SpeedGain(_speed, weightList[0])
             + DestinationGain(_angle, weightList[1], _isClose)
             + ObstaclePenalty(_obsL, _bias);
    }
    public float DwaObjective(float _speedGain, float _destinationGain, float _obsPenalty)
    {
        return _speedGain + _destinationGain + _obsPenalty;
    }


    /****************** Tuning method *************************************/

    // TODO
    public void UpdateWeight()
    {
        weightList[0] += 1;
    }

    /// <summary>
    /// Get the largest objective value in list
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
    public float DestinationGain(float _angle, float _weight, bool _isClose)
    {
        float result = 0;                                           // output result
        lastAngle = _angle;                                         // update angle

        // Evaluate the importance of turn angle
        double gap = Math.Sqrt(Math.Pow((_angle - lastAngle), 2));
        if(gap < MaxDecisionBias)
        {
            gap = 0;
        }

        // Evaluate whether move towards to the destination
        result += (float)gap * _weight * Discriminator(_isClose);
        return result;
    }

    public float ObstaclePenalty(float _dis2obs, float _bias=0.1f)
    {
        if(_dis2obs< 0)
        {
            return 10 * _bias;
        }

        return _dis2obs * _bias;
    }

    public float SpeedGain(float _speed, float _weight, float _deceleration=1)
    {
        float result = 0;                           // The output result

        // speed gain
        if(_speed < MaxSpeed / 2)
        {
            result += _speed * _weight;
        }
        else
        {
            result -= _speed * _weight;
        }
        // break penalty
        result -= (_speed % _deceleration + 0.1f) * (_weight-MaxDecisionBias*10);

        return result;
    }

    /// <summary>
    /// Add turning operations according to the angle
    /// </summary>
    private void TurningDegreeAdd(float _angle)
    {
        if(-MaxDecisionBias< _angle || _angle <= MaxDecisionBias)
        {
            AddNewRecord(MoveMent.MoveForward);
        }

        var viewAngle = _angle / -15f;

        if(_angle< 0)
        {
            AddNumOfTurning((int)viewAngle, -1);
        }
        else
        {
            AddNumOfTurning((int)viewAngle, 1);
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
