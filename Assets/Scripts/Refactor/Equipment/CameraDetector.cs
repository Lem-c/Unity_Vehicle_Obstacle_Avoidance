using UnityEngine;

namespace VehicleEqipment.Camera
{
    public class CameraDetector : Detector
    {
        float newDistance=1000f;
        // TODO: Keep record original start point


        public CameraDetector()
        {
            //SetParameters(_layer, _rayDistance, _sideVisualAngle);
        }

        public Vector3 GetVectorFromTwoPoint(float[] _from, float[] _to)
        {
            return new Vector3(_from[0] - _to[0], 0, _from[1] - _to[1]);
        }

        /// <summary>
        /// Check whether the angle from vehicle to the destination is decreasing
        /// Used to determine whether moving close to the target point
        /// </summary>
        /// <param name="_vehicle">vehilce vecotr.Forward</param>
        /// <param name="_destination">Calculating by minus vehilce point and destination point</param>
        /// <returns></returns>
        public bool IsAngleShrink(Vector3 _vehicle, Vector3 _destination, float _orginalAngle)
        {
            if(_orginalAngle < CalculateAngle(_vehicle, _destination))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Check whether two points are closing to each other
        /// </summary>
        /// <param name="_vehicle">vehilce vecotr.Forward</param>
        /// <param name="_destination">detination vector</param>
        /// <returns></returns>
        public bool IsClosingTo(Vector3 _vehicle, Vector3 _destination)
        {
            var tempDistance = newDistance;
            newDistance = Vector3.Distance(_vehicle, _destination);

            if(newDistance <= tempDistance)
            {
                return true;
            }

            return false;
        }


        private float CalculateAngle(float[] _from, float[] _to)
        {
            if(_from.Length != _to.Length || _to.Length != 2)
            {
                Debug.LogWarning("Vector value error");
                return 0;
            }

            float radian = Mathf.Acos((_from[0]*_to[0] + _from[1] * _to[1])
                  / (Mathf.Sqrt(_from[0] * _from[0]) + (_from[1] * _from[1])
                          * Mathf.Sqrt(_to[0] * _to[0]) + (_to[1] * _to[1])));

            return radian / Mathf.PI * 180;
        }

        /// <summary>
        /// Calculate angle off according to the Vectors
        /// </summary>
        /// <param name="_from"></param>
        /// <param name="_to"></param>
        /// <returns></returns>
        private float CalculateAngle(Vector3 _from, Vector3 _to)
        {
            float[] fromPos = { _from.x, _from.z };
            float[] toPos = { _to.x, _to.z };

            return CalculateAngle(fromPos, toPos);
        }
    }
}
