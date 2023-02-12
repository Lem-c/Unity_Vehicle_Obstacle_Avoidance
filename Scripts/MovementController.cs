using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionManager
{
    public class MovementController : StepManager
    {
        private GameObject Target;           // The main vehicle
        private int DetectiveLayer;          // The target layer where obstacles located
        private float RayMaxDistance;


        public MovementController(GameObject _target)
        {
            if (_target == null)
            {
                throw new ArgumentException("Null object reference");
            }


            Target = _target;
            DetectiveLayer = 1 << 3;
            RayMaxDistance = 10f;
        }

        /// <summary>
        /// The movement in 'z'-axis would be made through this method
        /// Override abs method from 'StepManager
        /// Get estimate distance from self to the obstacle'
        /// </summary>
        /// <param name="_velocity">Current velocity used to control action and decision</param>
        public override MoveMent StrightMovementDecisionMaker(float _velocity, float _scale = 1f)
        {
            if (Target == null)
            {
                throw new ArgumentException("Null __Target__ reference");
            }

            // Change default look rang(distance)
            float rayDistance = RayMaxDistance;

            // vague distance measure
            float fuzzyDIstance;


            RaycastHit hit;

            if (Physics.Raycast(Target.GetComponent<Transform>().position,
                                   Target.GetComponent<Transform>().TransformDirection(Vector3.forward),
                                   out hit,
                                   rayDistance, DetectiveLayer))
            {
                fuzzyDIstance = ObscureDistanceEstimator(hit.distance, 0.5f);
                var action = BreakPattenSimulation(FuzzyPredictHowClose(fuzzyDIstance));

                if (GetLengthOfRecord() < 20)
                {
                    AddNewRecord(action);
                }

                return action;
            }

            if (GetLengthOfRecord() < 20)
            {
                AddNewRecord(MoveMent.MoveForward);
            }

            return MoveMent.MoveForward;
        }

        /// <summary>
        /// Control the turning decision of the vehicle
        /// </summary>
        /// <returns>Turning movement</returns>
        public override MoveMent TurningDecisionMaker()
        {
            if (Target == null)
            {
                throw new ArgumentException("Null __Target__ reference");
            }

            // Change default look rang(distance)
            float rayDistance = RayMaxDistance;

            RaycastHit hit;

            if (Physics.Raycast(Target.GetComponent<Transform>().position,
                                   Target.GetComponent<Transform>().TransformDirection(Vector3.forward),
                                   out hit,
                                   rayDistance, DetectiveLayer))
            {

                var choice = SectorAreaIntercetDetection(Target.GetComponent<Transform>(),
                                                     hit.collider.GetComponent<Transform>(),
                                                     hit.distance);

                // Check whether the turning operation can be do immediately
                // If can't, clear commands
                var turningLevel = UnityEngine.Random.Range(2, 6);
                int counter = 0;
                if (GetLengthOfRecord() > 6 && GetNextMove() != MoveMent.TurnLeft && GetNextMove() != MoveMent.TurnRight)
                {
                    RefreshRecord();
                }

                while (counter < turningLevel)
                {
                    AddNewRecord(TurningPatternSimulation(choice));
                    counter += 1;
                }

                return TurningPatternSimulation(choice);
            }

            AddNewRecord(MoveMent.MoveForward);
            return MoveMent.MoveForward;
        }


        /// <summary>
        /// The simulation of distance prediction
        /// Get the distance that can not be measured directly
        /// </summary>
        /// <param name="_accuDistance">The accurate/incorrect distance</param>
        /// <param name="_mode">How to trust the distance that offered</param>
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
                _temp = UnityEngine.Random.Range(0.8f, 3f);
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

            if (_distance <= 1f)
            {
                return 5;
            }

            if (_distance > 1f - bias && _distance < 2f + bias)
            {
                return 4;
            }

            if (_distance > 3f && _distance < 5f - bias)
            {
                return 3;
            }

            if (_distance > 6 - bias && _distance < 9 + bias)
            {
                return 2;
            }
            else if (_distance <= 10f + bias)
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
                    return MoveMent.MoveForward;
                case 1:
                    if (UnityEngine.Random.Range(0, 100) > 50)
                    {
                        return MoveMent.MoveForward;
                    }

                    return MoveMent.Wait;
            }


            return MoveMent.Wait;
        }


        /// <summary>
        /// Select turning pattern according to the choice
        /// Used with method 'SectorAreaIntercetDetection()'
        /// </summary>
        /// <param name="_choice">Choice get from 'SectorAreaIntercetDetection()'</param>
        /// <returns>Turning movement pattern</returns>
        private MoveMent TurningPatternSimulation(int _choice)
        {
            switch(_choice){
                case 0:
                    return MoveMent.MoveForward;
                case 1:
                    return MoveMent.TurnLeft;
                case 2:
                    return MoveMent.TurnRight;
                case 3:
                    if(UnityEngine.Random.Range(0, 100) > 50)
                    {
                        return MoveMent.TurnRight;
                    }
                    return MoveMent.TurnLeft;
            }

            return MoveMent.Wait;
        }


        /// <summary>
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
            if (distance >= sectorSensitive){
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
            /*Debug.DrawLine(_object.position, norVec, Color.red);
            Debug.DrawLine(_object.position, LeftVec, Color.green);
            Debug.DrawLine(_object.position, RightVec, Color.green);*/

            // Calculate the angle-off difference [whether close to left or right]
            float angleOffLeft = Mathf.Acos(Vector3.Dot(norVec.normalized, LeftVec.normalized)) * Mathf.Rad2Deg;
            float angleOffRight = Mathf.Acos(Vector3.Dot(norVec.normalized, RightVec.normalized)) * Mathf.Rad2Deg;

            float angleDif = angleOffLeft - angleOffRight;

            if(angleDif>-2.5f && angleDif < 5f)
            {
                return 3;
            }

            if (angleDif <= -2.5f)
            {
                // Debug.Log("Inside the sector");
                return 1;
            }

            return 2;
        }
    }
}