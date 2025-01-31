using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;

public class IA_Simple : MonoBehaviour
{
    public enum EnemyState { Patrullando, Persiguiendo, Atacando, Esperando, Muerto }
    private EnemyState currentState;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;


    [SerializeField] private float distanciaVision;
    [SerializeField] private float distanciaAtaque;
    [SerializeField] private float tiempoEspera;
    [SerializeField] private UnityEvent eventoAtaque;
    [SerializeField] private Transform player;


    [SerializeField] private int puntosVida;


    private NavMeshAgent agent;
    private Transform currentPatrolPoint;
    private float esperaActual;
    private bool isMoving;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        currentPatrolPoint = pointA;

        currentState = EnemyState.Patrullando;


        isMoving = false;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrullando:
                Patrullar();
                break;
            case EnemyState.Persiguiendo:
                Perseguir();
                break;
            case EnemyState.Atacando:
                Atacar();
                break;
            case EnemyState.Esperando:
                Esperar();
                break;
            case EnemyState.Muerto:
                break;
        }
    }

    private void Patrullar()
    {
        agent.SetDestination(currentPatrolPoint.position);

        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1.5f)
        {
            currentPatrolPoint = currentPatrolPoint == pointA ? pointB : pointA;
        }

        if (Vector3.Distance(transform.position, player.position) <= distanciaVision)
        {
            currentState = EnemyState.Persiguiendo;
        }
    }

    private void Perseguir()
    {
        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= distanciaAtaque)
        {
            currentState = EnemyState.Atacando;
            agent.ResetPath();
        }
        else if (Vector3.Distance(transform.position, player.position) > distanciaVision)
        {
            currentState = EnemyState.Patrullando;
            agent.SetDestination(currentPatrolPoint.position);
        }
    }

    private void Atacar()
    {
        eventoAtaque?.Invoke();
        currentState = EnemyState.Esperando;

        esperaActual = tiempoEspera;
    }

    private void Esperar()
    {
        esperaActual -= Time.deltaTime;

        if (esperaActual <= 0)
        {
            currentState = EnemyState.Patrullando;

            agent.SetDestination(currentPatrolPoint.position);
        }
    }

    public void Golpear()
    {
        if (currentState != EnemyState.Esperando && currentState != EnemyState.Muerto)
        {

            puntosVida -= 1;

            if (puntosVida <= 0)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                agent.ResetPath();
                currentState = EnemyState.Muerto;
                Destroy(this.gameObject, 1);
            }
            else
            {
                agent.ResetPath();
                currentState = EnemyState.Esperando;
                esperaActual = tiempoEspera;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaVision);
    }

}