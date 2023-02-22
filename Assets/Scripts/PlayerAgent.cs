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

    private new Rigidbody rigidbody;

    public EnvironmentHandler environmentHandler;

    [Range(1, 200)]
    public int episodesBeforeReset = 100;
    private int episodes = 0;

    // todo: public int health = 100;

    public override void Initialize()
    {
        rigidbody= GetComponent<Rigidbody>();

        environmentHandler = Instantiate(environmentHandler, transform.parent);

        environmentHandler.InstantiateEnvironment();

        environmentHandler.GenerateEnvironment();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("New episode in " + transform.parent.ToString());
        episodes++;

        if(episodes % episodesBeforeReset == 0)
        {
            environmentHandler.GenerateEnvironment();
        }
        environmentHandler.GenerateTarget();
        environmentHandler.ResetEnemies();

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0f, 2f, 0f);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(environmentHandler.targetPosition);

        sensor.AddObservation(rigidbody.velocity.x);
        sensor.AddObservation(rigidbody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Vector3 direction = new Vector3(moveX, 0f, moveZ);

        rigidbody.AddForce(direction * moveSpeed);

        //transform.localPosition += movementVector * Time.deltaTime * moveSpeed;

        float distanceFromTarget = Vector3.Distance(transform.localPosition, environmentHandler.targetPosition);

        float reward = -0.1f - 0.01f * distanceFromTarget / 2;
        AddReward(reward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Enemy"))
        {
            AddReward(-100f);
            EndEpisode();
        } else if(other.gameObject.CompareTag("Target"))
        {
            AddReward(100f);
            EndEpisode();
        }
    }
}
