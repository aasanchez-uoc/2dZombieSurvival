using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    /// <summary>
    /// El estado actual en que se encuantra la máquina
    /// </summary>
    public State CurrentState { get; set; }

    /// <summary>
    /// Clase EnemyController del enemigo que usa esta máquina de estados
    /// </summary>
    public EnemyController Enemy;

    /// <summary>
    /// El estado de la FSM en el que el agente persigue al objetivo
    /// </summary>
    private ChaseState chaseState;

    /// <summary>
    /// El estado de la FSM en el que el agente se pasea por el escenario
    /// </summary>
    private WanderState wanderState;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="manager">La clase GameplayManager para comunicarnos con la interfaz del juego</param>
    public EnemyStateMachine(EnemyController enemyController)
    {
        Enemy = enemyController;
        wanderState = new WanderState(this);
        chaseState = new ChaseState(this);
        CurrentState = wanderState;
    }

    /// <summary>
    /// Método que inicia la maquina de estados
    /// </summary>
    public void Start()
    {
        CurrentState.Start();
    }

    /// <summary>
    /// Método encargado de cambiar entre estados
    /// </summary>
    /// <param name="newState">el nuevo estado al que pasamos</param>
    private void ChangeState(State newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        newState.Start();
    }
    
    public void ToChaseState()
    {
        ChangeState(chaseState);
    }

    public void ToWanderState()
    {
        ChangeState(wanderState);
    }
}
