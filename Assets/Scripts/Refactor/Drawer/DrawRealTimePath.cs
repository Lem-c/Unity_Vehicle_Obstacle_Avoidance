using System;
using UnityEngine;

public class DrawRealTimePath : DrawPen
{
    int count = 0;

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
            if (count > 2)
            {
                AddNewPoint(Vehicle.GetComponent<Transform>().position);
                count = 0;
            }

            // Update points of lineRanderer
            DrawLine();
            count += 1;
        }
    }
}
