using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;

    public WeaponController weapon;

    /// <summary>
    /// Referencia al componente Rigidbody2D del gameObject
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Referencia al componente Animator del gameObject
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Referencia al componente SpriteRenderer del gameObject
    /// </summary>
    private SpriteRenderer spriteRenderer;


    private Vector2 movement;

    private Vector2 lookDir;

    public float spriteBlinkingTimer = 0.0f;
    public float spriteBlinkingMiniDuration = 0.1f;
    private float spriteBlinkingTotalTimer = 0.0f;
    public float spriteBlinkingTotalDuration = 1.0f;
    bool isInvencible = false;

    public float HitColorAnimDuration = 0.1f;
    public float InvencibilityFrameDuration = 0.5f;

    [HideInInspector]
    public int Health { get;  set; } = 100;

    [HideInInspector]
    public int GunAmmo { get; set; } = 0;

    public delegate void StatusChanged();

    /// <summary>
    /// Evento que se invocará cuando el jugador pierda o gane vida
    /// </summary>
    public event StatusChanged OnHealthChanged;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        weapon.RotateGun(angle);

        if (Math.Abs(angle) > 90)
        {
            spriteRenderer.flipX = true;
        }
        else spriteRenderer.flipX = false;

        animator.SetFloat("angle", angle);
        animator.SetFloat("movement", movement.magnitude);

        if (Input.GetButtonDown("Fire1"))
        {
            weapon.Shoot();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown("1")) 
        {
            weapon.ChangeWeapon(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            weapon.ChangeWeapon(1);
        }
        if (isInvencible) 
        {
            SpriteBlinkingEffect();
        }

    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > MoveSpeed) rb.velocity = rb.velocity.normalized * MoveSpeed;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //rb.MovePosition(rb.position + movement * MoveSpeed * Time.fixedDeltaTime);
        rb.velocity += movement.normalized * Time.deltaTime * MoveSpeed;
        lookDir = mousePosition - rb.position;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.otherCollider.CompareTag("Player"))
        {
            if (other.collider.CompareTag("Enemy") && !isInvencible)
            {
                isInvencible = true;
                float force = 10;
                rb.AddForce(force * other.relativeVelocity.normalized, ForceMode2D.Impulse);
                //Recibimos daño
                Health -= 10;
                OnHealthChanged?.Invoke();
                StartCoroutine("HandleHitAnim");
            }
            if (other.collider.CompareTag("Ammo"))
            {
                GunAmmo++;
                Destroy(other.gameObject);
            }
        }
    }

    private IEnumerator HandleHitAnim()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(HitColorAnimDuration);
        spriteRenderer.color = Color.white;
    }

    private void SpriteBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            isInvencible = false;
            spriteBlinkingTotalTimer = 0.0f;
            spriteRenderer.enabled = true;   // according to 
                                                                             //your sprite
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (spriteRenderer.enabled == true)
            {
                spriteRenderer.enabled = false;  //make changes
            }
            else
            {
                spriteRenderer.enabled = true;   //make changes
            }
        }
    }

}
