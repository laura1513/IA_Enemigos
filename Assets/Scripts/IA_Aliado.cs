using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;


public class IA_Aliado : MonoBehaviour
{
    public Transform jugador; // Referencia al jugador
    public float distanciaSeguimiento; // Distancia mínima para seguir al jugador
    public float rangoAtaque; // Rango para atacar enemigos
    public float velocidadMovimiento = 3f; // Velocidad de movimiento del aliado
    public int dañoAtaque; // Daño que hace el aliado al atacar
    public int puntosVida; // Puntos de vida del aliado

    private Transform objetivoEnemigo; // Referencia al enemigo más cercano
    private NavMeshAgent agent;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        // Buscar al enemigo más cercano
        BuscarEnemigoCercano();

        if (objetivoEnemigo != null && Vector3.Distance(transform.position, objetivoEnemigo.position) <= rangoAtaque)
        {
            // Atacar al enemigo si está dentro del rango
            AtacarEnemigo();
        }
        else
        {
            // Seguir al jugador si no hay enemigos en rango
            SeguirJugador();
        }
    }

    void BuscarEnemigoCercano()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
        float distanciaMasCorta = Mathf.Infinity;
        Transform enemigoMasCercano = null;

        foreach (GameObject enemigo in enemigos)
        {
            float distanciaAlEnemigo = Vector3.Distance(transform.position, enemigo.transform.position);
            if (distanciaAlEnemigo < distanciaMasCorta)
            {
                distanciaMasCorta = distanciaAlEnemigo;
                enemigoMasCercano = enemigo.transform;
            }
        }

        if (enemigoMasCercano != null && distanciaMasCorta <= rangoAtaque)
        {
            objetivoEnemigo = enemigoMasCercano;
        }
        else
        {
            objetivoEnemigo = null;
        }
    }

    void AtacarEnemigo()
    {
        // Simula el ataque al enemigo
        Debug.Log("Atacando al enemigo: " + objetivoEnemigo.name);
        objetivoEnemigo.GetComponent<IA_Media>()?.Golpear();
    }

    void SeguirJugador()
    {
        if (Vector3.Distance(transform.position, jugador.position) > distanciaSeguimiento)
        {
            // Mover al aliado hacia el jugador sin rotar
            Vector3 direccion = (jugador.position - transform.position).normalized;
            transform.position += direccion * velocidadMovimiento * Time.deltaTime;
            Debug.Log("Siguiendo al jugador");
        }
    }

    public void RecibirDaño(int daño)
    {
        puntosVida -= daño;
        Debug.Log("Aliado recibió daño, vida restante: " + puntosVida);

        if (puntosVida <= 0)
        {
            Muerte();
        }
    }

    void Muerte()
    {
        Debug.Log("El aliado ha muerto");
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaSeguimiento);
    }
}
