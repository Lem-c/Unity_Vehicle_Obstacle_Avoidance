using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ActionManager
{
    public class MovementController : StepManager
    {
        private GameObject Target;           // The main vehicle
        private int DetectiveLayer;          // The target layer where obstacles located
        private float RayMaxDistance;
        private int SideVisualAngle;

        private float StraightBias = 7.1f;     // Normally the distance estimated ahead is further 
        private bool isTurning = false;
        private bool isForwardBlocked = false;

        // Equipment List
        LidarDetector leftRadar;
        LidarDetector rightRadar;
        LidarDetector cameraMain;

        public MovementController(GameObject _target, float _rayDistance, int _detectiveLayer = 3, int _sideVisualAngle = 20)
        {
            if (_target == null)
            {
                throw new ArgumentException("Null object reference");
            }

            Target = _target;
            DetectiveLayer = 1 << _detectiveLayer;
            RayMaxDistance = _rayDistance;
            SideVisualAngle = _sideVisualAngle;

            leftRadar = new LidarDetector(DetectiveLayer, RayMaxDistance-StraightBias, SideVisualAngle);
            rightRadar = new LidarDetector(DetectiveLayer, RayMaxDistance-StraightBias, SideVisualAngle);
            cameraMain = new LidarDetector(DetectiveLayer, RayMaxDistance);

            StrightMovementDecisionMaker();
            TurningDecisionMaker();
        }

        /// <summary>
        /// The movement in 'front'-axis would be made through this method
        /// Override abs method from 'StepManager
        /// Get estimate distance from self to the obstacle'
        /// </summary>
        public override void StrightMovementDecisionMaker()
        {
            if (Target == null)
            {
                throw new ArgumentException("Null __Target__ reference");
            }

            // vague distance measure
            float fuzzyDistance;

            if (cameraMain.RayDetection(Target.GetComponent<Transform>()))
            {
                // Debug.Log("Something String forward!");

                fuzzyDistance = ObscureDistanceEstimator(cameraMain.DistanceTo(), 0.5f);
                // Add break type actions
                var action = BreakPattenSimulation(fuzzyDistance);

                AddNewRecord(action);

                return;
            }

            if (!isTurning || IsOneWayOut())
            {
                leftRadar.RecoverAngle();
                rightRadar.RecoverAngle();

                AddNewRecord(MoveMent.MoveForward);
                return;
            }
            
        }

        /// <summary>
        /// Control the turning decision of the vehicle
        /// </summary>
        /// <returns>Turning movement</returns>
        public override void TurningDecisionMaker()
        {

            if (Target == null)
            {
                throw new ArgumentException("Null __Target__ reference");
            }

            float leftDistance = SideDetectResult(leftRadar, -1);
            float rightDistance = SideDetectResult(rightRadar, 1);

            if(GetLengthOfRecord() > 3)
            {
                RefreshRecord();
            }

            TwoPointDecisionMaker(leftDistance, rightDistance);
        }

        /// <summary>
        /// Dectec the obstacles in left or right side
        /// </summary>
        /// <param name="_direction">default as -1 means: left </param>
        /// <returns>
        /// If the result is lower than zero,
        /// it means there is no obstacles, otherwise,
        /// return the distance form it.
        /// </returns>
        private float SideDetectResult(LidarDetector _lidar , int _direction=-1)
        {
            // vague distance measure
            float fuzzyDistance;

            if (_lidar.RangRayDetection(_direction, Target.GetComponent<Transform>()))
            {
                fuzzyDistance = ObscureDistanceEstimator(_lidar.DistanceTo(), 0);
                // Add break type actions
                /*var action = BreakPattenSimulation(FuzzyPredictHowClose(fuzzyDIstance));*/

                return fuzzyDistance;
            }

            return -1;
        }

        private void TwoPointDecisionMaker(float _left, float _right)
        {
            if(_left < 0 && _right < 0)
            {
                if(GetIsForwardBlocked())
                {
                    RandomTurnning();
                    return;
                }

                isTurning = false;
                return;
            }

            var similar = IsTwoFloatValueSimilar(_left, _right, 0.05f);

            isTurning = true;

            // If obstacle close to right
            if ( (_left < 0 && _right > 0) ||
                (!similar && (_left > _right) && _right > 0) )
            {
                // Far enough
                if (FuzzyPredictHowClose(_right) < 3){ return;}

                var turn_action = MoveMent.TurnLeft;

                // A little bit far, whether need to slow down
                // var break_action = BreakPattenSimulation(_right);
                // if (FuzzyPredictHowClose(_right) > 3) { AddNewRecord(break_action); }
                rightRadar.ShrinkAngle(10);
                AddNewRecord(turn_action);
                AddNewRecord(MoveMent.MoveForward);

                return;
            }

            // if obstacle close to left
            if ( (_right < 0 && _left > 0) || 
                (!similar && (_left < _right) && _left > 0) )
            {
                // Far enough
                if (FuzzyPredictHowClose(_left) < 3) { return; }

                var turn_action = MoveMent.TurnRight;

                leftRadar.ShrinkAngle(10);
                AddNewRecord(turn_action);
                AddNewRecord(MoveMent.MoveForward);

                return;
            }

            isTurning = false;
            // AddNewRecord(MoveMent.MoveForward);
        }

        private bool IsTwoFloatValueSimilar(float _a, float _b, float _range)
        {
            if(_a< 0 || _b < 0) { return false;}

            float _bias = 0.1f;
            if(_a - _b < _range + _bias)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// check whether one side of vehicle do not has obstacle and can move
        /// </summary>
        /// <returns></returns>
        private bool IsOneWayOut()
        {
            float leftDistance = SideDetectResult(leftRadar, -1);
            float rightDistance = SideDetectResult(rightRadar, 1);

            if(leftDistance < 0 || rightDistance < 0)
            {
                return true;
            }

            if(IsTwoFloatValueSimilar(leftDistance, rightDistance, 1.3f))
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// The simulation of distance prediction
        /// Get the distance that can not be measured directly
        /// </summary>
        /// <param name="_accuDistance">The accurate/incorrect distance</param>
        /// <param name="_mode">How to trust the judgement that offered</param>
        /// <returns>An fuzzy float distance</returns>
        private float ObscureDistanceEstimator(float _accuDistance, float _mode = 1)
        {
            float _temp;
            // Balance the estimation using mis-judgement
            int _misJudge = -1;

            if (_accuDistance <= 5f)
            {
                // close range distance estimation
                _temp = UnityEngine.Random.Range(0.1f, 0.9f);
            }
            else
            {
                // Far range distance estiamtion may more unaccracy
                _temp = UnityEngine.Random.Range(0.8f, 1.7f);
            }

            if (UnityEngine.Random.Range(0, 100) < 50)
            {
                _misJudge = 1;
            }

            return _temp * _mode * _misJudge + _accuDistance;
        }

        /// <summary>
        /// According to the distance from target to the obstacle
        /// to decide current situation. The large returned value is,
        /// the more close to it.
        /// </summary>
        /// <param name="_distance">Current distance from target to obstalce</param>
        /// <param name="_bias">Float bias</param>
        /// <returns>situation</returns>
        private int FuzzyPredictHowClose(float _distance, float _bias = 0.5f)
        {
            float bias = _bias;

            if (_distance <= 0.2 * RayMaxDistance)
            {
                return 5;
            }

            if (_distance > 0.2*RayMaxDistance - bias && _distance <= 0.35*RayMaxDistance + bias)
            {
                return 4;
            }

            if (_distance > 0.35*RayMaxDistance && _distance <= 0.8*RayMaxDistance - bias)
            {
                return 3;
            }

            if (_distance > 0.8*RayMaxDistance - bias && _distance < 0.95*RayMaxDistance + bias)
            {
                return 2;
            }
            else if (_distance >= 0.95*RayMaxDistance + bias)
            {
                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Select the break patten according to the distance
        /// which measured from objetc to taget
        /// </summary>
        /// <param name="_distance">The distance from object to targe(Normally obstacle)</param>
        /// <returns>One of the  move patterns</returns>
        private MoveMent BreakPattenSimulation(float _distance)
        {
            int situation = FuzzyPredictHowClose(_distance);

            switch (situation)
            {
                case 5:
                    return MoveMent.SlamBreak;
                case 4:
                    return MoveMent.LightBreak;
                case 3:
                    return MoveMent.MoveForward;
                case 2:
                    if (UnityEngine.Random.Range(0, 100) > 50)
                    {
                        return MoveMent.MoveForward;
                    }
                    else
                    {
                        return MoveMent.Wait;
                    }
                case 1:
                    return MoveMent.MoveForward;
                default:
                    Debug.LogWarning("There is no case in this pattern");
                    return MoveMent.Wait;
            }

        }


        /// <summary>
        /// Select turning pattern according to the choice
        /// Used with method 'SectorAreaIntercetDetection()'
        /// </summary>
        /// <param name="_choice">Choice get from 'SectorAreaIntercetDetection()'</param>
        /// <returns>Turning movement pattern</returns>
        private MoveMent TurningPatternSimulation(int _choice)
        {
            switch (_choice)
            {
                case 0:
                    return MoveMent.MoveForward;
                case 1:
                    return MoveMent.TurnLeft;
                case 2:
                    return MoveMent.TurnRight;
                case 3:
                    if (UnityEngine.Random.Range(0, 100) > 50)
                    {
                        return MoveMent.TurnRight;
                    }
                    return MoveMent.TurnLeft;
            }

            return MoveMent.Wait;
        }


        public override bool GetIsForwardBlocked()
        {
            isForwardBlocked = false;

            if (FuzzyPredictHowClose(leftRadar.DistanceTo())>4 ||
                FuzzyPredictHowClose(rightRadar.DistanceTo())>4 ||
                FuzzyPredictHowClose(cameraMain.DistanceTo())>3){

                isForwardBlocked = true;

            }

            return isForwardBlocked;
        }

        private void RandomTurnning()
        {
            isTurning = true;

            var choice = UnityEngine.Random.Range(0,100);

            if(choice < 60)
            {
                AddNewRecord(MoveMent.TurnLeft);
                return;
            }

            AddNewRecord(MoveMent.TurnRight);
        }

/*        /// <summary>
        /// Simulate the visual judgement.
        /// Using sector to determain whether close to left ot right
        /// </summary>
        /// <param name="_object">The object has visual ability</param>
        /// <param name="_target">The target obstacle</param>
        /// <param name="_distane">The distance measured from object to target</param>
        /// <param name="_range">The visual distance of eye</param>
        /// <param name="_angle">The visual angle</param>
        /// <returns>
        /// Return integer 0 to 3
        /// 0: out of range
        /// 1: close to left
        /// 2: close to right
        /// 3: Obscure
        /// </returns>
        private int SectorAreaIntercetDetection(Transform _object, Transform _target, float _distane, float _range = 8f, float _angle = 60f)
        {
            float sectorSensitive = _range;// Distance of sector detected
            float sectorAngle = _angle;// Angle of sector

            // Get the distance from object to the target
            float distance = _distane;
            if (distance >= sectorSensitive)
            {
                return 0;
            }

            // Display the detection ray
            Vector3 norVec = _object.rotation * Vector3.forward * 10;

            // Position difference;
            Vector3 targetSize = _target.GetComponent<MeshFilter>().mesh.bounds.size;
            Vector3 LeftPoint = new Vector3(-(targetSize.x / 2) * _target.lossyScale.x, _target.position.y, _target.position.z);
            Vector3 RightPoint = new Vector3(targetSize.x / 2 * _target.lossyScale.x, _target.position.y, _target.position.z);
            // Get left and right vector difference
            Vector3 LeftVec = LeftPoint - _object.position;
            Vector3 RightVec = RightPoint - _object.position;

            // Assist display
            *//*Debug.DrawLine(_object.position, norVec, Color.red);
            Debug.DrawLine(_object.position, LeftVec, Color.green);
            Debug.DrawLine(_object.position, RightVec, Color.green);*//*

            // Calculate the angle-off difference [whether close to left or right]
            float angleOffLeft = Mathf.Acos(Vector3.Dot(norVec.normalized, LeftVec.normalized)) * Mathf.Rad2Deg;
            float angleOffRight = Mathf.Acos(Vector3.Dot(norVec.normalized, RightVec.normalized)) * Mathf.Rad2Deg;

            float angleDif = angleOffLeft - angleOffRight;

            if (angleDif > -2.5f && angleDif < 5f)
            {
                return 3;
            }

            if (angleDif <= -2.5f)
            {
                // Debug.Log("Inside the sector");
                return 1;
            }

            return 2;
        }*/
    }
}