using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{

    [SerializeField]
    private Transform obstaclePrefab = null;

    private List<Transform> obstacles;

    private int environmentSize;

    private int noSpawnAreaSize;

    private int minObstacleDistance;

    public void InstantiateObstacles(int numberOfObstacles, int environmentSize, int noSpawnAreaSize, int minDistanceBetweenObstacles)
    {
        obstacles = new List<Transform>(numberOfObstacles);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            var obstacle = Instantiate(obstaclePrefab, transform.parent);
            obstacles.Add(obstacle);
        }

        this.environmentSize = environmentSize-3;
        this.noSpawnAreaSize = noSpawnAreaSize;
        this.minObstacleDistance = minDistanceBetweenObstacles;
    }

    public void RandomizeObstaclePositions()
    {
        foreach (var obstacle in obstacles)
        {
            RandomObstaclePosition(obstacle);
        }
    }

    private void RandomObstaclePosition(Transform obstacle)
    {
        float randomX;
        float randomZ;
        Vector3 newPosition;

        do {
            randomX = RandomNumberSkippingRange(
                -(environmentSize / 2),
                environmentSize / 2,
                -noSpawnAreaSize,
                noSpawnAreaSize);
            randomZ = RandomNumberSkippingRange(
                -(environmentSize / 2),
                environmentSize / 2,
                -noSpawnAreaSize,
                noSpawnAreaSize);
            newPosition = new Vector3(randomX, 2f, randomZ);
        } while(TooCloseToObstacles(newPosition));

        obstacle.localPosition = newPosition;
    }

    private int RandomNumberSkippingRange(int min, int max, int skipMin, int skipMax)
    {
        int randomNumber;

        do
        {
            randomNumber = Random.Range(min, max);
        } while (randomNumber >= skipMin && randomNumber <= skipMax);

        return randomNumber;
    }

    private bool TooCloseToObstacles(Vector3 newPosition)
    {
        foreach (Transform otherObstacle in obstacles)
        {
            if (Vector3.Distance(newPosition, otherObstacle.localPosition) < minObstacleDistance)
            {
                return true;
            }
        }

        return false;
    }
}
