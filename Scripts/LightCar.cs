using UnityEngine;
using ActionManager;
using UnityEngine.SceneManagement;

public class LightCar : VehicleController
{
    // Serialized variables shows in the inspector
    public float Speed;
    public float nowBreak;

    // Key device params
    private readonly float MaxRayDistance = 13f;


    /******************Unity methods************************/
    // Start is called before the first frame update
    void Start()
    {
        Vehicle = GameObject.FindWithTag("Player");
        
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

        // Speed value Monitor
        Speed = GetCurrentSpeed();
        nowBreak = GetCurrentDeceleration();
    }

    void FixedUpdate()
    {
        if (isStart_)
        {
            ProcessDecision();
        }
    }
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
        if (!vdp.motherBoard.GetIsForwardBlocked())
        {
            // Get straight movement decision from agent
            vdp.GenerateStraightMovement();
        }
    }

    public void SetScale(float _scale)
    {
        SelfScale = _scale;
        SetDefaultParam(SelfScale);
    }
}
