using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private Transform ia_media;
    [SerializeField] private Transform ia_simple;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, 5);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Taca");
        this.transform.parent = collision.gameObject.transform; //Linea mágica para que las flechas se claven en el enemigo
        _rb.angularVelocity = 0;
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;

        //Si golpeamos a un enemigo le quitamos vida y destruimos la flecha
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(collision.gameObject.GetComponent<IA_Simple>() != null)
            {
                collision.gameObject.GetComponent<IA_Simple>().Golpear();
            } else if (collision.gameObject.GetComponent<IA_Media>() != null)
            {
                collision.gameObject.GetComponent<IA_Media>().Golpear();
            }
        }
    }
}
