using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    public float radMin;
    public float radMax;
    float wanderRadius;
    public float timeMin;
    public float timeMax;
    float wanderTimer;
    private UnityEngine.AI.NavMeshAgent agent;
    private float timer;

    // Use this for initialization
    void OnEnable()
    {
        wanderRadius = Random.Range(radMin, radMax);
        wanderTimer = Random.Range(timeMin, timeMax);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = wanderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void MovePositionRandom()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        transform.position = newPos;
    }
}