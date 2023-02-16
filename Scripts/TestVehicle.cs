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

    // Temp value saver
    private float tempScale;

    // Start is called before the first frame update
    void Start()
    {
        Vehicle = GameObject.FindWithTag("Player");

        stp_ = new MovementController(Vehicle, MaxRayDistance, 3, 28);
        SetDefaultParam(GetSpeedScale());
        tempScale = GetSpeedScale();
    }

    void Update()
    {
        SpeedMonitor = GetCurrentSpeed();
        BreakMonitor = GetCurrentDeceleration();

        if(tempScale != GetSpeedScale())
        {
            tempScale= GetSpeedScale();
            SetDefaultParam(GetSpeedScale());
        }
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

        if (stp_.GetIsForwardBlocked() || Random.Range(0,100)<50)
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