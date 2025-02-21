using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float nextDamageTime;
    [SerializeField] ParticleSystem particulasDMG;
    [SerializeField] ParticleSystem particulasMuerte;
    [SerializeField] GameObject ia_media;
    [SerializeField] GameObject ia_simple;
    [SerializeField] GameObject ia_aliado;
    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void OnTriggerStay2D(Collider2D colision)
    {
        // Verificamos si colisiona con un objeto que tenga el tag "Enemy" o "Wizard o WizardEnemy" o con cualquier "Boss"
        if (colision.CompareTag("Enemy"))
        {
            if (Time.time > nextDamageTime)
            {
                TakeDamage(1); // Aplicar da�o
                Instantiate(particulasDMG, this.transform); // Instanciar part�culas
                nextDamageTime = Time.time + cooldownTime; // Actualizar tiempo de espera para recibir da�o
            }
        }
    }


    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Instantiate(particulasMuerte, transform.position, transform.rotation);
            Destroy(gameObject);
            Destroy(ia_media);
            Destroy(ia_simple);
            Destroy(ia_aliado);
        }
    }
}
