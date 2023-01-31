using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class PlayerAgent : Agent
{
    [Range(0f, 20f)]
    public float moveSpeed = 10f;

    [Range(10, 100)]
    public int environmentSize = 10;

    [SerializeField]
    public GoalHandler goalHandler = null;
    [SerializeField]
    public ObstacleHandler obstacleHandler = null;

    [Range(1, 10)]
    public int numberOfGoals = 5;
    [Range(1, 10)]
    public int numberOfObstacles = 5;

    [Range(1, 200)]
    public int episodesBeforeReset = 100;
    private int episodes = 0;




    public override void Initialize()
    {
        base.Initialize();
        goalHandler.InstantiateGoals(numberOfGoals, environmentSize);
        obstacleHandler.InstantiateObstacles(numberOfObstacles, environmentSize);

        goalHandler.RandomizeGoalPositions();
        obstacleHandler.RandomizeObstaclePositions();
    }

    public override void OnEpisodeBegin()
    {

        Debug.Log("New episode");

        episodes++;
        if(episodes == episodesBeforeReset)
        {
            goalHandler.RandomizeGoalPositions();
            obstacleHandler.RandomizeObstaclePositions();
            episodes= 0;
        }

        transform.localPosition = new Vector3(0f, 2f, 0f);
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        var movementVector = new Vector3(moveX, 0f, moveZ);

        transform.localPosition += movementVector * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

        Debug.Log("horizontal: " + continuousActions[0] + " vertical: " + continuousActions[1]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-100f);
            EndEpisode();
        } else if(other.gameObject.CompareTag("Goal"))
        {
            AddReward(10f);
            goalHandler.ResetGoal(other.transform);
        }
    }



}
