// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;


public class MoveTo : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    //Transform goal;
    int lureId = -1;
    Vector3 exitPosition = default;
    Vector3 lurePosition = default;
    Vector3 wanderPosition = default;
    bool isWandering = false;


    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }


    public void SetWander(Vector3 destination)
    {
        isWandering = true;
        wanderPosition = destination;
        navMeshAgent.SetDestination(currentDestination());
    }

    public void CancelWander()
    {
        isWandering = false;
        navMeshAgent.SetDestination(currentDestination());
    }


    public void SetExitPosition(Vector3 pos)
    {
        exitPosition = pos;
        navMeshAgent.SetDestination(currentDestination());
    }

    public void SetSpeed(float speed)
    {
        navMeshAgent.speed = speed;
    }

    public void SetLure(int id, Vector3 position)
    {
        lureId = id;
        lurePosition = position;
        navMeshAgent.SetDestination(lurePosition);
    }

    public void CancelLure(int id, float speed)
    {
        if (lureId == id)
        {
            lureId = -1;
            navMeshAgent.SetDestination(exitPosition);
            SetSpeed(speed);
        }

    }


    Vector3 currentDestination()
    {
        return lureId >= 0 ? lurePosition : 
            isWandering ? wanderPosition :
            exitPosition;
    }

    public void RecalculateNavigation()
    {
        navMeshAgent.autoBraking = false;
        navMeshAgent.ResetPath();
        bool result = navMeshAgent.SetDestination(currentDestination());
        //Debug.Log("RecaculateNavigation: " + gameObject.name + " = " + (result ? "TRUE" : "FALSE"));
    }

}