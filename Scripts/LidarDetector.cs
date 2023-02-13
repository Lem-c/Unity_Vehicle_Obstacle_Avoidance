using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LidarDetector
{
    private RaycastHit hit;
    private bool isHit = false;

    // Setting params
    private int LayerMask { set; get; }
    private float MaxRayDistance;


    // Changing variables
    private float angleBias_y;    // The rotation on y-axis [Have to greater than zero, rotation angle]
    private float tempAngle;

    public LidarDetector(int _layer, float _rayDistance, float _sideVisualAngle=0)
    {
        LayerMask = _layer;
        MaxRayDistance = _rayDistance;
        angleBias_y = _sideVisualAngle;
        tempAngle = angleBias_y;
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
            MaxRayDistance, LayerMask))
        {

            isHit = true;
            return isHit;
        }

        isHit = false;
        return false;
    }

    public float DistanceTo()
    {
        if (!isHit)
        {
            throw new Exception("Trying call null ref of ray distance detective");
        }

        return hit.distance;
    }

    private bool RayDetection(Vector3 _from, Vector3 _direction)
    {
        if (Physics.Raycast(_from,
            _direction,
            out hit,
            MaxRayDistance, LayerMask))
        {

            isHit = true;
            return true;
        }

        isHit = false;
        return false;
    }

    /// <summary>
    /// The main method that this class has.
    /// Can be used to detect the obstacles that lays on a rang (sector)
    /// The complexity requires improve.
    /// </summary>
    /// <param name="_side">The bias added(turn left or right)</param>
    /// <param name="_target">The tartget object that equipped with ray detector</param>
    /// <returns>Whether deteced the obstacles</returns>
    public bool RangRayDetection(int _side, Transform _target, int _checkTime = 5)
    {
        if ((_side != -1 && _side != 1) || angleBias_y <= 0)
        {
            throw new ArgumentException("The direction choice or turning bias value is incorrect!");
        }

        // The rotation temporary variables
        float currentBias = 0;
        float tempBias = 0;

        // The init hit bool
        bool isDetected = false;

        Vector3 tempDirection;

        int activateTimes = 0;

        while (!isDetected && activateTimes < _checkTime)
        {
            // dynamice change the detected direction
            if (currentBias < angleBias_y + 0.5f)
            {
                currentBias += 1f;
                tempBias = _side * currentBias * (angleBias_y / 5);
            }

            // Add 'tempBias' to the y-axis
            tempDirection = Quaternion.Euler(0, tempBias, 0) *
                            _target.TransformDirection(Vector3.forward);

            isDetected = RayDetection(_target.position, tempDirection);

            DrawRay(_target.position, tempDirection);

            activateTimes += 1;
        }

        return isDetected;
    }

    public void ShrinkAngle(int _minus)
    {
        if(angleBias_y -_minus <= 0)
        {
            return;
        }

        SetYBiasAngle(angleBias_y - _minus);
    }

    public void RecoverAngle()
    {
        angleBias_y = tempAngle;
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

    public bool GetIsHit()
    {
        return isHit;
    }
}