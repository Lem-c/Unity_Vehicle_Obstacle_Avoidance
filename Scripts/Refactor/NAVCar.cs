using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NAVCar : VehicleController
{
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;

        Debug.Log("After baking the map, click anyweher to test");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Debug.Log(Camera.main.ScreenPointToRay(Input.mousePosition));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                navMeshAgent.nextPosition = transform.position;
                navMeshAgent.SetDestination(hit.point);

            }
        }
        Move();
    }

    private void Move()
    {
        if (navMeshAgent.remainingDistance < 0.5f) return;
        navMeshAgent.nextPosition = transform.position;
        if (navMeshAgent.desiredVelocity == Vector3.zero) return;
        Quaternion targetQuaternion = Quaternion.LookRotation(navMeshAgent.desiredVelocity, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.deltaTime * 3);
        transform.Translate(Vector3.forward * Time.deltaTime * 3);
    }
    protected override void HandBreak()
    {
        throw new System.NotImplementedException();
    }

    protected override void ProcessDecision()
    {
        throw new System.NotImplementedException();
    }
}
