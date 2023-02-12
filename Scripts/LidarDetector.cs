using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarDetector
{
    private RaycastHit hit;

    // Setting params
    private int LayerMask { set; get; }
    private float MaxRayDistance;


    // Changing variables
    private float angleBias_y;    // The rotation on y-axis

    public LidarDetector(int _layer, float _rayDistance)
    {
        LayerMask = 1 << _layer;
        MaxRayDistance = _rayDistance;
    }

    /// <summary>
    /// Overload method that check whether there is an object 
    /// On the route of the ray
    /// </summary>
    /// <param name="_target">The target that equipped with the ray </param>
    /// <returns>Bool value whther detected obstacle</returns>
    public bool RayDetection(Transform _target)
    {
        if (Physics.Raycast(_target.position,
            _target.TransformDirection(Vector3.forward),
            out hit,
            MaxRayDistance, LayerMask)){

            return true;
        }

        return false;
    }

    private bool RayDetection(Vector3 _from, Vector3 _direction)
    {
        if (Physics.Raycast(_from,
            _direction,
            out hit,
            MaxRayDistance, LayerMask))
        {

            return true;
        }

        return false;
    }

    /// <summary>
    /// The main method that this class has.
    /// Can be used to detect the obstacles that lays on a rang (sector)
    /// The complexity requires improve.
    /// </summary>
    /// <param name="_side">The bias added(turn left or right)</param>
    /// <param name="_bias">Have to greater than zero, rotation angle</param>
    /// <param name="_target">The tartget object that equipped with ray detector</param>
    /// <returns>Whether deteced the obstacles</returns>
    public bool RangRayDetection(int _side, int _bias, Transform _target, int _checkTime = 5)
    {
        if((_side != -1 && _side != 1) || _bias < 0)
        {
            throw new ArgumentException("The direction choice or turning bias value is incorrect!");
        }

        // The rotation temporary variables
        float currentBias = 0;
        float tempBias = 0;

        Vector3 tempDirection;

        bool isDetected = false;
        int activateTimes = 0;

        while (!isDetected && activateTimes < _checkTime)
        {
            // dynamice change the detected direction
            if (currentBias < _bias+0.5f)
            {
                currentBias += 1f;
                tempBias = _side * currentBias * 4;
            }

            // Add 'tempBias' to the y-axis
            tempDirection = new Vector3(_target.position.x + tempBias,
                                         (_target.position.y),
                                         _target.position.z + 1);

            isDetected = RayDetection(_target.position, tempDirection);

            DrawRay(_target.position, tempDirection);

            activateTimes += 1;
        }


        return isDetected;
    }

    /// <summary>
    /// Debug method drawing the ray in unity Debug
    /// </summary>
    /// <param name="_from"></param>
    /// <param name="_direction"></param>
    private void DrawRay(Vector3 _from, Vector3 _direction)
    {
        Debug.DrawRay(_from, _direction, Color.red);
    }


    // Value modify methods
    protected void SetYBiasAngle(float _new)
    {
        angleBias_y = _new;
    }

    public float GetYBiasAngle()
    {
        return angleBias_y;
    }
}
