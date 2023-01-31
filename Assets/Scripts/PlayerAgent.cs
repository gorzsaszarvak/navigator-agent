using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class PlayerAgent : Agent
{
    private float moveSpeed = 10f;

    private int episodes = 0;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("New episode");

        episodes++;

        transform.localPosition = new Vector3(0f, 2f, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("Action received");
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

}
