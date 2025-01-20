using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;


public class IA_Simple : MonoBehaviour
{
    private enum EnemyState { Patrullando, Persiguiendo, Atacando, Esperando, Muerto }
    private EnemyState currentState;

    [Header("Puntos de Patrulla")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Variables del Enemigo")]
    [SerializeField] private float distanciaVision = 10f;
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float tiempoEspera = 3f;
    [SerializeField] private UnityEvent eventoAtaque;
    [SerializeField] private Transform player;
    [SerializeField] private int puntosVida;

    private NavMeshAgent agent;
    private Transform currentPatrolPoint;
    private float esperaActual;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentPatrolPoint = pointA;
        currentState = EnemyState.Patrullando;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrullando: Patrullar(); break;
            case EnemyState.Persiguiendo: Perseguir(); break;
            case EnemyState.Atacando: Atacar(); break;
            case EnemyState.Esperando: Esperar(); break;
        }

        ActualizarRotacion();
    }

    private void Patrullar()
    {
        agent.SetDestination(currentPatrolPoint.position);
        
        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1.5f)
            currentPatrolPoint = currentPatrolPoint == pointA ? pointB : pointA;

        if (Vector3.Distance(transform.position, player.position) <= distanciaVision)
            currentState = EnemyState.Persiguiendo;
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
        }
    }

    private void Atacar()
    {
        eventoAtaque?.Invoke();
        CambiarEstadoConEspera(EnemyState.Esperando);
    }

    private void Esperar()
    {
        if ((esperaActual -= Time.deltaTime) <= 0)
            CambiarEstado(EnemyState.Patrullando);
    }

    private void CambiarEstado(EnemyState nuevoEstado)
    {
        currentState = nuevoEstado;
        if (nuevoEstado == EnemyState.Patrullando)
            agent.SetDestination(currentPatrolPoint.position);
    }

    private void CambiarEstadoConEspera(EnemyState nuevoEstado)
    {
        currentState = nuevoEstado;
        esperaActual = tiempoEspera;
    }

    public void Golpear()
    {
        if (currentState == EnemyState.Muerto || currentState == EnemyState.Esperando) return;

        if (--puntosVida <= 0)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            currentState = EnemyState.Muerto;
            Destroy(gameObject, 1);
        }
        else
        {
            CambiarEstadoConEspera(EnemyState.Esperando);
        }
    }

    private void ActualizarRotacion()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Euler(0, agent.velocity.x > 0 ? 180 : 0, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaVision);
    }
}
