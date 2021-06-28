using UnityEngine;
/// <summary>
/// Esta clase implementa el estado de la FSM en el que el agente pasea por el escenario
/// </summary>
public class WanderState : State
{
    #region Campos Privados
    /// <summary>
    /// Campo privado el en que almacenamos la última posicion establecida como destino
    /// </summary>
    private Vector3 lastDestination;

    private bool lastMovementSucceeded;

    private BoxCollider2D boxCollider;
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="machine">La FSM que controla este estado</param>
    public WanderState(EnemyStateMachine machine) : base(machine) { }
    #endregion

    #region Métodos Públicos

    /// <summary>
    /// La actualización del estado, que contiene toda la lógica para deambular por el escenario  y el cambio de estado
    /// </summary>
    public override void Update()
    {
        //comprobamos si el jugador está a la vista:
        if (stateMachine.Enemy.isPlayerInFOV)
        {
            stateMachine.ToChaseState();
        }
        //si el objetivo no está a la vista, seguimos deambulando
        else
        {
            
            bool obstacle = Physics2D.BoxCastAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f, stateMachine.Enemy.LookDirection, 0.1f).Length >= 1;
            if ((stateMachine.Enemy.transform.position - lastDestination).sqrMagnitude < 0.05 || !lastMovementSucceeded || obstacle)
            {
                moveToRandomPosition();
            }

        }

    }

    public override void Exit()
    {
    }

    public override void Start()
    {
        boxCollider = stateMachine.Enemy.GetComponent<BoxCollider2D>();
        moveToRandomPosition();
    }
    #endregion

    #region Métodos Privados
    /// <summary>
    /// Método que calcula un punto al azar en el círculo con centro en el origen y radio 1 (en el plano XZ)
    /// </summary>
    /// <returns></returns>
    private Vector3 getRandomDirection()
    {
        float randomAngle = Random.value * 2 * Mathf.PI;
        return new Vector3(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle), 0);
    }

    /// <summary>
    /// Método que calcula el siguiente punto de destino del agente
    /// </summary>
    /// <returns></returns>
    private Vector3 calculateDestination()
    {
        Vector3 futureLocation = stateMachine.Enemy.gameObject.transform.position + stateMachine.Enemy.getPlayerVelocity().normalized * stateMachine.Enemy.SteeringDistance;
        return futureLocation + getRandomDirection() * stateMachine.Enemy.WanderStrength;
    }

    private void moveToRandomPosition()
    {
        lastDestination = calculateDestination();
        lastMovementSucceeded = stateMachine.Enemy.TryMoveTo(lastDestination); ;
    }
    #endregion

}
