using System;

namespace ActionManager
{
    /// <summary>
    /// The movement controller with factory move pattern generator
    /// Gudie the vehicle mevement
    /// Inherit from : MovementStep (step manager)
    /// </summary>
    public class StepController : MovementStep
    {
        /// <summary>
        /// Decision Space bias
        /// </summary>
        protected readonly float MaxDecisionBias = 0.05f;

        protected StepController(float _bias = 0.05f)
        {
            MaxDecisionBias = _bias;
            // Can not created object by this method
            // TODO: oop optimization
        }
        /************Main Methods****************/

        /// <summary>
        /// Determine movement style in the 'z' direction
        /// Using Fuzzy/DWA logic to weight judge from: speed + distance
        /// </summary>
        /// <param name="_speed">current speed of tager</param>
        /// <param name="_dist">distance from target to the obstacles</param>
        public override void StrightMovementDecisionMaker(float _speed, float _dist)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Turning decision maker
        /// </summary>
        /// <param name="_speed">current speed of tager</param>
        /// <param name="_leftDis">left distance from target to the obstacles</param>
        /// <param name="_rightDis">right distance from target to the obstacles</param>
        /// <param name="_isForwardblocked">whether front is blocked</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void TurningDecisionMaker(float _speed, float _leftDis, float _rightDis, bool _isForwardblocked)
        {
            throw new NotImplementedException();
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

        /*************** Factory ***************************/
        public static StepController GenerateStepManager(string _type, float _maxSpeed, float _MaxRayDistance,
                                                         float _startAngle=0, float[] _weight=null, float _MaxDecisionBias=0.05f)
        {
            switch  (_type)
            {
                case "Smooth":
                    return new SmoothMove(_maxSpeed, _MaxRayDistance, _MaxDecisionBias);
                case "DWA":
                    return new DWAMove(_maxSpeed, _startAngle, _weight, _MaxDecisionBias);
                default:
                    return null;

            }
        }

        /********************Debug methods****************************/
        public override void PrintMessage()
        {
            UnityEngine.Debug.Log(GetNextMove());
        }
    }
}
