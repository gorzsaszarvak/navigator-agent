using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System;

public class PlayerAgent : Agent
{
    [Range(0f, 20f)]
    public float moveSpeed = 10f;

    [Range(10, 100)]
    public int environmentSize = 10;
    [Range(2, 10)]
    public int obstacleFreeRadius = 3;

    [SerializeField]
    public EnvironmentHandler environmentHandler;

    [Range(1, 10)]
    public int targetCount = 5;
    [Range(1, 10)]
    public int obstacleCount = 5;
    [Range(1, 10)]
    public int minObstacleDistance = 5;

    [Range(1, 200)]
    public int episodesBeforeReset = 100;
    private int episodes = 0;

    private int collectedTargets = 0;

    public override void Initialize()
    {
        base.Initialize();

        environmentHandler = Instantiate(environmentHandler, transform.parent);

        environmentHandler.InstantiateEnvironment(environmentSize, targetCount, obstacleCount, obstacleFreeRadius, minObstacleDistance);

        environmentHandler.GenerateEnvironment();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("New episode in " + transform.parent.ToString());
        episodes++;

        if(episodes  == 0)
        {
            environmentHandler.GenerateEnvironment();
        }
        environmentHandler.GenerateTargets();

        transform.localPosition = new Vector3(0f, 2f, 0f);
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetCount);
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
            collectedTargets++;
            if(collectedTargets == targetCount)
            {
                AddReward(100f);
                EndEpisode();
            }

            environmentHandler.ResetTarget(other.transform);
        }
    }
}
