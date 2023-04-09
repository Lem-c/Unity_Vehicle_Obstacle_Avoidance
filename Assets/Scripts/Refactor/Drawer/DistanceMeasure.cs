using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeasure
{
    private GameObject vehicle;
    float timeGap { get; set; }
    float timer = 0;

    // point recorder
    private Vector3 lastPoint;
    // result distance
    private float distance { get; set; }

    public DistanceMeasure(GameObject _target, float _timeGap)
    {
        vehicle = _target;
        timeGap = _timeGap;
        lastPoint = vehicle.transform.position;
    }

    public float GetMoveDistance()
    {
        return distance;
    }

    public void UpdateDistance()
    {
        if (vehicle != null)
        {
            timer += 0.1f;

            if (timer >= timeGap)
            {
                distance += Vector3.Distance(lastPoint, vehicle.transform.position);
                lastPoint = vehicle.transform.position;
                timer = 0;
            }
        }
    }
}
