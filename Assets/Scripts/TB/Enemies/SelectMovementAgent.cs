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
        sensor.AddObservation((int)enemy.CharacterData.Element);  // Elemento del enemigo
        sensor.AddObservation(enemy.Level / 10);
        sensor.AddObservation(enemy.CurrentHP / 100);
        sensor.AddObservation(enemy.Attack / 100);
        sensor.AddObservation(enemy.Defense / 100);
        sensor.AddObservation(enemy.Speed / 100);
        sensor.AddObservation(enemy.Statuses.Count / 10);  // Número de estados actuales
        foreach (Condition status in enemy.Statuses)
        {
            sensor.AddObservation((int)status.ID / 10);  // ID del estado 
        }

        // Añadir información sobre los movimientos disponibles
        foreach (TBMove move in enemy.Moves)
        {
            sensor.AddObservation(move.MoveData.Power / 100);
            sensor.AddObservation((int)move.MoveData.Element / 10);
            sensor.AddObservation(move.MoveData.CriticalChance / 100);
        }

        // Observaciones del jugador (oponente del enemigo)
        var player = playerUnit.Character;
        sensor.AddObservation((int)PlayerStats.Element);
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

        // Añadir información sobre los movimientos del jugador
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
        int maxChecks = 100, currentCheck = 0;
        // Verificar si enemyUnit y playerUnit están asignados
        if (enemyUnit == null || playerUnit == null || enemyUnit.Character == null || playerUnit.Character == null)
        {
            Debug.LogError("Cannot get move index because enemyUnit or playerUnit or their Character references are not properly initialized.");
            return -1; // Devuelve un valor por defecto o lanza una excepción
        }

        do
        {
            RequestDecision();
            currentCheck++;
            if (currentCheck >= maxChecks)
            {
                Debug.LogWarning("Reached maximum number of checks. Returning a random move index.");
                selectedMoveIndex = UnityEngine.Random.Range(0, enemyUnit.Character.Moves.Count);
                AddReward(-50f);
                break;
            }
        } while (CheckIfIsACorrectMove() == false);

        Academy.Instance.EnvironmentStep();
        return selectedMoveIndex;
    }

    private bool CheckIfIsACorrectMove()
    {
        TBMove enemyMove = enemyUnit.Character.Moves[selectedMoveIndex];
        float effectiveness = TypeChart.GetEffectiveness(enemyMove.MoveData.Element, playerUnit.Character.CharacterData.Element);
        Debug.Log("Effectiveness: " + effectiveness);

        if (enemyMove.MoveData.Effects.statBoosts.Count > 0)
        {
            var stat = enemyMove.MoveData.Effects.statBoosts[0].stat;
            if (enemyMove.MoveData.Target == MoveTarget.Self)
            {
                if (enemyUnit.Character.Stats[stat] == 5 || enemyUnit.Character.Stats[stat] == 0)
                {
                    AddReward(-10f);
                    return false;
                }
            }
            if (enemyMove.MoveData.Target == MoveTarget.Foe)
            {
                
                if (playerUnit.Character.Stats[stat] == 5 || playerUnit.Character.Stats[stat] == 0)
                {
                    AddReward(-10f);
                    return false;
                }
                if (enemyUnit.Character.Speed > playerUnit.Character.Speed && enemyMove.MoveData.Effects.statBoosts[0].stat == Stat.Speed)
                {
                    AddReward(-10f);
                    return false;
                }
            }
        }
        if (enemyMove.MoveData.Effects.status != ConditionID.None)
        {
            var condition = ConditionsDB.Conditions[enemyMove.MoveData.Effects.status];
            if (enemyMove.MoveData.Target == MoveTarget.Foe)
            {
                if (playerUnit.Character.Statuses.Contains(condition))
                {
                    AddReward(-10f);
                    return false;
                }
            }
        }
        if (playerUnit.Character.Statuses.Count <= 0)
        {
            AddReward(-10f);
            return false;
        }

        if (enemyUnit.Character.Speed < playerUnit.Character.Speed)
        {
            AddReward(-10f);
            return false;
        }
        if (effectiveness <= 0.5f)
        {
            AddReward(-10f);
            return false;
        }
        if (effectiveness >= 1.5f)
        {
            AddReward(60f);
            return true;
        }

        AddReward(50f);
        return true;
    }

    private void AddRewards(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("Player won the battle.");
            AddReward(-10f * playerUnit.Character.CurrentHP);
        }
        else
        {
            Debug.Log("Enemy won the battle.");
            AddReward(10f * enemyUnit.Character.CurrentHP);
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
