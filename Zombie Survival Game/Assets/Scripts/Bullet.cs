using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    private float damage;

    private void OnCollisionEnter2D(Collision2D other)
    {
        //si la colisión es con un enemigo
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.TakeDamage(damage); //entonces le producimos daño
            //OnEnemyDestroyed?.Invoke(); //invocamos el evento
        }
        Instantiate(ExplosionPrefab, transform.position, transform.rotation); //instanciamos la explosión
        Destroy(this.gameObject);
    }

    public void SetBulletDamage(float damage)
    {
        this.damage = damage;
    }
}
