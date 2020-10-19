// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public Transform goal;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = goal.position;
    }

    public void RecalculateNavigation()
    {
        navMeshAgent.autoBraking = false;
        navMeshAgent.ResetPath();
        bool result = navMeshAgent.SetDestination(goal.position);
        Debug.Log("RecaculateNavigation: " + gameObject.name + " = " + (result ? "TRUE" : "FALSE"));
    }

}