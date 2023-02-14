using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Wingman;
using ActionManager;

public abstract class VehicleController : MonoBehaviour
{
    // The Main common params
    protected float MaxSpeed;
    protected float ReverseMaxSpeed;
    protected float Acceleration;
    protected float Deceleration;
    protected float ReverseAcceleration;

    // The physical params
    private float Friction = 1.2f;
    private float Gravity = 10f;

    // The real-time params
    private float currentSpeed = 0;     // The current speed of vehicle
    private float turnAngle;
    private StepManager.MoveMent currentMove;

    // Class ref
    protected GameObject Vehicle;       // Get Vehicle Object
    protected StepManager stp_;         // Main operations distribution class

    // State
    protected bool isStart_ = true;
    protected int scene_ = 1;

    // The method used to auto set parameters as default value
    protected void SetDefaultParam(float _scale = 1f)
    {
        MaxSpeed = 50f * _scale;
        ReverseMaxSpeed = -25f * _scale;     // Negative
        Acceleration = 3f * _scale;
        Deceleration = 6.5f * _scale;
        ReverseAcceleration = 1.5f * _scale;
    }

    // Moving control methods

    protected void MoveForawrd()
    {
        if(currentSpeed >= MaxSpeed)
        {
            return;
        }

        currentSpeed = currentSpeed + Time.deltaTime * Acceleration;
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

    protected void SimulateFriction()
    {
        currentSpeed -= Time.deltaTime * Friction;
    }
    
    // Abstact methods
    protected abstract void HandBreak();
    protected abstract void DecisionMaker();


    // The main operation control the transformer to move
    protected void Operation()
    {
        transform.Translate(Vector3.forward * GetCurrentSpeed() * Time.deltaTime);
    }

    // Value change methods
    protected void SetGravityOfVehicle(float _g)
    {
        Gravity = _g;
    }

    protected void ChangeCurrentSpeed(float _new)
    {
        currentSpeed = _new;
    }

    protected float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    protected float GetCurrentDeceleration()
    {
        return Deceleration;
    }

    protected void ChangeCurrentMoveOperation(StepManager.MoveMent _op)
    {
        currentMove = _op;
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

    // The method choose a resonable deceleration value
    // The _percentage means the rate approaching the upper speed limit
    // Simulate different strength
    protected void FuzzyDecelerationChoice(StepManager.MoveMent situation, float _scale = 1f)
    {
        if(stp_ == null)
        {
            return;
        }

        ChangeCurrentMoveOperation(situation);

        switch (situation)
        {
            case StepManager.MoveMent.LightBreak:
                Deceleration = 1f* _scale + Random.Range(0f, 0.3f)*Random.Range(-1, 1);
                break;
            case StepManager.MoveMent.Break:
                Deceleration = 2f* _scale + Random.Range(0f, 0.3f)*Random.Range(-1, 1);
                break;
            case StepManager.MoveMent.SlamBreak:
                Deceleration = 4.8f* _scale + Random.Range(0f, 0.3f)*Random.Range(-1, 1);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Method used with 'Operation()' 
    /// Guide vehilce movement
    /// </summary>
    private void ActionApply()
    {
        if(currentMove == StepManager.MoveMent.MoveForward)
        {
            MoveForawrd();
        }

        if(currentMove == StepManager.MoveMent.MoveBackward 
            || currentMove == StepManager.MoveMent.Break
            || currentMove == StepManager.MoveMent.LightBreak)
        {
            MoveBackward();
        }

        if(currentMove == StepManager.MoveMent.SlamBreak)
        {
            float tempDec = Deceleration;
            Deceleration *= 4;
            MoveBackward();
            Deceleration = tempDec;
        }

        if(currentMove == StepManager.MoveMent.TurnLeft)
        {
            TurnLeft();
        }

        if(currentMove == StepManager.MoveMent.TurnRight)
        {
            TurnRight();
        }

        if(currentMove == StepManager.MoveMent.Wait)
        {
            // When no operation
        }
    }


    /// <summary>
    /// Main metod that can be used to apply actions
    /// </summary>
    /// <exception cref="System.Exception">The stepManager has not been init</exception>
    protected void QueueCommandOperation()
    {
        if(stp_ is null)
        {
            throw new System.Exception("Null step manager");
        }

        if (stp_.IsThereAnyInstructions())
        {
            currentMove = stp_.ProcessNextMove();
            // Debug.Log(currentMove);
            ActionApply();
        }
    }

    public void StartSim()
    {
        isStart_ = !isStart_;
    }

    public void SetScene(int _scene)
    {
        scene_ = _scene;
    }
}
