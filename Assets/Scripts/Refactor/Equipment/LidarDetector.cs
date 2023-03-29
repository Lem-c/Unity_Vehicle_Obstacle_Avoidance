using ActionManager;
using System;
using UnityEngine;

namespace VehicleEqipment.Lidar
{
    public class LidarDetector : Detector
    {
        protected bool isTurning = false;      // Whether next step is turning

        public LidarDetector(int _layer, float _rayDistance, float _sideVisualAngle = 0)
        {
            SetParameters(_layer, _rayDistance, _sideVisualAngle);
        }

        /// <summary>
        /// Overload method that check whether there is an object 
        /// On the route of the ray
        /// </summary>
        /// <param name="_target">The target that equipped with the ray </param>
        /// <returns>Bool value whther detected obstacle</returns>
        public bool RayDetection(Transform _target)
        {
            if (Physics.Raycast(_target.position,
                _target.TransformDirection(Vector3.forward),
                out hit,
                MaxRayDistance, LayerMask))
            {
                // DrawRay(_target.position, _target.TransformDirection(Vector3.forward), Color.red);
                return true;
            }
            isTurning= false;
            return false;
        }

        /// <summary>
        /// The main method that this class has.
        /// Can be used to detect the obstacles that lays on a rang (sector)
        /// TODO: The complexity requires improve./ Interface add
        /// </summary>
        /// <param name="_side">The bias added(turn left or right)</param>
        /// <param name="_target">The tartget object that equipped with ray detector</param>
        /// <param name="_checkTime">How many times used to finish one detection</param>
        /// <returns>Whether deteced the obstacles</returns>
        public bool RangRayDetection(Transform _target, int _side, int _checkTime = 5)
        {
            if ((_side != -1 && _side != 1) || angleBias_y <= 0)
            {
                throw new ArgumentException("The direction choice or turning bias value is incorrect!");
            }

            // The rotation temporary variables
            float currentBias = 10f;
            float tempBias = 0;

            // The init hit bool
            bool isDetected = false;

            Vector3 tempDirection;

            int activateTimes = 0;

            while (!isDetected && activateTimes < _checkTime)
            {
                // dynamice change the detected direction
                if (currentBias < angleBias_y + currentBias)
                {
                    currentBias += (angleBias_y / 5);
                    tempBias = _side * currentBias;
                }

                // Add 'tempBias' to the y-axis
                tempDirection = Quaternion.Euler(0, tempBias, 0) *
                                _target.TransformDirection(Vector3.forward);

                isDetected = RayDetection(_target.position, tempDirection);

                DrawRay(_target.position, tempDirection, Color.green);

                activateTimes += 1;
            }
            isTurning = isDetected;
            return isDetected;
        }

        /// <summary>
        /// Focus the dection range of lidar to a small angle
        /// </summary>
        /// <param name="_minus">The value decreased of angel</param>
        public void ShrinkAngle(int _minus)
        {
            if (angleBias_y - _minus <= 0)
            {
                return;
            }

            SetYBiasAngle(angleBias_y - _minus);
        }

        /***********Value modify methods*************/
        protected void SetYBiasAngle(float _new)
        {
            angleBias_y = _new;
        }

        public void RecoverAngle()
        {
            angleBias_y = tempAngle;
        }

        /***********value get methods***************/
        public float GetYBiasAngle()
        {
            return angleBias_y;
        }

        public bool GetIsTurning()
        {
            return isTurning;
        }
    }
}