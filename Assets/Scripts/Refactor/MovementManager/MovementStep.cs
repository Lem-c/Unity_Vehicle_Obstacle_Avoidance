using System.Collections;
using System.Collections.Generic;
using DecisionMake;
using static DecisionMake.DecisionMaker;

namespace ActionManager
{
    /// <summary>
    /// The class used to etimate and guide every step of 
    /// object from Vehicl class.
    /// TODO: Save and reload all actions/steps
    /// </summary>
    public abstract class MovementStep
    {
        // The movement enum structure used to represent step value
        public enum MoveMent
        {
            MoveForward,
            MoveBackward,
            TurnLeft,
            TurnRight,
            Break,
            LightBreak,
            SlamBreak,
            HandBreak,
            Wait
        }

        /***********MainDecisionMaker*********/
        

        /***********Memories******************/
        // Predicted next movement record list
        private ArrayList RecordSteps = new ArrayList();
        // Next step with highest confidence
        private MoveMent nextMove = MoveMent.Wait;

        /***********Hyper-Param****************/
        protected int DetectiveLayer;          // The target layer where obstacles located
        protected float RayMaxDistance;        // The maximum distance that radar can ditected
        protected int SideVisualAngle;         // The range of left/right radar could reach(If equipped)
        protected float StraightBias = 7.1f;   // Normally the distance estimated ahead is longer 
        protected bool isForwardBlocked = false;// True if forward is blocked by pbstacle
        protected int StepSize = 10;           // The maximum steps can generate 

        /***********Methods Interface************/
        public abstract void StrightMovementDecisionMaker(float _speed, float _dist);
        // TODO: optimize this method
        public abstract void TurningDecisionMaker(float _speed, float _leftDis, float _rightDis, bool _isForwardblocked);
        public abstract void PrintMessage();
        public void SetStepSize(int _newSize)
        {
            StepSize = _newSize;
        }

        /***********Value Get/Set Methods*********/
        // Return current estimated next action
        protected MoveMent GetNextMove()
        {
            return nextMove;
        }

        // Add a new action to the record list
        protected void AddNewRecord(MoveMent _step)
        {
            if (GetLengthOfRecord() > 2 && nextMove==MoveMent.MoveForward)
            {
                RefreshRecord();
                return;
            }

            RecordSteps.Add(_step);
        }

        // Delete all current saved actions in the record list
        protected void RefreshRecord()
        {
            if (GetLengthOfRecord() == 0)
            {
                return;
            }

            RecordSteps.RemoveRange(0, GetLengthOfRecord());
        }

        // Return how many actions estimated saved in the list
        // (Actions have not been executed) 
        public int GetLengthOfRecord()
        {
            return RecordSteps.Count;
        }

        // Pop out the first element in the record list
        public MoveMent PopNextMove()
        {
            // Halt waiting
            if (GetLengthOfRecord() == 0)
            {
                return MoveMent.Wait;
            }

            nextMove = (MoveMent)RecordSteps[0];
            RecordSteps.RemoveAt(0);

            return nextMove;
        }

        /*********Default pattern mathods****************/
        protected MoveMent BreakPattenSimulation(Situation _state)
        {
            switch (_state)
            {
                case Situation.Safe:
                    return MoveMent.MoveForward;
                case Situation.Unstable:
                    return MoveMent.LightBreak;
                case Situation.Dangerous:
                    return MoveMent.SlamBreak;
                default:
                    return MoveMent.Wait;
            }
        }

        /// <summary>
        /// Check whether one side of vehicle do not has obstacle and can move
        /// Side: left / right
        /// </summary>
        /// <returns>Ture if there is a path lead to no obstacle</returns>
        public static bool IsOneSideNotBlocked(float leftDistance, float rightDistance)
        {
            if (leftDistance < 0 || rightDistance < 0)
            {
                return true;
            }

            if (!StepController.IsTwoFloatValueSimilar(leftDistance, rightDistance, 0.3f))
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// Random choose direction to turn
        /// Add action into list
        /// Prefer to turn left
        /// </summary>
        /// <param name="_bias">Using this value to control turning direction</param>
        public void RandomTurning(int _bias=0)
        {

            var choice = UnityEngine.Random.Range(0, 100);

            if (choice+ _bias < 60)
            {
                AddNewRecord(MoveMent.TurnLeft);
                return;
            }

            AddNewRecord(MoveMent.TurnRight);
        }
    }
}
