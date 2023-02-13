using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Wingman;
using ActionManager;

public class LightCar : VehicleController
{

    public float Speed;
    public float nowBreak;

    // Key device params
    private float MaxRayDistance = 1.5f;
    private float SelfScale = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        Vehicle = GameObject.FindWithTag("Player");
        stp_ = new MovementController(Vehicle, MaxRayDistance);

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

        DecisionMaker();
        // Obstacle detected aciton simulation
        LidarDetection();
    }


    // Main control or override methods
    protected override void HandBreak()
    {
        ChangeCurrentSpeed(0);
    }

    /// <summary>
    /// Using a short rang lidar to detec obstacles
    /// If ray retrun a true value means there is obstacles in front
    /// Decrease the speed
    /// </summary>
    protected void LidarDetection()
    {
        if (AssistMethod.ObstacleDetective(3, Vehicle.transform, MaxRayDistance + 5f))
        {
            Debug.Log("Far range lidar acivate");
        }

        // Emergency situation
        if(AssistMethod.ObstacleDetective(3, Vehicle.transform, MaxRayDistance))
        {
            HandBreak();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Obs")
        {
            // ToDo
        }
    }

    /// <summary>
    /// Main decision maker method controls the vehicle
    /// </summary>
    protected override void DecisionMaker()
    {
        throw new System.NotImplementedException();
    }
}
