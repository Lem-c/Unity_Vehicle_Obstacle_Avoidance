using DecisionMake;
using System;
using UnityEngine;
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
            AddNewRecord(BreakPattenSimulation(state));
        }

        public override void TurningDecisionMaker(float _speed, float _leftDis, float _rightDis, bool _isForwardblocked)
        {
            /*Fuzzy active threshold is matter, Consider this when meeting problems*/
            Situation leftState = fdm.GetFuzzyResult(_speed, _leftDis, 2.4f + MaxDecisionBias * UnityEngine.Random.Range(-1, 2));
            Situation rightState = fdm.GetFuzzyResult(_speed, _rightDis, 2.4f + MaxDecisionBias * UnityEngine.Random.Range(-1, 2));

            // Save two side situation into array | No safe check
            Situation[] stateList = { rightState, leftState };
            // Debug.Log(stateList[0] + ", " + stateList[1]);

            SideSituationJudgement(stateList, _leftDis, _rightDis, _isForwardblocked);
        }

        /***********************Assit Methods**********************/
        /// <summary>   
        /// Intialize the fuzzy object
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void InitFuzzyChip(float _maxSpeed, float _MaxRayDistance)
        {
            fdm = new FuzzyDecisionMaker(_maxSpeed, _MaxRayDistance);

            float[] _tempWieght = { 0.5f, 2.25f, 5.35f, 2.5f, 5f, 10f };
            fdm.SetWieghtList(_tempWieght);
        }

        /// <summary>
        /// Load existed fuzzy class from external
        /// </summary>
        /// <param name="_fdm">The special fuzzy class</param>
        /// <exception cref="ArgumentException">Load null fuzzy module</exception>
        public void LoadFuzzyChip(FuzzyDecisionMaker _fdm)
        {
            fdm = _fdm;

            if (fdm == null)
            {
                throw new ArgumentException("Null Decision Maker");
            }
        }

        /// <summary>
        /// Determine current safty level accroding to the distance to left/right
        /// And make judgement from states array
        /// </summary>
        /// <param name="_states">
        /// Array saves states, contain two elements.
        /// First one is right, and another one is left side.
        /// </param>
        /// <param name="_left">Distance to the left side obstacle</param>
        /// <param name="_right">Distance to the right side obstacle</param>
        /// <param name="_isForawadBlocked">Bool velue</param>
        private void SideSituationJudgement(Situation[] _states, float _left, float _right, bool _isForawadBlocked)
        {
            // Check whether no obstacles or two sides hvae been blocked
            // Avoid to make mis-judgement
            var canTurning = IsOneSideNotBlocked(_left, _right);

            if ( (_left < 0 && _right < 0))
            {
                if (_isForawadBlocked && canTurning)
                {
                    RandomTurning(-10);
                    return;
                }

                AddNewRecord(MoveMent.MoveForward);
                return;
            }

            // Direction Determine
            if( (_left<0 && _right>0) || 
                (canTurning && (_left > _right) && _right > 0))
            {
                if (_states[0] != Situation.Dangerous) return;

                // Right lidar found obstacle
                AddNewRecord(MoveMent.TurnLeft);
                // AddNewRecord(MoveMent.TurnLeft);

                return;
            }

            if ((_right < 0 && _left > 0) ||
                (canTurning && (_left < _right) && _left > 0))
            {
                if (_states[1] != Situation.Dangerous) return;

                // Left lidar found obstacle
                AddNewRecord(MoveMent.TurnRight);
                // AddNewRecord(MoveMent.TurnRight);

                return;
            }
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

            float _bias = 0.01f;
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
