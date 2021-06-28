using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    /// <summary>
    /// El prefab de las balas que dispararán nuestras armas
    /// </summary>
    public GameObject BulletPrefab;

    /// <summary>
    /// La velocidad de las balas
    /// </summary>
    public float BulletSpeed = 20f;

    /// <summary>
    /// EL daño de las balas
    /// </summary>
    public float BulletDamage = 50;

    public float FistDamage = 25;

    public AudioClip ShootAudio;


    /// <summary>
    /// La posición inicial del arma
    /// </summary>
    private Vector3 defaultGunPos;

    /// <summary>
    /// Referencia al componente SpriteRenderer del arma
    /// </summary>
    private SpriteRenderer gunSpriteRenderer;

    private int currentWeapon = 0;

    /// <summary>
    /// Referencia al componente Animator del gameObject
    /// </summary>
    private Animator animator;

    private AudioSource source;

    private PlayerController player;
    void Start()
    {
        animator = GetComponent<Animator>();
        gunSpriteRenderer = GetComponent<SpriteRenderer>();
        source = gameObject.AddComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        float vol =  ((float)PlayerPrefs.GetInt("VolumeEffect", 100))/100f;
        source.volume = vol;
        defaultGunPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RotateGun(float angle)
    {
        Vector3 GunLookDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float GunAngle = Mathf.Atan2(GunLookDir.y, GunLookDir.x) * Mathf.Rad2Deg;

        transform.parent.rotation = Quaternion.Euler(0, 0, GunAngle);
        if (Math.Abs(angle) > 90)
        {
            transform.localPosition = new Vector3(-defaultGunPos.x, defaultGunPos.y, defaultGunPos.z);
            gunSpriteRenderer.flipY = true;
        }
        else
        {
            transform.localPosition = defaultGunPos;
            gunSpriteRenderer.flipY = false;
        }
    }

    public void Shoot()
    {
        animator.SetTrigger("Fire");
        if (currentWeapon == 1) //pistola
        {
            if (player.GunAmmo > 0)
            {
                int dirY = (gunSpriteRenderer.flipY) ? -1 : 1;
                Vector3 gunEnd = new Vector3(gunSpriteRenderer.sprite.bounds.extents.x, dirY * gunSpriteRenderer.sprite.bounds.extents.y / 2);
                Vector3 SpawnPos = transform.position + transform.rotation * gunEnd;
                GameObject bullet = Instantiate(BulletPrefab, SpawnPos, transform.rotation);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(transform.right * BulletSpeed, ForceMode2D.Impulse);
                bullet.GetComponent<Bullet>().SetBulletDamage(BulletDamage);
                source.PlayOneShot(ShootAudio);
                player.GunAmmo--;
            }

        }

    }

    public void ChangeWeapon(int weapon)
    {
        currentWeapon = weapon;
        animator.SetInteger("weaponType", weapon);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.otherCollider.CompareTag("Weapon"))
        {
            if (other.collider.CompareTag("Enemy"))
            {
                EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
                float force = - 1000;
                other.rigidbody.AddForce(force * other.relativeVelocity.normalized , ForceMode2D.Impulse);
                enemy.TakeDamage(FistDamage);
            }

        }
    }
}
