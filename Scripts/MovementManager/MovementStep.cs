using System.Collections;
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
        DecisionMaker decMaker;                // Have to set an decision maker

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
        protected bool isTurning = false;      // Whether next step is turning
        protected bool isForwardBlocked = false;// True if forward is blocked by pbstacle

        /***********Methods Interface************/
        public abstract void StrightMovementDecisionMaker(float _speed, float _dist);
        public abstract void TurningDecisionMaker(float _leftDis, float _rightDis, bool _isForwardblocked);

        /***********Value Get/Set Methods*********/
        // Return current estimated next action
        protected MoveMent GetNextMove()
        {
            return nextMove;
        }

        // Add a new action to the record list
        protected void AddNewRecord(MoveMent _step)
        {
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
        /// check whether one side of vehicle do not has obstacle and can move
        /// </summary>
        /// <returns>Ture if there is a path lead to no obstacle</returns>
        public static bool IsOneSideNotBlocked(float leftDistance, float rightDistance)
        {
            if (leftDistance < 0 || rightDistance < 0)
            {
                return true;
            }

            if (SmoothMovement.IsTwoFloatValueSimilar(leftDistance, rightDistance, 1.3f))
            {
                return true;
            }


            return false;
        }
    }
}
