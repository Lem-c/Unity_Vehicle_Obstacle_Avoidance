using BezierVector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPen : MonoBehaviour
{
    private LineRenderer line;
    private Bezier bezier;                  // Curve darawing class
    private Material penMat;                // Material of pen

    // data array
    private ArrayList pointList;            // List saving coordinates of Vehicle
    private List<Vector3> lineList;         // List saving points of line

    // Tracing target
    protected GameObject Vehicle;
    protected GameObject pen;

    /// <summary>
    /// The default construction of parameters
    /// </summary>
    protected void DefaultSetup()
    {
        pen = GameObject.FindWithTag("Drawer");
        Vehicle = GameObject.FindWithTag("Player");

        pointList = new ArrayList();
        lineList = new List<Vector3>();

        line = pen.GetComponent<LineRenderer>();
        FastInitLine();

        bezier = new Bezier();
    }

    /// <summary>
    /// Drawing line according to the number of points in array
    /// </summary>
    protected void DrawLine()
    {
        if(pointList.Count < 4)
        {
            return;
        }

        line.positionCount = lineList.Count;
        lineList.Add(bezier.formula(pointList, 0.1f));
        line.SetPositions(lineList.ToArray());
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

        line.startColor = Color.blue;
        line.endColor = Color.blue;
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
        line.numCapVertices = 2;      // End point slick
        line.numCornerVertices = 2;
    }

    protected bool StateCheck()
    {
        if (Vehicle.GetComponent<LightCar>().enabled)
        {
            return Vehicle.GetComponent<LightCar>().GetCurrentState();
        }

        if (Vehicle.GetComponent<NAVCar>().enabled)
        {
            return Vehicle.GetComponent<NAVCar>().GetCurrentState();
        }

        if (Vehicle.GetComponent<DWACar>().enabled)
        {
            return Vehicle.GetComponent<DWACar>().GetCurrentState();
        }

        return false;
    }
}
