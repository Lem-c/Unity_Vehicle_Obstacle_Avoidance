using UnityEngine;
using UnityEngine.SceneManagement;

public class DWACar : VehicleController
{
    // Key device params
    private readonly float MaxRayDistance = 9f;
    public Dashboard dashboard;

    public static int costCount = 0;
    public static float routeLength = 0;

    // Counter obj
    public DistanceMeasure dm;

    /******************Unity methods************************/
    // Start is called before the first frame update
    void Start()
    {
        Vehicle = GameObject.FindWithTag("Player");
        dashboard = new Dashboard();
        dm = new DistanceMeasure(Vehicle, 1f);

        // float[] tempWeight = { 0.094f, 0.13f, 0.6f };
        float[] tempWeight = { 1.333f, 2,657, 3,123f };
        /**
         * Remember to adjust the camera weight either
         */

        SetDefaultParam(SelfScale);
        // TODO: When updating 'LightCar' using method: SetDefaultParam,
        // MaxSpeed.. in vdp class would not update correspondingly
        vdp = new VecicleDecisionPlatform(Vehicle, 3, MaxSpeed, MaxRayDistance, "DWA", 1f, tempWeight);
    }

    // Update is called once per frame
    void Update()
    {
        // Process decisions
        QueueCommandOperation();
        // Call op to realize movement
        Operation();
        // Generate decisions
        ProcessDecision();

        // Speed value Monitor
        dashboard.Speed = GetCurrentSpeed();
        dashboard.NowBreak = GetCurrentDeceleration();
        // Debug.Log(GetCurrentSpeed());
    }

    void FixedUpdate()
    {
        if (isStart_)
        {
            // data
            dm.UpdateDistance();
            routeLength = dm.GetMoveDistance();
        }
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
    /// Override method of how the operations are taken out from queue
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private new void QueueCommandOperation()
    {
        if (vdp is null)
        {
            throw new System.Exception("Null step manager: VecicleDecisionProcess");
        }

        while (vdp.lidarHelper.GetLengthOfRecord() > 0)
        {
            ChangeCurrentMove(vdp.lidarHelper.PopNextMove());
            // Debug.Log(GetCurrentMove());
            ActionApply();
        }

        // Continue action execution
        while (vdp.stepManager.GetLengthOfRecord() > 1)
        {
            ChangeCurrentMove(vdp.stepManager.PopNextMove());
            // Debug.Log(GetCurrentMove());
            ActionApply();
        }
    }

    /// <summary>
    /// Main decision maker method controls the vehicle
    /// Call it in the fixed update plz
    /// </summary>
    protected override void ProcessDecision()
    {
        if (!isStart_) { return; }

        // vdp.GenerateStraightMovement();
        vdp.GenerateTurningMovement();
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
