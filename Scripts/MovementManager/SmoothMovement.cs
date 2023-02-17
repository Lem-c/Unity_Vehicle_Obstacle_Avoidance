using DecisionMake;
using System;
using static DecisionMake.DecisionMaker;

namespace ActionManager
{
    /// <summary>
    /// The movement controller with smooth move pattern
    /// Try to move quckly and steady
    /// Inherit from : MovementStep (step manager)
    /// </summary>
    public class SmoothMovement : MovementStep
    {
        // The fuzzy logic model
        private FuzzyDecisionMaker fdm;

        // Decision Space
        private readonly float MaxDecisionBias = 0.05f;

        public SmoothMovement(float _maxSpeed, float _MaxRayDistance)
        {
            InitFuzzyChip(_maxSpeed, _MaxRayDistance);
        }
        /************Main Methods****************/

        /// <summary>
        /// Determine movement style in the 'z' direction
        /// Using Fuzzy logic to weight judge from: speed + distance
        /// </summary>
        /// <param name="_speed">current speed of tager</param>
        /// <param name="_dist">distance from target to the obstacles</param>
        public override void StrightMovementDecisionMaker(float _speed, float _dist)
        {
            // Determine current situation according to the speed and distance to the obs
            Situation state = fdm.GetFuzzyResult(_speed, _dist);

            // Add new action to the action-list
            AddNewRecord( BreakPattenSimulation(state) );
        }

        public override void TurningDecisionMaker(float _leftDis, float _rightDis, bool _isForwardblocked)
        {
            SideSituationJudgement(_leftDis, _rightDis, _isForwardblocked);
        }

        /***********************Assit Methods**********************/
        /// <summary>
        /// Intialize the fuzzy object
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void InitFuzzyChip(float _maxSpeed, float _MaxRayDistance)
        {
            fdm = new FuzzyDecisionMaker(_maxSpeed, _MaxRayDistance);

            float[] _tempWieght = { 0.5f, 3.25f, 8.35f, 2.5f, 5f, 10f };
            fdm.SetWieghtList(_tempWieght);
        }
        public void LoadFuzzyChip(FuzzyDecisionMaker _fdm)
        {
            fdm = _fdm;

            if (fdm == null)
            {
                throw new ArgumentException("Null Decision Maker");
            }
        }

        private void SideSituationJudgement(float _left, float _right, bool _isForawadBlocked)
        {
            if(_left < 0 && _right < 0)
            {
                if (_isForawadBlocked)
                {
                    RandomTurning();
                    return;
                }
            }

            isTurning = true;           // Going to turn
            // Check whether two distances are similar
            // Avoid to make mis-judgement
            var isDisSimilar = IsTwoFloatValueSimilar(_left, _right, MaxDecisionBias);

            // Direction Determine
            if( (_left<0 && _right>0) || 
                (!isDisSimilar && (_left > _right) && _right > 0))
            {
                // Right lidar found obstacle
                var turn_action = MoveMent.TurnLeft;
                AddNewRecord(turn_action);
                AddNewRecord(MoveMent.MoveForward);

                return;
            }

            if ((_right < 0 && _left > 0) ||
                (!isDisSimilar && (_left < _right) && _left > 0))
            {
                // Left lidar found obstacle
                var turn_action = MoveMent.TurnRight;
                AddNewRecord(turn_action);
                AddNewRecord(MoveMent.MoveForward);

                return;
            }

            isTurning = false;
        }

        /// <summary>
        /// Random choose direction to turn
        /// Add action into list
        /// Prefer to turn left
        /// </summary>
        private void RandomTurning()
        {
            isTurning = true;

            var choice = UnityEngine.Random.Range(0, 100);

            if (choice < 60)
            {
                AddNewRecord(MoveMent.TurnLeft);
                return;
            }

            AddNewRecord(MoveMent.TurnRight);
        }

        /// <summary>
        /// Determine the magnitude whether a and b is similar
        /// </summary>
        /// <param name="_a">first float num</param>
        /// <param name="_b">second float num</param>
        /// <param name="_range">similar gap</param>
        /// <returns></returns>
        public static bool IsTwoFloatValueSimilar(float _a, float _b, float _range)
        {
            if (_a < 0 || _b < 0) { return false; }

            float _bias = 0.1f;
            if (_a - _b < _range + _bias)
            {
                return true;
            }

            return false;
        }

        /********************Debug methods****************************/
        public void PrintCurrentSituation()
        {
            UnityEngine.Debug.Log(GetNextMove());
        }
    }
}
