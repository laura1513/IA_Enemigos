using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;


public class IA_Aliado : MonoBehaviour
{
    //IEnumerator es una interfaz en C# que se utiliza para trabajar con secuencias de elementos o para implementar rutinas (coroutines) en Unity.
    private NavMeshAgent ally;

    public GameObject jugador;  // Referencia al jugador (puede asignarse en el Inspector)
    
    void Start()
    {
        ally.updateRotation = false;
        ally.updateUpAxis = false;
        // Si no se asigna manualmente el jugador, se busca automáticamente en la escena
        if (jugador == null)
        {
            jugador = GameObject.FindWithTag("Player");  // Buscar el jugador por su etiqueta
        }
        
        // Iniciar la corutina que actualiza la posición cada 3 segundos
        StartCoroutine(SeguirCada3Segundos());
    }

    IEnumerator SeguirCada3Segundos()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);  // Espera 3 segundos
            if (jugador != null)
            {
                transform.position = jugador.transform.position;  // Actualiza la posición del aliado
            }
        }
    }
}
