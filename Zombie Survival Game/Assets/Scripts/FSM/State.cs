using UnityEngine;
/// <summary>
/// Clase abstracta que representa los estados de la FSM
/// </summary>
public abstract class State
{
    /// <summary>
    /// Referencia a la máquina de estados que maneja este estado
    /// </summary>
    protected EnemyStateMachine stateMachine;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="machine">la FSM que maneja este estado</param>
    public State(EnemyStateMachine machine)
    {
        stateMachine = machine;
    }

    /// <summary>
    /// Método de inicialización que se ejecutará cuando se cambie a este estado
    /// </summary>
    public abstract void Start();

    /// <summary>
    /// Método de actualización que se ejecutará en cada fotograma cuando este estado sea el activo
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Método de finalización que se ejecuta cuando se está cambiando a otro estado
    /// </summary>
    public abstract void Exit();

}
