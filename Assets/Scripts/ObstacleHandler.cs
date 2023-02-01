using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{

    [SerializeField]
    private Transform obstaclePrefab = null;

    private List<Transform> obstacles;

    private float environmentSize;

    private float noSpawnAreaSize;

    public void InstantiateObstacles(int numberOfObstacles, int environmentSize, int noSpawnAreaSize, Transform parent)
    {
        obstacles = new List<Transform>(numberOfObstacles);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            var obstacle = Instantiate(obstaclePrefab, parent);
            obstacles.Add(obstacle);
        }

        this.environmentSize = (float)--environmentSize;
        this.noSpawnAreaSize = (float)noSpawnAreaSize;
    }

    public void RandomizeObstaclePositions()
    {
        foreach (var obstacle in obstacles)
        {
            RandomObstaclePosition(obstacle);
            RandomRotation(obstacle);
        }
    }

    private void RandomObstaclePosition(Transform obstacle)
    {
        float randomX = RandomNumberSkippingRange(
            -(environmentSize / 2), 
            environmentSize / 2, 
            -noSpawnAreaSize, 
            noSpawnAreaSize);
        float randomZ = RandomNumberSkippingRange(
            -(environmentSize / 2), 
            environmentSize / 2, 
            -noSpawnAreaSize, 
            noSpawnAreaSize);

        obstacle.localPosition = new Vector3(randomX, 2f, randomZ);
    }

    private void RandomRotation(Transform obstacle)
    {
        float randomAngle = Random.Range(0f, 360f);
        obstacle.eulerAngles = new Vector3(obstacle.eulerAngles.x, randomAngle, obstacle.eulerAngles.z);
    }

    private float RandomNumberSkippingRange(float min, float max, float skipMin, float skipMax)
    {
        float number;

        do
        {
            number = Random.Range(min, max);
        } while (number >= skipMin && number <= skipMax);

        return number;
    }
}
