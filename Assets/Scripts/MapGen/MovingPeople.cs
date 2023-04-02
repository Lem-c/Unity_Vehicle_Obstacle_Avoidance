using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPeople : MonoBehaviour
{
    // Start is called before the first frame update
    Transform[] movingPeopleList;
    void Start()
    {
        movingPeopleList = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        int count = 0;

        foreach (var i in movingPeopleList)
        {
            if (count < 2)
            {
                var side = Random.Range(0, 2);
                RandomMoving(i, side);
            }
            else
            {
                var side = Random.Range(2, 4);
                RandomMoving(i, side);
            }

            count++;
        }
    }

    private void RandomMoving(Transform _obj, int _side=1)
    {
        RandomMove(_obj, 2f, 3, _side);
    }

    private void MoveForward(Transform _obj, float _step, int _sing, int _type=1)
    {
        Vector3 tempLocation = _obj.position;
        Vector3 newLocation;

        if (_type == 1)
        {
            newLocation = new Vector3(tempLocation.x + _step * Time.deltaTime * _sing, tempLocation.y, tempLocation.z);
        }
        else
        {
            newLocation = new Vector3(tempLocation.x, tempLocation.y, tempLocation.z + _step * Time.deltaTime * _sing);
        }

        _obj.position = newLocation;
    }

    private void RandomMove(Transform _obj, float _step, int _time=1, int _choice=1)
    {
        switch(_choice)
        {
            case 0:
                for (int i = 0; i < _time; i++)
                {
                    MoveForward(_obj, _step, 1);
                }
                break;
            case 1:
                for (int i = 0; i < _time; i++)
                {
                    MoveForward(_obj, _step, -1);
                }
                break;
            case 2:
                for (int i = 0; i < _time; i++)
                {
                    MoveForward(_obj, _step, 1, 2);
                }
                break;
            case 3:
                for (int i = 0; i < _time; i++)
                {
                    MoveForward(_obj, _step, -1, 2);
                }
                break;
            default:
                return;
        }
    }
}
