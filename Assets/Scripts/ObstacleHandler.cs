using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{

    [SerializeField]
    private Transform obstaclePrefab = null;

    private List<Transform> obstacles;

    private int environmentSize;

    public void InstantiateObstacles(int numberOfObstacles, int environmentSize)
    {
        obstacles = new List<Transform>(numberOfObstacles);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            var obstacle = GameObject.Instantiate(obstaclePrefab);
            obstacles.Add(obstacle);
        }

        this.environmentSize = --environmentSize;
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
        obstacle.localPosition = new Vector3(
            Random.Range((float)-(environmentSize / 2), (float)(environmentSize / 2)),
            0f,
            Random.Range((float)-(environmentSize / 2), (float)(environmentSize / 2)));
    }

    private void RandomRotation(Transform obstacle)
    {
        float randomAngle = Random.Range(0f, 360f);
        obstacle.eulerAngles = new Vector3(obstacle.eulerAngles.x, randomAngle, obstacle.eulerAngles.z);
    }
}
