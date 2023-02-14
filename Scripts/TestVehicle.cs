using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Import self scripts
using ActionManager;
using UnityEngine.SceneManagement;

public class TestVehicle : VehicleController
{
    // The important values showed on Debug
    public float SpeedMonitor;
    public float BreakMonitor;

    // Key device parameters
    private float MaxRayDistance = 15f;
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
        if (isStart_)
        {
            QueueCommandOperation();
            Operation();
            DecisionMaker();
        }
    }

    protected override void DecisionMaker()
    {
        stp_.StrightMovementDecisionMaker();

        if (stp_.GetIsForwardBlocked())
        {
            stp_.TurningDecisionMaker();
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) 
        {
            SceneManager.LoadScene(scene_);
        }
    }

    protected override void HandBreak()
    {
        throw new System.NotImplementedException();
    }
}