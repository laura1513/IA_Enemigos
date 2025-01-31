using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;

public class IA_Media : MonoBehaviour
{
    public enum EnemyState {Persiguiendo, Atacando, Esperando, Muerto }
    private EnemyState currentState;


    [SerializeField] private float distanciaVision = 30f;
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float tiempoEspera = 3f;
    [SerializeField] private UnityEvent eventoAtaque;
    [SerializeField] private Transform player;


    [SerializeField] private int puntosVida;


    private NavMeshAgent agent;
    private float esperaActual;
    private bool isMoving;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        currentState = EnemyState.Persiguiendo;


        isMoving = false;
    }

    void Update()
    {
        switch (currentState)
        {
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

   
    private void Perseguir()
    {
        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= distanciaAtaque)
        {
            currentState = EnemyState.Atacando;
            agent.ResetPath();
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
            currentState = EnemyState.Persiguiendo;

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