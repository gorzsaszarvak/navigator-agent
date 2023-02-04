using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class EnvironmentHandler : MonoBehaviour
{
    [SerializeField]
    private Transform obstaclePrefab;

    [SerializeField]
    private Transform targetPrefab;

    private List<Transform> obstacles;
    private List<Transform> targets;

    private List<Vector3> obstaclePositions;
    private List<Vector3> targetPositions;

    private int environmentSize;
    private int obstacleCount;
    private int targetCount;
    private int obstacleFreeRadius;
    private int minObstacleDistance;


    public void InstantiateEnvironment(int environmentSize, int targetCount, int obstacleCount, int obstacleFreeRadius, int minObstacleDistance)
    {
        targets = new List<Transform>(targetCount);
        for(int i = 0; i < targetCount; i++)
        {
            var target = Instantiate(targetPrefab, transform.parent);
            targets.Add(target);
        }
        targetPositions= new List<Vector3>(targetCount);

        obstacles = new List<Transform>(obstacleCount);
        for (int i = 0; i < obstacleCount; i++)
        {
            var obstacle = Instantiate(obstaclePrefab, transform.parent);
            obstacles.Add(obstacle);
        }
        obstaclePositions = new List<Vector3>(obstacleCount);

        this.environmentSize= environmentSize;
        this.obstacleCount= obstacleCount;
        this.targetCount= targetCount;
        this.obstacleFreeRadius= obstacleFreeRadius;
        this.minObstacleDistance= minObstacleDistance;
    }

    public void GenerateEnvironment()
    {

        GenerateObstaclePositions();
        for(int i = 0; i < obstacleCount; i++)
        {
            if (Vector3.Distance(obstaclePositions[i], new Vector3(0f, 2f, 0f)) >= obstacleFreeRadius 
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

        GenerateTargets();
    }

    public void GenerateTargets()
    {
        foreach (var target in targets)
        {
            target.gameObject.SetActive(true);
            RandomTargetPosition(target);
        }
    }

    public void ResetTarget(Transform target)
    {
        target.gameObject.SetActive(false);
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
        } while(ObstacleTooClose(randomTargetPosition));

        target.localPosition = randomTargetPosition;
    }

    private Vector3 SingleRandomPosition(float minDistance, float maxDistance)
    {
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0;
        randomDirection = randomDirection.normalized;
        float randomDistance = Random.Range(minDistance, maxDistance);

        return new Vector3(0f, 2f, 0f) + randomDirection * randomDistance;
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
