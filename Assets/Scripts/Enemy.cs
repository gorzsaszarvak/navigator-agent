using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform playerTransform;
    public float speed = 15f;

    private void Start()
    {
        playerTransform= transform.parent.transform.Find("PlayerAgent").transform;
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            MoveTowardsPlayer();
        }
    }

    private bool CanSeePlayer()
    {
        Ray ray = new Ray(transform.position, playerTransform.position - transform.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == playerTransform)
            {
                return true;
            }
        }
        return false;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 movementVector = (playerTransform.position - transform.position).normalized;

        transform.position += movementVector * speed * Time.deltaTime;
    }
}
