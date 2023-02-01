using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalHandler : MonoBehaviour
{

    [SerializeField]
    private Transform goalPrefab = null;

    private List<Transform> goals;

    private int environmentSize;

    public void InstantiateGoals(int numberOfGoals, int environmentSize, Transform parent)
    {
        goals = new List<Transform>();
        for (int i = 0; i < numberOfGoals; i++)
        {
            var goal = Instantiate(goalPrefab, parent);
            goals.Add(goal);
        }
        this.environmentSize= --environmentSize;
    }

    public void RandomizeGoalPositions()
    {
        foreach (var goal in goals)
        {
            RandomGoalPosition(goal);
        }
    }

    public void ResetGoal(Transform goal)
    {
        RandomGoalPosition(goal);
    }

    private void RandomGoalPosition(Transform goal)
    {
        goal.localPosition = new Vector3(
            Random.Range((float)-(environmentSize / 2), (float)(environmentSize / 2)), 
            2f, 
            Random.Range((float)-(environmentSize / 2), (float)(environmentSize / 2)));
    }
}
