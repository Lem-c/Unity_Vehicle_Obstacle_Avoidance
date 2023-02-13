using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Import self scripts
using ActionManager;

public class TestVehicle : VehicleController
{
    // The important values showed on Debug
    public float SpeedMonitor;
    public float BreakMonitor;

    // Key device parameters
    private float MaxRayDistance = 6f;
    private float SpeedScale = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        Vehicle = GameObject.FindWithTag("Player");

        stp_ = new MovementController(Vehicle, MaxRayDistance, 3, 30);
        SetDefaultParam(SpeedScale);
    }

    void Update()
    {
        SpeedMonitor = GetCurrentSpeed();
        BreakMonitor = GetCurrentDeceleration();
    }

    void FixedUpdate()
    {
        Operation();
        DecisionMaker();
    }

    protected override void DecisionMaker()
    {
        QueueCommandOperation();

        stp_.StrightMovementDecisionMaker();
        stp_.TurningDecisionMaker();
    }

    protected override void HandBreak()
    {
        throw new System.NotImplementedException();
    }
}