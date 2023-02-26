using UnityEngine;
using UnityEngine.SceneManagement;
using static LightCar;

public class LightCar : VehicleController
{
    // Serialized variables shows in the inspector
    public class Dashboard
    {
        public double Speed;
        public double nowBreak;
    }

    // Key device params
    private readonly float MaxRayDistance = 13f;
    public Dashboard dashboard;


    /******************Unity methods************************/
    // Start is called before the first frame update
    void Start()
    {
        Vehicle = GameObject.FindWithTag("Player");
        dashboard = new Dashboard();


        SetDefaultParam(SelfScale);
        // TODO: When updating 'LightCar' using method: SetDefaultParam,
        // MaxSpeed.. in vdp class would not update correspondingly
        vdp = new VecicleDecisionPlatform(Vehicle, 3, MaxSpeed, MaxRayDistance);
    }

    // Update is called once per frame
    void Update()
    {
        // Process decisions
        QueueCommandOperation();
        // Call op to realize movement
        Operation();

        // vdp.motherBoard.CheckIsCloseToDestination(GameObject.FindWithTag("SceneView").GetComponent<Camera>());

        // Speed value Monitor
        dashboard.Speed = GetCurrentSpeed();
        dashboard.nowBreak = GetCurrentDeceleration();
    }

    void FixedUpdate()
    {
        if (isStart_)
        {
            ProcessDecision();
        }

        // Debug.Log( vdp.motherBoard.KalmanEstimation());
        // vdp.motherBoard.KalmanFilter.WriteLine();
    }

    /// <summary>
    /// If vehicle hit to the edge of the map,
    /// Restart the scene
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            // ToDo
            SceneManager.LoadScene(1);
        }
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
        vdp.GenerateTurningMovement();
        vdp.GenerateStraightMovement();
        // Debug.Log(vdp.stepManager.GetLengthOfRecord());

        /*if (!vdp.motherBoard.GetIsForwardBlocked() && !vdp.motherBoard.WhetherNeedTurning())
        {
            // Get straight movement decision from agent
            vdp.GenerateStraightMovement();
        }*/
    }

    public void SetScale(float _scale)
    {
        SelfScale = _scale;
        SetDefaultParam(SelfScale);
    }
}
