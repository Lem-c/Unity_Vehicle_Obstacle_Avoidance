using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionManager
{
    /// <summary>
    /// Default movement controller inherit from: SetpController
    /// Main methods used from parents
    /// </summary>
    public class SmoothMove : StepController
    {
        public SmoothMove(float _maxSpeed, float _MaxRayDistance) : base(_maxSpeed, _MaxRayDistance)
        {
            InitFuzzyChip(_maxSpeed, _MaxRayDistance);
        }
    }
}
