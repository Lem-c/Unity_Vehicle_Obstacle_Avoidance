using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace VehicleEqipment.Camera
{
    public class CameraDetector : Detector
    {
        // Recorder
        float lastAngle = 360f;
        float bestClosestDis = 1000f;
        float recoverCount = 20;

        public static GameObject Target;                  // The object where camera would be installed
        public static Vector3 destination;                         // The target destination

        // List recording all distances measured to obstalces
        private List<List<float>> obsDistanceMap;


        public CameraDetector(GameObject target,
                              int _layer, float _rayDistance, 
                              float _sideVisualAngle = 0)
        {
            Target = target;

            if (GameObject.FindWithTag("MousePoint") == null) {
                Debug.LogWarning("Fail to find cube destination, set TAG first");
            }
            else
            {
                destination = GameObject.FindWithTag("MousePoint").transform.position;
            }

            obsDistanceMap = new List<List<float>>();

            if (Target == null || Target.IsUnityNull())
            {
                throw new UnityException("Null object reference:Target");
            }

            SetParameters(_layer, _rayDistance, _sideVisualAngle);
        }

        /// <summary>
        /// Main method camera processed
        /// Interval execution
        /// Assign value to '_out' whether moving to the target
        /// </summary>
        /// <param name="_cam">Mouse input catch camera</param>
        /// <param name="_angleWeight">How much relys on the shrink of angle</param>
        /// <param name="_disWeight">How much relys on the distance decrease</param>
        /// <param name="_out">External value assignment: whether moving to the target</param>
        public void ProcessCamera(UnityEngine.Camera _cam, float _angleWeight, float _disWeight, ref bool _out)
        {
            _out = IsMovingClose2Target(_angleWeight, _disWeight);
            // Update data and record
            LerpUpdate(_cam);
        }

        /*************** Internal methods *****************/
        private void AddDistance2ObsIntoList()
        {
            int leftDetection = UnityEngine.Random.Range(5,8);
            int rightDetection = UnityEngine.Random.Range(5,8);

            RangRayDetection(Target.GetComponent<Transform>(), -1, leftDetection);
            RangRayDetection(Target.GetComponent<Transform>(), 1, rightDetection);
        }

        public void CleanDataMap()
        {
            obsDistanceMap.Clear();
        }

        public int GetLengthOfDataset()
        {
            return obsDistanceMap.Count;
        }

        /// <summary>
        /// Get the angle and distance facing to obstacle
        /// </summary>
        /// <param name="_index"></param>
        /// <returns>{angle, distance2obs}</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public List<float> GetIndexOfDataMap(int _index)
        {
            if(_index > GetLengthOfDataset())
            {
                throw new IndexOutOfRangeException("The required index is out of map range");
            }

            List<float> result = obsDistanceMap[_index];

            return result;
        }

        /// <summary>
        /// Check whether car is moving towards to the destination
        /// </summary>
        /// <param name="_angleWeight">How much relys on the shrink of angle</param>
        /// <param name="_disWeight">How much relys on the distance decrease</param>
        /// <returns>bool value</returns>
        public bool IsMovingClose2Target(float _angleWeight, float _disWeight)
        {
            //TODO: this seems not work
            var angle = IsAngleShrink(Target.GetComponent<Transform>().position, destination,
                                      lastAngle);
            var dis = IsClosingTo(Target.GetComponent<Transform>().position, destination);

            float result = DWAMove.Discriminator(angle) * _angleWeight;
            result += DWAMove.Discriminator(dis) * _disWeight;

            // Debug.Log("Angle: " + DWAMove.Discriminator(angle) + " Dis: " + DWAMove.Discriminator(dis));
            // Debug.Log(DWAMove.ActiveFunction(result));

            return DWAMove.ActiveFunction(result);
        }
        public bool IsMovingClose2Target(float _angleWeight, float _disWeight,
                                         float _lashAngle, float _lastDis)
        {
            var angle = IsAngleShrink(Target.GetComponent<Transform>().position, destination,
                                      _lashAngle);
            var dis = IsClosingTo(Target.GetComponent<Transform>().position, destination);

            float result = DWAMove.Discriminator(angle) * _angleWeight;
            result += DWAMove.Discriminator(dis) * _disWeight;

            return DWAMove.ActiveFunction(result);
        }


        /// <summary>
        /// Update shrinked angle and detination point vector
        /// Should run before:IsMovingClose2Target(...)
        /// </summary>
        /// <param name="_cam">Map catch camera</param>
        public void LerpUpdate(UnityEngine.Camera _cam)
        {
            lastAngle = CalculateAngle(Target.GetComponent<Transform>().position, destination);

            UpdateDestination(_cam);
            AddDistance2ObsIntoList();
        }

        /*******************************Sensor detected methods****************************************/
        /// <summary>
        /// Check whether the angle from vehicle to the destination is decreasing
        /// Used to determine whether moving close to the target point
        /// </summary>
        /// <param name="_vehicle">vehilce vecotr.Forward</param>
        /// <param name="_destination">Calculating by minus vehilce point and destination point</param>
        /// <returns></returns>
        public bool IsAngleShrink(Vector3 _vehicle, Vector3 _destination, float _lstAngle)
        {
            // Debug.Log("Last: " + _lstAngle + "Current: " + CalculateAngle(_vehicle, _destination));

            var val = CalculateAngle(_vehicle, _destination) - _lstAngle;

            if (val < 0)
            {
                return true;
            }else if(val == 0) {
                return DWAMove.ActiveFunction(UnityEngine.Random.Range(-1f, 1f));
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
            // Debug.Log(Vector3.Distance(_vehicle, _destination) + ", " + _lastDis);

            var val = Vector3.Distance(_vehicle, _destination);

            if ( val <= bestClosestDis)
            {
                bestClosestDis = val;
                return true;
            }

            // Re-init
            if( recoverCount<=0){
                bestClosestDis = 1000f;
                recoverCount = 10;
            }

            recoverCount--;
            return false;
        }

        public float GetAngleBetween()
        {
            return CalculateAngle(Target.GetComponent<Transform>().position, destination);
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
        private void RangRayDetection(Transform _target, int _side, int _checkTime = 5)
        {
            if ((_side != -1 && _side != 1) || angleBias_y <= 0)
            {
                throw new ArgumentException("The direction choice or turning bias value is incorrect!");
            }

            // The rotation temporary variables
            float currentBias = UnityEngine.Random.Range(3f, 22f);
            float tempBias = 0;

            Vector3 tempDirection;

            int activateTimes = 0;

            while (activateTimes < _checkTime)
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

                // Detection result
                RayDetection(_target.position, tempDirection);

                // Add distacen into the lists
                obsDistanceMap.Add(new List<float> {
                    tempBias,
                    DistanceTo()
                });

                DrawRay(_target.position, tempDirection, UnityEngine.Color.blue);

                activateTimes += 1;
            }
        }

        /***************************** Assist method ******************************/
        /// <summary>
        /// Using a target camera to get the position of mouse in the SceneWorld
        /// When mouse clicked a new point
        /// Update the destination
        /// </summary>
        /// <param name="_cam">The target camera</param>
        private void UpdateDestination(UnityEngine.Camera _cam)
        {
            if (!Input.GetMouseButtonUp(0)) { return; }

            // TODO: Add mouse click cool down

            // Mouse input check
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            bool isCollider = Physics.Raycast(ray, out RaycastHit hit);
            if (isCollider)
            {
                // Update destination
                Debug.Log("Destinaton updated");
                // Reset recorder
                lastAngle = 360f;
                bestClosestDis = 1000f;

                destination = hit.point;

                if(GameObject.FindWithTag("MousePoint") != null)
                {
                    var cube = GameObject.FindWithTag("MousePoint");
                    cube.transform.position = destination;
                }
            }
        }

        /************************ Camera Special methods *****************************/
        private static float CalculateAngle(float[] _from, float[] _to)
        {
            if(_from.Length != _to.Length || _to.Length != 2)
            {
                Debug.LogWarning("Vector value error");
                return 0;
            }

            float radian = Mathf.Acos((_from[0]*_to[0] + _from[1] * _to[1])
                  / (Mathf.Sqrt(_from[0] * _from[0]) + (_from[1] * _from[1])
                          * Mathf.Sqrt(_to[0] * _to[0]) + (_to[1] * _to[1])));

            return Math.Abs(radian / Mathf.PI * 180 - 180);
        }

        /// <summary>
        /// Calculate angle off according to the Vectors
        /// </summary>
        /// <param name="_from"></param>
        /// <param name="_to"></param>
        /// <returns></returns>
        public static float CalculateAngle(Vector3 _from, Vector3 _to)
        {
            float[] fromPos = { _from.x, _from.z };
            float[] toPos = { _to.x, _to.z };

            return CalculateAngle(fromPos, toPos);
        }

        /// <summary>
        /// Return a vector3 object accoridng to a two float array
        /// However, y is always zero
        /// </summary>
        /// <param name="_from">{x,y=0,z}</param>
        /// <param name="_to">{x,y=0,z}</param>
        /// <returns></returns>
        public static Vector3 GetVectorFromTwoPoint(float[] _from, float[] _to)
        {
            return new Vector3(_from[0] - _to[0], 0, _from[1] - _to[1]);
        }
    }
}
