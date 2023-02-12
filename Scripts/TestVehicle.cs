using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Import self scripts
using Wingman;
using ActionManager;

public class TestVehicle : VehicleController
{
    // The important values showed on Debug
    public float SpeedMonitor;
    public float BreakMonitor;

    // Key device parameters
    private float MaxRayDistance = 10f;
    // private float SpeedScale = 0.3f;

    // Equipment List
    LidarDetector leftRadar;

    // Start is called before the first frame update
    void Start()
    {
        Vehicle = GameObject.FindWithTag("Player");
        leftRadar = new LidarDetector(3, MaxRayDistance);

        /*stp_ = new MovementController(Vehicle);
        SetDefaultParam(SpeedScale);*/
    }

    // Update is called once per frame
    void Update()
    {
        if(leftRadar.RangRayDetection(-1, 20, Vehicle.GetComponent<Transform>()))
        {
            Debug.Log("Danger!");
        }
    }

    protected override void DecisionMaker()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandBreak()
    {
        throw new System.NotImplementedException();
    }
}
