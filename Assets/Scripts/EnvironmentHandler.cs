using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class EnvironmentHandler : MonoBehaviour
{
    [SerializeField]
    private Transform obstaclePrefab;

    private List<Transform> obstacles;
    private List<Vector3> obstaclePositions;
    private List<Tuple<int, int>> obstacleCoordinates;
    private int obstacleSize;

    [SerializeField]
    private Transform targetPrefab;

    private Transform target;
    public Vector3 targetPosition;

    private int environmentSize;
    private int obstacleCount;
    private int noSpawnRadius;
    private int minObstacleDistance;

    private Vector3 center = new Vector3(0f, 2f, 0f);

    public void InstantiateEnvironment(int environmentSize, int obstacleCount, int noSpawnRadius, int minObstacleDistance)
    {
        target = Instantiate(targetPrefab, transform.parent);

        obstacles = new List<Transform>(obstacleCount);
        for (int i = 0; i < obstacleCount; i++)
        {
            var obstacle = Instantiate(obstaclePrefab, transform.parent);
            obstacles.Add(obstacle);
        }
        obstaclePositions = new List<Vector3>(obstacleCount);
        obstacleCoordinates= new List<Tuple<int, int>>(obstacleCount);

        this.obstacleSize = (int)obstaclePrefab.transform.lossyScale.x;
        this.environmentSize= environmentSize;
        this.obstacleCount= obstacleCount;
        this.noSpawnRadius= noSpawnRadius;
        this.minObstacleDistance= minObstacleDistance;
    }

    public void GenerateEnvironment()
    {
        GenerateObstaclePositions();
        for(int i = 0; i < obstacleCount; i++)
        {
            if (Vector3.Distance(obstaclePositions[i], new Vector3(0f, 2f, 0f)) >= noSpawnRadius 
                || Mathf.Abs(obstaclePositions[i].x) > environmentSize / 2
                || Mathf.Abs(obstaclePositions[i].z) > environmentSize / 2)
            {
                obstacles[i].localPosition = obstaclePositions[i];
                obstacles[i].gameObject.SetActive(true);
            } else
            {
                obstacles[i].gameObject.SetActive(false);
            }
        }

        GenerateTarget();
    }

    public void GenerateTarget()
    {
        RandomTargetPosition(target);
    }

    private void GenerateObstaclePositions()
    {
        obstaclePositions.Clear();
        GenerateObstacleCoordinates();
        for (int i = 0; i < obstacleCount; i++)
        {
            float x = obstacleCoordinates[i].Item1;
            float z = obstacleCoordinates[i].Item2;

            obstaclePositions.Add(new Vector3(x, 2f, z));
            obstacles[i].localPosition = obstaclePositions[i];
        }

    }

    private void GenerateObstacleCoordinates()
    {
        obstacleCoordinates.Clear();

        int cells = environmentSize / obstacleSize;

        int minCoordinate = -cells / 2 + 1;
        int maxCoordinate = cells / 2 - 1;

        while (obstacleCoordinates.Count < obstacleCount)
        {
            int randomX = UnityEngine.Random.Range(minCoordinate, maxCoordinate) * obstacleSize;
            int randomZ = UnityEngine.Random.Range(minCoordinate, maxCoordinate) * obstacleSize;
            if (Math.Abs(randomX) >= noSpawnRadius && Math.Abs(randomZ) >= noSpawnRadius 
                && IsValidCoordinate(randomX, randomZ))
            {
                obstacleCoordinates.Add(new Tuple<int, int>(randomX, randomZ));
            }
        }
    }

    private bool IsValidCoordinate(int x, int z)
    {
        foreach (var coordinate in obstacleCoordinates)
        {
            int xDiff = Math.Abs(coordinate.Item1 - x);
            int zDiff = Math.Abs(coordinate.Item2 - z);
            if (xDiff < minObstacleDistance && zDiff < minObstacleDistance)
            {
                return false;
            }
        }
        return true;
    }

    private void RandomTargetPosition(Transform target)
    {
        Vector3 randomTargetPosition;

        do
        {
            randomTargetPosition = SingleRandomPosition(0, environmentSize/ 2);
        } while(ObstacleTooClose(randomTargetPosition)
            || Vector3.Distance(center, randomTargetPosition) < noSpawnRadius);

        target.localPosition = randomTargetPosition;
        targetPosition = randomTargetPosition;
    }

    private Vector3 SingleRandomPosition(float minDistance, float maxDistance)
    {
        Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
        randomDirection.y = 0;
        randomDirection = randomDirection.normalized;
        float randomDistance = UnityEngine.Random.Range(minDistance, maxDistance);

        return center + randomDirection * randomDistance;
    }

    private bool ObstacleTooClose(Vector3 position)
    {
        foreach(var obstaclePosition in obstaclePositions)
        {
            if (Vector3.Distance(position, obstaclePosition) < minObstacleDistance)
            {
                return true;
            }
        }

        return false;
    }
}
