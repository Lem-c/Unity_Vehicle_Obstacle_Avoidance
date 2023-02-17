using UnityEngine;
using ActionManager;

public class LightCar : VehicleController
{
    // Serialized variables shows in the inspector
    public float Speed;
    public float nowBreak;

    // Key device params
    private readonly float MaxRayDistance = 15f;
    private float SelfScale = 0.3f;


    /******************Unity methods************************/
    // Start is called before the first frame update
    void Start()
    {
        Vehicle = GameObject.FindWithTag("Player");
        vdp = new VecicleDecisionProcess(Vehicle, 3, MaxSpeed, MaxRayDistance);
        
        SetDefaultParam(SelfScale);
    }

    // Update is called once per frame
    void Update()
    {
        // Call op to realize movement
        Operation();
        // Speed value Monitor
        Speed = GetCurrentSpeed();
        nowBreak = GetCurrentDeceleration();
    }

    void FixedUpdate()
    {
        ProcessDecision();
    }

    /*********************Class methods***********************/
    // Main control or override methods
    protected override void HandBreak()
    {
        ChangeCurrentSpeed(0);
        isStart_ = false;
    }

    /// <summary>
    /// Main decision maker method controls the vehicle
    /// Call it in the fixed update plz
    /// </summary>
    protected override void ProcessDecision()
    {
        // Get straight movement decision from agent
        vdp.GenerateStraightMovement();
        vdp.GenerateTurningMovement();
        // Process decisions
        QueueCommandOperation();
    }
}
