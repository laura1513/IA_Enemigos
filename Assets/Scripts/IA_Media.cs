using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;

public class IA_Media : MonoBehaviour
{
    
    [Header("Patrulla")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed;
    public float chaseSpeed;

    [Header("Rango de visión")]
    public float visionRange;
    public float visionAngle;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Ataque")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private NavMeshAgent agent;
    private Transform player;
    private bool playerInSight = false;
    private bool isChasing = false;
    private Transform currentTarget;
    private float lastFireTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        currentTarget = pointA;
        agent.SetDestination(currentTarget.position);
    }

    void Update()
    {
        CheckPlayer();

        if (playerInSight)
        {
            ChasePlayer();
            AttackPlayer();
        }
        else if (isChasing)
        {
            Stop();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
            agent.SetDestination(currentTarget.position);
        }
    }

    void CheckPlayer()
    {
        agent.SetDestination(firePoint.position);

        if (Vector3.Distance(transform.position, firePoint.position) <= visionAngle)
        {
            AttackPlayer();
        }
        else if (Vector3.Distance(transform.position, firePoint.position) > visionRange)
        {
            Patrol() ;
        }
    }

    void ChasePlayer()
    {
        if (!isChasing)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
        }
        agent.SetDestination(player.position);
    }

    void Stop()
    {
        isChasing = false;
        agent.speed = patrolSpeed;
        agent.SetDestination(currentTarget.position);
    }

    void AttackPlayer()
    {
        if (Time.time > lastFireTime + (1f / fireRate))
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionAngle);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);

    }
}
