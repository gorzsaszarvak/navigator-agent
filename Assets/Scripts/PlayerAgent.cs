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

    public HealthBar healthBar;
    public float maxHealth = 100;
    public float currentHealth;
    public float collisionPenalty = 0.01f;
    public float gasPenalty = 0.5f;



    public override void Initialize()
    {
        rigidbody= GetComponent<Rigidbody>();

        environmentHandler = Instantiate(environmentHandler, transform.parent);

        environmentHandler.InstantiateEnvironment();

        environmentHandler.GenerateEnvironment();

        healthBar.SetMaxHealth(maxHealth);
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

        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);

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

        float distanceFromTarget = Vector3.Distance(transform.localPosition, environmentHandler.targetPosition);

        float distancePenalty = -0.1f - 0.01f * distanceFromTarget / 2;
        AddReward(distancePenalty);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(maxHealth);
        } else if(other.gameObject.CompareTag("Obstacle"))
        {
            float speed = rigidbody.velocity.magnitude;
            TakeDamage(speed * collisionPenalty);
        } else if(other.gameObject.CompareTag("Target"))
        {
            AddReward(100f);
            EndEpisode();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Gas"))
        {
            TakeDamage(gasPenalty);
        }
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if(currentHealth <= 0)
        {
            AddReward(-100f);
            EndEpisode();
        }
    }
}
