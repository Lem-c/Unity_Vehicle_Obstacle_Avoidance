using BezierVector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DrawPen : MonoBehaviour
{
    private GameObject pen;
    private LineRenderer line;
    private Bezier bezier;
    private Material penMat;

    // data array
    private ArrayList pointList;
    private List<Vector3> lineList;

    // Tracing target
    private GameObject Vehicle;

    public void DefaultSetup()
    {
        pen = GameObject.FindWithTag("Drawer");
        Vehicle = GameObject.FindWithTag("Player");

        pointList = new ArrayList();
        lineList = new List<Vector3>();

        line = pen.GetComponent<LineRenderer>();
        FastInitLine();

        bezier = new Bezier();
    }

    void Awake()
    {
        DefaultSetup();

        if (pen == null || Vehicle == null)
        {
            throw new Exception("Null pen/target object!");
        }
    }

    void Update()
    {
        if (Vehicle.GetComponent<LightCar>().GetCurrentState())
        {
            AddNewPoint(Vehicle.GetComponent<Transform>().position);

            // Update points of lineRanderer
            DrawLine();
        }
    }

    private void DrawLine()
    {
        if(pointList.Count < 3)
        {
            return;
        }

        line.positionCount = lineList.Count;
        lineList.Add(bezier.formula(pointList, 0.1f));
        line.SetPositions(lineList.ToArray());

        /*for (int i = 0; i < pointList.Count; ++i)
        {
            Vector3 to = bezier.formula(pointList, 0.5f);
            line.SetPosition(i, to);
        }*/
    }


    public void AddNewPoint(Vector3 point)
    {
        pointList.Add(point);
    }

    /// <summary>
    /// Init the style of line renderer
    /// </summary>
    private void FastInitLine()
    {
        if (line == null)
        {
            Debug.LogWarning("Init line fail!");
            return;
        }

        line.startColor = new Color(1f, 1f, 0f, 0.5f);
        line.endColor = new Color(0f, 1f, 1f, 0.5f);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.numCapVertices = 2;      // End point slick
        line.numCornerVertices = 2;
    }
}
