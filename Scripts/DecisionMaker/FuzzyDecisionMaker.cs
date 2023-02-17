using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace DecisionMake
{
    public class FuzzyDecisionMaker : DecisionMaker
    {
        struct SpeedCondition
        {
            public Speed speed;
            public float probability;
        }

        struct DistanceCondition
        {
            public Distance distance;
            public float probability;
        }

        private SpeedCondition currentSpeed;
        private DistanceCondition currentCondition;

        // Default parameters
        public float speedUpperBound;
        public float distanceUpperBound;

        public FuzzyDecisionMaker(float _maxSpeed, float _maxDistance)
        {
            speedUpperBound = _maxSpeed;
            distanceUpperBound = _maxDistance;
        }

        /// <summary>
        /// Main method for this class
        /// Plz call 'SetWieghtList(...)' method to set degree weight in advance
        /// </summary>
        /// <param name="_currentSpeed">current vehicle speed</param>
        /// <param name="_currentDis">current distance to the obstacle(if has)</param>
        /// <returns>Situation determined</returns>
        public Situation GetFuzzyResult(float _currentSpeed, float _currentDis, float _threshlod=1)
        {
            // Check whether too far to the obstacle
            // TODO: Can't avoid crush
            if(_currentDis <= 0 || _currentDis > 2*distanceUpperBound)
            {
                return Situation.Safe;
            }

            // Get degree fucntion and apply fuzzyfication
            Fuzzyfication(_currentSpeed, _currentDis);
            // Calculate average weight sum and get Situation
            var state = ActivateFunction( GetWeightedAverage(), _threshlod);

            return state;
        }

        public void SetWieghtList(float[] _weight)
        {
            if (_weight.Length < 1)
            {
                return;
            }

            degreeWeight = _weight;
        }

        /************Private methods********************/
        private Speed DetermineSpeed(float _speed)
        {
            float misJudge = UnityEngine.Random.Range(0f, 0.1f) * UnityEngine.Random.Range(-1,2);

            if (_speed + misJudge <= 0.3f * speedUpperBound)
            {
                return Speed.Slow;
            }

            if(_speed + misJudge >= 0.7f * speedUpperBound)
            {
                return Speed.Fast;
            }

            return Speed.Steady;
        }

        private Distance DeterminDistance(float _dis)
        {
            if(_dis < 0)
            {
                return Distance.Far;
            }

            float misJudge = UnityEngine.Random.Range(0f, 0.2f) * UnityEngine.Random.Range(-1, 2);

            if (_dis + misJudge <= 0.25f * distanceUpperBound)
            {
                return Distance.Close;
            }

            if (_dis + misJudge >= 0.75f * distanceUpperBound)
            {
                return Distance.Far;
            }

            return Distance.Mid;
        }

        private float SpeedRation(float _speed)
        {
            float misJudge = UnityEngine.Random.Range(0f,0.5f) * UnityEngine.Random.Range(-1, 2);

            if(_speed < 0)
            {
                return 0;
            }

            if(_speed + misJudge < 0.3f * speedUpperBound)
            {

                return (1f - _speed / (0.3f*speedUpperBound));
            }
            else if(_speed + misJudge > 0.7f * speedUpperBound)
            {
                return _speed / speedUpperBound;
            }
            else
            {
                return _speed / (0.7f * speedUpperBound);
            }
        }

        private float DistRatio(float _dist)
        {
            float misJudge = UnityEngine.Random.Range(0f, 1.5f) * UnityEngine.Random.Range(-1, 2);

            // Debug.Log(currentCondition.distance + ", " + _dist);

            if (_dist < 0)
            {
                return 10*distanceUpperBound;
            }

            if (_dist + misJudge < 0.25f * distanceUpperBound)
            {

                return (1f - _dist / (0.25f * distanceUpperBound));
            }
            else if (_dist + misJudge > 0.75f * distanceUpperBound)
            {
                return _dist / distanceUpperBound;
            }
            else
            {
                return _dist / (0.75f * distanceUpperBound);
            }
        }

        private int CompareProbability(ArrayList _one, ArrayList _two, int type=0)
        {
            if (type == 0)
            {
                DistanceCondition tempDist_1 = (DistanceCondition)_two[0];
                DistanceCondition tempDist_2 = (DistanceCondition)_two[1];

                return SelectValue(tempDist_1.probability, tempDist_2.probability);
            }

            SpeedCondition tempSpeed_1 = (SpeedCondition)_one[0];
            SpeedCondition tempSpeed_2= (SpeedCondition)_one[1];

            return SelectValue(tempSpeed_1.probability, tempSpeed_2.probability);

        }

        /// <summary>
        /// Part of fuzzyfication
        /// Generate two temp selected degree and assign them to ArrayList
        /// The situation is judged by two methods:
        /// one for state:{speed, distance}, another for probability
        /// </summary>
        /// <param name="_speed">current speed</param>
        /// <param name="_dis">current distance to</param>
        /// <returns>
        /// A list contains two ArrayList:
        /// SpeedCondition and DistanceCondition
        /// </returns>
        private ArrayList[] GenerateFuzzyDegree(float _speed, float _dis)
        {
            ArrayList tempSpeedList = new ArrayList();
            ArrayList tempDisList = new ArrayList();

            
            while(tempSpeedList.Count < 2)
            {
                var tempSpeed = new SpeedCondition();
                tempSpeed.speed = DetermineSpeed(_speed);
                tempSpeed.probability = SpeedRation(_speed);

                tempSpeedList.Add(tempSpeed);
            }
            while (tempDisList.Count < 2)
            {
                var tempDist = new DistanceCondition();
                tempDist.distance = DeterminDistance(_dis);
                tempDist.probability = DistRatio(_dis);

                tempDisList.Add(tempDist);
            }

            ArrayList[] result= {tempSpeedList , tempDisList};
            return result;
        }

        /// <summary>
        /// After get the degree of membership
        /// apply fuzzification to get probability for each state
        /// </summary>
        /// <param name="_currentSpeed">current speed</param>
        /// <param name="_currentDis">curren distance to the obstacle</param>
        /// <exception cref="Exception">Method 'GenerateFuzzyDegree()' failed</exception>
        private void Fuzzyfication(float _currentSpeed, float _currentDis)
        {
            ArrayList[] dataList = GenerateFuzzyDegree(_currentSpeed, _currentDis);
            if (dataList is null || dataList[0].Count == 0 || dataList[1].Count == 0)
            {
                throw new Exception("Error occured in generating fuzzy degree");
            }

            currentSpeed = (SpeedCondition)dataList[0][CompareProbability(dataList[0], dataList[1], 1)];
            currentCondition = (DistanceCondition)dataList[1][CompareProbability(dataList[0], dataList[1], 0)];
        }

        /// <summary>
        /// Calculate Weighted average of all degree
        /// The _val array should be distributed already
        /// </summary>
        /// <param name="_weight"></param>
        /// <exception cref="Exception"></exception>
        private float GetWeightedAverage()
        {
            if (currentSpeed.Equals(null) || currentCondition.Equals(null))
            {
                throw new Exception("Fuzzification failed!");
            }

            if (degreeWeight.Length < 6)
            {
                throw new Exception("Weight is not initialized! | Part");
            }

            int speedWeightIndex = (int)currentSpeed.speed - 1;
            int distWeightIndex = (int)currentCondition.distance - 1;

            var fireStrength = currentSpeed.probability * degreeWeight[speedWeightIndex];
            fireStrength += currentCondition.probability * degreeWeight[distWeightIndex];
            
            var strengthSum = currentSpeed.probability + currentCondition.probability;

            // Debug.Log(currentSpeed.probability + ", " + currentCondition.probability);
            Debug.Log(fireStrength / strengthSum);

            return fireStrength / strengthSum;
        }
    }
}
