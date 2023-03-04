using System;
using UnityEngine;

public class DrawRealTimePath : DrawPen
{
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
        // Care about the script that binded with Vehicle object 
        if (StateCheck())
        {
            AddNewPoint(Vehicle.GetComponent<Transform>().position);

            // Update points of lineRanderer
            DrawLine();
        }
    }
}
