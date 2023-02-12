using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActionManager
{

    /// <summary>
    /// The class used to record every step that object from Vehicl class
    /// Can be serialized and reload as a bit map to guide movement
    /// </summary>
    public abstract class StepManager
    {
        /// <summary>
        /// The movement enum structure used to represent step value
        /// </summary>
        public enum MoveMent
        {
            MoveForward,
            MoveBackward,
            TurnLeft,
            TurnRight,
            Break,
            LightBreak,
            SlamBreak,
            HandBreak,
            Wait
        }

        // Movement revord list
        private ArrayList RecordSteps = new ArrayList();

        private MoveMent nextMove = MoveMent.Wait;
        // private MoveMent predictNextMove;

        protected MoveMent GetNextMove()
        {
            return nextMove;
        }

        protected void AddNewRecord(MoveMent _step)
        {
            RecordSteps.Add(_step);
        }

        protected void RefreshRecord()
        {
            if(GetLengthOfRecord() == 0)
            {
                return;
            }

            RecordSteps.RemoveRange(0, RecordSteps.Count);
        }

        protected int GetLengthOfRecord()
        {
            return RecordSteps.Count;
        }

        public MoveMent ProcessNextMove()
        {

            if (GetLengthOfRecord() == 0)
            {
                return MoveMent.Wait;
            }

            nextMove = (MoveMent)RecordSteps[0];
            RecordSteps.RemoveAt(0);

            return nextMove;
        }

        public bool IsThereAnyInstructions()
        {
            if(GetLengthOfRecord() > 0)
            {
                return true;
            }

            return false;
        }

        public abstract MoveMent StrightMovementDecisionMaker(float _velocity, float _scale = 1f);
        public abstract MoveMent TurningDecisionMaker();
    }

}