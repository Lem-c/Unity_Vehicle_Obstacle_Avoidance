using System.Collections;
using UnityEngine;


namespace DecisionMake
{
    /// <summary>
    /// The abstact class of DecisionMaker
    /// </summary>
    public abstract class DecisionMaker
    {
        public enum Speed
        {
            Slow = 1,
            Steady = 2,
            Fast = 3
        }

        public enum Distance
        {
            Close = 4,
            Mid = 5,
            Far = 6
        }

        public enum Situation
        {
            Safe,
            Unstable,
            Dangerous
        }

        protected float[] degreeWeight;           // Each weigh for 

        /// <summary>
        /// Threshold used to determine whether activate the state
        /// </summary>
        /// <param name="_val">value would be checked</param>
        /// <param name="_threshold">a float number</param>
        /// <returns></returns>
        protected Situation ActivateFunction(float _val, float _threshold = 1, float _bias=0.2f)
        {
            if (_val < _threshold+_bias) return Situation.Dangerous;

            if (_val > _threshold*2) return Situation.Safe;

            return Situation.Unstable;
        }

        protected int SelectValue(float _num1, float _num2, int type = 1)
        {

            if (type <= 0)
            {
                return MinimunRule(_num1, _num2);
            }

            return MaximumRule(_num1, _num2);
        }

        /*************Private methods******************/
        private int MinimunRule(float _first, float _second)
        {
            if (_first < 0 && _second < 0) return -1;

            if(_first > _second) return 1;

            return 0;
        }

        private int MaximumRule(float _first, float _second)
        {
            if (_first < 0 && _second < 0) return 0;

            if (_first < _second) return 1;

            return 0;
        }
    }
}
