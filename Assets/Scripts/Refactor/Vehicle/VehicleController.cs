using UnityEngine;
using ActionManager;
using Unity.VisualScripting;

public abstract class VehicleController : MonoBehaviour
{
    // The Main common params
    protected float MaxSpeed;
    protected float ReverseMaxSpeed;
    protected float Acceleration;
    protected float Deceleration;
    protected float ReverseAcceleration;

    /*// The physical params
    private float Friction = 1.2f;
    private float Gravity = 10f;*/

    // The real-time params
    private float currentSpeed = 0;     // The current speed of vehicle
    private float turnAngle;
    private MovementStep.MoveMent currentMove;  // Current operation

    // In-game object reference [Essential part]
    protected GameObject Vehicle;       // Get Vehicle Object
    protected VecicleDecisionPlatform vdp;         // Logic process model

    // State of vehicle
    protected bool isStart_ = false;
    protected float SelfScale = 0.1f;

    // The method used to auto set parameters as default value
    protected void SetDefaultParam(float _scale = 1f)
    {
        MaxSpeed = 50f * _scale;
        ReverseMaxSpeed = -25f * _scale;     // Negative
        Acceleration = 3f * _scale;
        Deceleration = 6.5f * _scale;
        ReverseAcceleration = 1.5f * _scale;
    }

    /// <summary>
    /// Dash board class
    /// Serialized variables shows in the inspector
    /// Create obj in class before use
    /// </summary>
    public class Dashboard
    {
        public double Speed { get; set; }
        public double NowBreak { get; set; }
    }

    /***********************Abstact methods*************/
    protected abstract void HandBreak();
    protected abstract void ProcessDecision();


    /************Moving control methods*****************/
    protected void MoveForawrd()
    {
        if(currentSpeed >= MaxSpeed)
        {
            return;
        }

        currentSpeed += Time.deltaTime * Acceleration;
    }

    protected void MoveBackward()
    {
        if(currentSpeed < 0)
        {
            return;
        }

        currentSpeed -= Time.deltaTime * Deceleration;
    }
    protected void TurnLeft()
    {
        turnAngle = -1 * 60f * Time.deltaTime;
        transform.Rotate(0, turnAngle, 0);
    }

    protected void TurnRight()
    {
        turnAngle = 1 * 60f * Time.deltaTime;
        transform.Rotate(0, turnAngle, 0);
    }

    protected void ReverseVehicle()
    {
        if(currentSpeed > ReverseMaxSpeed || currentSpeed > 0)
        {
            return;
        }

        currentSpeed -= Time.deltaTime * ReverseAcceleration;
    }
    
    /***The main operation control the transformer to move***/
    protected void Operation()
    {
        if (!isStart_) { return; }
        transform.Translate(GetCurrentSpeed() * Time.deltaTime * Vector3.forward);   
    }

    protected void QueueCommandOperation()
    {
        if (vdp is null)
        {
            throw new System.Exception("Null step manager: VecicleDecisionProcess");
        }

        if (vdp.stepManager.GetLengthOfRecord() > 0)
        {
            currentMove = vdp.stepManager.PopNextMove();
            // Debug.Log(currentMove);
            ActionApply();
        }
    }

    /// <summary>
    /// Method used with 'Operation()' 
    /// Guide vehilce movement by selecting actions
    /// </summary>
    protected void ActionApply()
    {
        if (currentMove == MovementStep.MoveMent.MoveForward)
        {
            MoveForawrd();
        }

        if (currentMove == MovementStep.MoveMent.MoveBackward
            || currentMove == MovementStep.MoveMent.Break
            || currentMove == MovementStep.MoveMent.LightBreak
            || currentMove == MovementStep.MoveMent.SlamBreak)
        {
            MoveBackward();
        }

        if (currentMove == MovementStep.MoveMent.TurnLeft)
        {
            TurnLeft();
        }

        if (currentMove == MovementStep.MoveMent.TurnRight)
        {
            TurnRight();
        }

        if (currentMove == MovementStep.MoveMent.Wait)
        {
            // When no operation
        }
    }

    /***************Value change/Get methods****************/
    protected void ChangeCurrentSpeed(float _new)
    {
        currentSpeed = _new;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetCurrentDeceleration()
    {
        return Deceleration;
    }

    // Return a percentage value
    // Represent a ratio that means how much percent reaching max speed;
    protected float GetSpeedRatio()
    {
        if(currentSpeed < 0)
        {
            return currentSpeed / ReverseMaxSpeed * 1f;
        }

        return currentSpeed / MaxSpeed * 1f;
    }

    public void ChangeStartState()
    {
        isStart_ = !isStart_;
    }

    public bool GetCurrentState()
    {
        return isStart_;
    }

    protected void ChangeCurrentMove(MovementStep.MoveMent _newStep)
    {
        currentMove = _newStep;
    }


    protected MovementStep.MoveMent GetCurrentMove()
    {
        return currentMove;
    }
    /// <summary>
    /// Useed in UI-control button method
    /// Change the start position of vehicle
    /// Fix y-axis
    /// </summary>
    /// <param name="_x">The horizontal coordinate</param>
    /// <param name="_z">The vertical cooricate</param>
    /// <exception cref="System.Exception"></exception>
    public void ChangeStartPosition(float _x = 0.5f, float _z = -5.2f)
    {
        if (Vehicle.IsUnityNull())
        {
            throw new System.Exception("Empty Vehilce!");
        }

        Transform tempTrans = Vehicle.GetComponent<Transform>();
        Vector3 tempPos = new Vector3(_x, tempTrans.position.y, _z);

        Vehicle.GetComponent<Transform>().position = tempPos;
    }
}
