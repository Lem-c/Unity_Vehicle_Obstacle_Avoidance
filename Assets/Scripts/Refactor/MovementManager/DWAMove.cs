using ActionManager;
using System;

public class DWAMove : StepController, DynamicWindow
{
    // Default Data processer
    // private KalmanFilter KalmanFilter;
    private readonly float MaxSpeed;
    // saved angle modified last time
    private float lastAngle;
    // The weight list {SpeedWeight, DistanceWeight}
    private float[] weightList;

    public DWAMove(float _maxSpeed, float _initAngle, float[] _weight, float _bias = 0.05f) : base(_bias)
    {
        // KalmanFilter = new KalmanFilter();
        MaxSpeed = _maxSpeed;
        lastAngle = _initAngle;
        weightList = _weight;
    }

    public override void StrightMovementDecisionMaker(float _speed, float _dist)
    {
        throw new NotImplementedException();
    }

    public override void TurningDecisionMaker(float _speed, float _leftDis, float _rightDis, bool _isForwardblocked)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Overload method used to evaluate situation from data array
    /// </summary>
    /// <param name="_speedL">{float _speed, float _deceleration}</param>
    /// <param name="_disL">
    /// {float _angle,
    ///  float distance change from last to now: D(last)-D(now)}
    /// </param>
    /// <param name="_obsL">{float _dis2obs, float _bias=0.1f}</param>
    /// <returns>The value of evaluation</returns>
    public float DwaObjective(float[] _speedL, float[] _disL, float[] _obsL)
    {
        // TODO: array check

        return SpeedGain(_speedL[0], weightList[0], _speedL[1])
             + DestinationGain(_disL[0], weightList[1], ActiveFunction(_disL[1]))
             + ObstaclePenalty(_obsL[0], _obsL[1]);
    }

    public float DwaObjective(float _speedGain, float _destinationGain, float _obsPenalty)
    {
        return _speedGain + _destinationGain + _obsPenalty;
    }


    // TODO
    public void UpdateWeight()
    {
        weightList[0] += 1;
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
        return -(_dis2obs * _bias);
    }

    public float SpeedGain(float _speed, float _deceleration, float _weight)
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
        result -= (_speed % _deceleration) * (_weight-MaxDecisionBias*10);

        return result;
    }

    public bool ActiveFunction(float _val)
    {
        if (_val < 0)
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
    private int Discriminator(bool _val)
    {
        if (_val)
        {
            return 1;
        }

        return -1;
    }

    /*** Debug methods***/
    public void PrintMessage()
    {
        // Debug.Log( vdp.motherBoard.KalmanEstimation());
        // vdp.motherBoard.KalmanFilter.WriteLine();
    }
}
