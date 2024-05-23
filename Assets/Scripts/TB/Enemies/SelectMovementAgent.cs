using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class SelectMovementAgent : Agent
{
    [SerializeField] TBCharacterUnit enemyUnit;
    [SerializeField] TBCharacterUnit playerUnit;
    private int selectedMoveIndex;
    public static event Action OnEpisodeBeginAction;

    protected override void OnEnable()
    {
        base.OnEnable(); // Asegúrate de llamar a la base
        BattleSystem.OnBattleEnd += AddRewards;
    }

    protected override void OnDisable()
    {
        base.OnDisable(); // Asegúrate de llamar a la base
        BattleSystem.OnBattleEnd -= AddRewards;
    }

    public override void OnEpisodeBegin()
    {
        OnEpisodeBeginAction?.Invoke();
        
        // Verificar si enemyUnit y playerUnit están asignados
        if (enemyUnit == null || playerUnit == null)
        {
            Debug.LogError("enemyUnit or playerUnit is not assigned.");
            return;
        }

        // Verificar si las referencias a Character están asignadas
        if (enemyUnit.Character == null || playerUnit.Character == null)
        {
            Debug.LogError("Character reference in enemyUnit or playerUnit is null.");
            return;
        }

        Academy.Instance.AutomaticSteppingEnabled = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Verificar si enemyUnit y playerUnit están asignados
        if (enemyUnit == null || playerUnit == null || enemyUnit.Character == null || playerUnit.Character == null)
        {
            Debug.LogWarning("Skipping observations due to null references.");
            return;
        }

        // Observaciones del enemigo (agente controlado por ML-Agents)
        var enemy = enemyUnit.Character;
        sensor.AddObservation(enemy.Level / 10);
        sensor.AddObservation(enemy.CurrentHP / 100);
        sensor.AddObservation(enemy.Attack / 100);
        sensor.AddObservation(enemy.Defense / 100);
        sensor.AddObservation(enemy.Speed / 100);
        sensor.AddObservation(enemy.Statuses.Count / 10);  // Número de estados actuales
        foreach (Condition status in enemy.Statuses)
        {
            sensor.AddObservation((int)status.ID / 10);  // ID del estado (suponiendo que ConditionID es un enum)
        }

        // Añadir información sobre los movimientos disponibles
        sensor.AddObservation(enemy.Moves.Count / 10);
        foreach (TBMove move in enemy.Moves)
        {
            sensor.AddObservation(move.MoveData.Power / 100);
            sensor.AddObservation((int)move.MoveData.Element / 10); // Suponiendo que Element es un enum
            sensor.AddObservation(move.MoveData.CriticalChance / 100);
        }

        // Observaciones del jugador (oponente del enemigo)
        var player = playerUnit.Character;
        sensor.AddObservation(player.Level / 10);
        sensor.AddObservation(player.CurrentHP / 100);
        sensor.AddObservation(player.Attack / 100);
        sensor.AddObservation(player.Defense / 100);
        sensor.AddObservation(player.Speed / 100);
        sensor.AddObservation(player.Statuses.Count / 10);
        foreach (var status in player.Statuses)
        {
            sensor.AddObservation((int)status.ID / 10);
        }

        // Añadir información sobre los movimientos del jugador (si es relevante para la toma de decisiones)
        sensor.AddObservation(player.Moves.Count / 10);
        foreach (var move in player.Moves)
        {
            sensor.AddObservation(move.MoveData.Power / 100);
            sensor.AddObservation((int)move.MoveData.Element / 10);
            sensor.AddObservation(move.MoveData.CriticalChance / 100);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        selectedMoveIndex = actions.DiscreteActions[0];
        Debug.Log("Action Received in method OnActionReceived: " + selectedMoveIndex);
    }

    public int GetMoveIndex()
    {
        // Verificar si enemyUnit y playerUnit están asignados
        if (enemyUnit == null || playerUnit == null || enemyUnit.Character == null || playerUnit.Character == null)
        {
            Debug.LogError("Cannot get move index because enemyUnit or playerUnit or their Character references are not properly initialized.");
            return -1; // Devuelve un valor por defecto o lanza una excepción
        }

        RequestDecision();
        Academy.Instance.EnvironmentStep();
        return selectedMoveIndex;
    }

    private void AddRewards(bool playerWon)
    {
        if (playerWon)
        {
            AddReward(-1f);
        }
        else
        {
            AddReward(1f);
        }

        // Asegurarse de que el episodio puede terminar correctamente
        if (enemyUnit == null || playerUnit == null || enemyUnit.Character == null || playerUnit.Character == null)
        {
            Debug.LogWarning("Cannot end episode properly because enemyUnit or playerUnit or their Character references are not properly initialized.");
            return;
        }

        EndEpisode();
    }
}
