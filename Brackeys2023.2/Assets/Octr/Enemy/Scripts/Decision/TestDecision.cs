using JadePhoenix.Tools;
using octr.Enemy;
using UnityEngine;
using UnityEngine.AI;

public class TestDecision : AIDecision
{
    private EnemyAI _enemy;

    protected override void Start()
    {
        _enemy = gameObject.GetComponentInParent<EnemyAI>();
    }

    public override bool Decide()
    {       
        // Access the name of the current state
        string currentStateName = _brain.CurrentState.StateName;

        Debug.Log("Am I walking or idle?");

        // Check the current state and return a decision based on it
        if (currentStateName == "Walk")
        {
            return true;  // Transition to Idle
        }
        else if (currentStateName == "Idle")
        {
            return false; // Transition to Walk
        }

        // Default decision
        return false; // Transition to Walk if state is not found
    }
}