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

        int cells = Mathf.CeilToInt(Mathf.Sqrt(obstacleCount));
        float cellSize = environmentSize / cells;

        int x = 0;
        int z = 0;
        int dx = 0;
        int dz = -1;

        for (int i = 0; i < obstacleCount; i++)
        {
            float randomX = x * cellSize + Random.Range(-cellSize / 2, cellSize / 2);
            float randomZ = z * cellSize + Random.Range(-cellSize / 2, cellSize / 2);

            obstaclePositions.Add(new Vector3(randomX, 2f, randomZ));

            if ((x == z) || (x < 0 && x == -z) || (x > 0 && x == 1 - z))
            {
                int temp = dx;
                dx = -dz;
                dz = temp;
            }

            x += dx;
            z += dz;
        }
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
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0;
        randomDirection = randomDirection.normalized;
        float randomDistance = Random.Range(minDistance, maxDistance);

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
