using UnityEngine;

namespace VehicleEqipment.Camera
{
    public class CameraDetector : Detector
    {

        public CameraDetector(int _layer, float _rayDistance, float _sideVisualAngle = 0)
        {
            SetParameters(_layer, _rayDistance, _sideVisualAngle);
        }
    }
}
