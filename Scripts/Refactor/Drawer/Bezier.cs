using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierVector
{
    public class Bezier
    {
        List<Vector3> points;

        public Bezier(Vector3[] points)
        {
            if (points.Length == 0 || points == null)
            {
                throw new ArgumentException("Empty points array");
            }

            this.points = new List<Vector3>(points);
        }

        public Bezier()
        {

        }

        public Vector3 LinearBezier(Vector3 _start, Vector3 _end, float _smoothness)
        {
            return _start + (_end - _start) * _smoothness;
        }

        /// <summary>
        /// Second order:biquad bezier calculation method
        /// </summary>
        /// <param name="_start">Start point</param>
        /// <param name="_mid">Mid connection point</param>
        /// <param name="_end">End point</param>
        /// <param name="_smoothness">T</param>
        /// <returns>quadratic bezier vector</returns>
        public Vector3 QuadraticBezier(Vector3 _start, Vector3 _mid, Vector3 _end, float _smoothness)
        {
            Vector3 sm = LinearBezier(_start, _mid, _smoothness);
            Vector3 me = LinearBezier(_mid, _end, _smoothness);

            return LinearBezier(sm, me, _smoothness);
        }

        public Vector3 CubicBezier(Vector3 _start, Vector3 _m1,Vector3 _m2 , Vector3 _end, float _t)
        {
            Vector3 smm = QuadraticBezier(_start, _m1, _m2, _t);
            Vector3 mme = QuadraticBezier(_m1, _m2, _end, _t);

            return LinearBezier(smm, mme, _t);
        }

        /// <summary>
        /// Choose mezier method accoding to the lenght of array list
        /// </summary>
        /// <param name="_vectorList">Points where saved</param>
        /// <param name="t">time/accu/smoothness</param>
        /// <returns>Vector</returns>
        internal Vector3 formula(ArrayList _vectorList, float t)
        {
            if(_vectorList.Count < 3)
            {
                return Vector3.zero;
            }

            var mod = -1;
            if((_vectorList.Count % 3) == 0){
                mod = 3;
            }
            if((_vectorList.Count % 4) == 0){
                mod = 4;
            }

            switch(mod)
            {
                case 3:
                    var val_1 = QuadraticBezier((Vector3)_vectorList[0],
                                                (Vector3)_vectorList[1],
                                                (Vector3)_vectorList[2], t);
                    _vectorList.RemoveRange(0, 2);
                    return val_1;
                case 4:
                    var val_2 = CubicBezier((Vector3)_vectorList[0],
                                                (Vector3)_vectorList[1],
                                                (Vector3)_vectorList[2],
                                                (Vector3)_vectorList[3], t);
                    _vectorList.RemoveRange(0, 4);
                    return val_2;
                default:
                    return Vector3.zero;
            }
        }
    }
}
