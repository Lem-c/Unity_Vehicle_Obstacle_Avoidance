using Unity.VisualScripting;
using UnityEngine;


namespace VehicleEqipment
{
    public class Detector
    {
        protected RaycastHit hit;     // The raycast save the ray first hit
        // Setting params used to tuning functions
        protected int LayerMask { set; get; }
        protected float MaxRayDistance { set; get; }
        // Changing variables
        protected float angleBias_y { set; get; }  // The rotation on z-axis [Have to greater than zero, rotation angle]
        protected float tempAngle { set; get; }   // Save radar rotation angle


        /// <summary>
        /// Set previous parameters
        /// </summary>
        /// <param name="_layer">Detection layer</param>
        /// <param name="_rayDistance">Max distance of ray</param>
        /// <param name="_sideVisualAngle">Ray rotarion angle</param>
        public void SetParameters(int _layer, float _rayDistance, float _sideVisualAngle = 0)
        {
            LayerMask = _layer;
            MaxRayDistance = _rayDistance;
            angleBias_y = _sideVisualAngle;

            tempAngle = angleBias_y;
        }

        /// <summary>
        /// The raw method take two vectors and apply ray check
        /// </summary>
        /// <param name="_from">The target vector</param>
        /// <param name="_direction">Where the vector would continue</param>
        /// <returns></returns>
        protected bool RayDetection(Vector3 _from, Vector3 _direction)
        {
            if (Physics.Raycast(_from,
                _direction,
                out hit,
                MaxRayDistance, LayerMask))
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// Get the distance from self to the obstacle
        /// If no hit object, return -1
        /// </summary>
        /// <returns>A float value</returns>
        public float DistanceTo()
        {
            if (hit.collider == null)
            {
                /*throw new Exception("Trying call null ref of ray distance detective");*/
                return -1f;
            }

            return hit.distance;
        }

        /// <summary>
        /// Debug method drawing the ray in unity scene window
        /// </summary>
        /// <param name="_from"></param>
        /// <param name="_direction"></param>
        public void DrawRay(Vector3 _from, Vector3 _direction, Color _color)
        {
            Debug.DrawRay(_from, _direction, _color);
        }
    }
}
