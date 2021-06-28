using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// La distancia máxima a la que ve nuestro agente.
    /// </summary>
    public float ViewDistance;

    /// <summary>
    /// Si se asigna, este elemento se mostrará cuando el jugador esté a la vista y lo oculta cuando no.
    /// </summary>
    public GameObject PlayerInFovIcon;

    /// <summary>
    /// El águnlo de vision del agente, centrado alrededor de forward.
    /// </summary>
    public float viewAngle = 30f;

    /// <summary>
    /// Propiedad de solo lectura que nos indica si el jugador se encuentra a la vista
    /// </summary>
    public bool isPlayerInFOV { get; private set; }

    /// <summary>
    /// La velocidad de movimiento del enemigo
    /// </summary>
    public float MoveSpeed = 2;

    public float SteeringDistance = 6f;
    public float WanderStrength = 2f;

    public float MaxHealth = 100;

    public GameObject BloodEffectPrefab;

    public GameObject AmmoPrefab;

    public AudioClip HitSound;

    private float Health;

    /// <summary>
    /// Referencia al gameObject del jugador
    /// </summary>
    private GameObject Player;

    /// <summary>
    /// Referencia al componente RigidBody2D del enemigo
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Referencia al componente animator del enemigo
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Referencia al componente SpriteRenderer del gameObject
    /// </summary>
    private SpriteRenderer spriteRenderer;

    public Vector3 LookDirection { get; private set; }

    private EnemyStateMachine StateMahine;

    private Vector2 lastPosition;

    private bool isMoving = false;
    private Vector3 moveTarget;
    private AudioSource source;

    public delegate void StatusChanged();

    /// <summary>
    /// Evento que se invocará cuando el enemigo muera
    /// </summary>
    public event StatusChanged OnEnemyDead;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        source = gameObject.AddComponent<AudioSource>();
        float vol = ((float)PlayerPrefs.GetInt("VolumeEffect", 100)) / 100f;
        source.volume = vol;
        lastPosition = rb.position;
        LookDirection = -transform.up;
        animator.SetInteger("direction", 1);
        Health = MaxHealth;
        StateMahine = new EnemyStateMachine(this);
        StateMahine.Start();
    }

    // Update is called once per frame
    void Update()
    {

        //actualizamos la percepcion del enemigo
        isPlayerInFOV = checkTargetInFOV(Player);
        if (PlayerInFovIcon != null)
        {
            //mostramos el icono
            PlayerInFovIcon.SetActive(isPlayerInFOV);
        }

        UpdateMovementData();

        StateMahine.CurrentState.Update();
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            HandleMovement();
        }
    }

    private bool checkTargetInFOV(GameObject Target)
    {
        bool isInFOV = false;

        //comprobamos si el objetivo esta dentro de la distancia y del angulo de vision
        Vector3 distanceToTarget = Target.transform.position - transform.position;
        float angle = Vector3.Angle(LookDirection, distanceToTarget);
        if (distanceToTarget.magnitude < ViewDistance && angle < viewAngle / 2)
        {
            //hacemos raycast al objetivo para comprobar si hay obstáculos
            RaycastHit2D raycastHit = Physics2D.Linecast(transform.position, Target.transform.position);

            if (raycastHit.transform.position == Target.transform.position)
            {
                isInFOV = true;
            }
        }
        return isInFOV;
    }


    public void UpdateMovementData()
    {
        Vector2 speed = (rb.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = rb.position;
        bool moving = speed.magnitude >= 0.01;
        animator.SetBool("isMoving", moving);
    }

    public void UpdateMoveDirection(Vector3 dir)
    {
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
        {
            animator.SetInteger("direction", 3);
            if (dir.x > 0)
            {
                LookDirection = transform.right;
                spriteRenderer.flipX = false;
            }
            else
            {
                LookDirection = -transform.right;
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            if (dir.y > 0)
            {
                LookDirection = transform.up;
                animator.SetInteger("direction", 2);
            }
            else
            {
                LookDirection = -transform.up;
                animator.SetInteger("direction", 1);
            }
        }
    }

    public Vector3 getPlayerPosition()
    {
        return Player.transform.position;
    }

    public Vector3 getPlayerVelocity()
    {
        return rb.velocity;
    }

    public void MoveTo(Vector3 position) 
    {
        isMoving = true;
        moveTarget = position;
    }

    public bool TryMoveTo(Vector3 position)
    {
        //Store start position to move from, based on objects current transform position.
        Vector2 start = transform.position;
        //Cast a line from start point to end point checking collision on blockingLayer.
        RaycastHit2D hit = Physics2D.Linecast(start, position);
        //Check if anything was hit
        if (hit.transform == null)
        {
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            MoveTo(position);

            //Return true to say that Move was successful
            return true;
        }
        //If something was hit, return false, Move was unsuccesful.
        return false;
    }

    public void TakeDamage(float damage) 
    {
        Health -= damage;
        Instantiate(BloodEffectPrefab, transform.position, transform.rotation);
        source.PlayOneShot(HitSound);
        if (Health <= 0)
        {

            int r = (new System.Random().Next(0,4));
            if( r == 0)
            {
                Instantiate(AmmoPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            OnEnemyDead?.Invoke();
        }
    }

    public void HandleMovement()
    {
        if (rb.velocity.magnitude >= MoveSpeed) rb.velocity = rb.velocity.normalized * MoveSpeed;

        float sqrRemainingDistance = (transform.position - moveTarget).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        if (sqrRemainingDistance > float.Epsilon)
        {
            //Actualizamos la direccion hacia donde miramos
            UpdateMoveDirection(moveTarget - transform.position);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb.velocity +=   (Vector2 )(moveTarget - transform.position) * Time.deltaTime * MoveSpeed;
        }
        else
        {
            isMoving = false;
        }
    }
}
