using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class EnvironmentHandler : MonoBehaviour
{
    [Range(1, 10)]
    public int obstacleCount = 9;
    [Range(1, 10)]
    public int rotatingWallCount = 5;
    [Range(1, 10)]
    public int gasCount = 2;
    [Range(0, 5)]
    public int enemyCount = 0;
    [Range(1, 10)]
    public int minObstacleDistance = 8;
    [Range(50, 100)]
    public int environmentSize = 50;
    [Range(2, 10)]
    public int noSpawnRadius = 5;

    [SerializeField]
    private Transform playgroundPrefab;


    private List<Transform> obstacles;
    public List<Vector3> obstaclePositions;
    private List<Tuple<int, int>> obstacleCoordinates;
    private int obstacleSize;

    [SerializeField]
    private Transform obstaclePrefab;
    [SerializeField]
    private Transform rotatingWallPrefab;
    [SerializeField]
    private Transform gasPrefab;
    private List<Transform> gasTransforms;
    private List<Coroutine> gasCoroutines;



    [SerializeField]
    private Transform targetPrefab;
    private Transform target;
    public Vector3 targetPosition;

    [SerializeField]
    private Transform enemyPrefab;
    private List<Transform> enemies;
    private List<Vector3> enemyPositions;
    private int enemySize;

    private Vector3 center = new Vector3(0f, 2f, 0f);

    public void InstantiateEnvironment()
    {
        Transform playground = Instantiate(playgroundPrefab, transform.parent);

        playground.localScale = new Vector3(environmentSize / 10, 4f, environmentSize / 10);

        this.target = Instantiate(targetPrefab, transform.parent);

        this.obstacles = new List<Transform>(obstacleCount + rotatingWallCount + gasCount);
        this.gasTransforms = new List<Transform>();
        this.gasCoroutines = new List<Coroutine>();
        for (int i = 0; i < obstacleCount; i++)
        {
            var obstacle = Instantiate(obstaclePrefab, transform.parent);
            obstacles.Add(obstacle);
        }
        for (int i = 0; i < rotatingWallCount; i++)
        {
            var rotatingWall = Instantiate(rotatingWallPrefab, transform.parent);
            obstacles.Add(rotatingWall);
        }
        for (int i = 0;i < gasCount; i++)
        {
            var gas = Instantiate(gasPrefab, transform.parent);
            obstacles.Add(gas);
            gasTransforms.Add(gas);
        }
        this.obstaclePositions = new List<Vector3>();
        this.obstacleCoordinates = new List<Tuple<int, int>>();

        this.enemies = new List<Transform>(enemyCount);
        for (int i =0; i < enemyCount; i++)
        {
            var enemy = Instantiate(enemyPrefab, transform.parent);
            enemies.Add(enemy);
        }
        this.enemyPositions = new List<Vector3>(enemyCount);

        this.obstacleSize = (int)obstaclePrefab.transform.lossyScale.x;
        this.enemySize = (int)enemyPrefab.transform.lossyScale.x;
    }

    public void GenerateEnvironment()
    {
        GenerateObstacles();

        GenerateTarget();

        GenerateEnemies();
    }

    public void GenerateTarget()
    {
        RandomTargetPosition(target);
    }

    public void ResetEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            enemies[i].localPosition = enemyPositions[i];
        }
    }

    private void GenerateObstacles()
    {
        GenerateObstaclePositions();
        for (int i = 0; i < obstaclePositions.Count; i++)
        {
            obstacles[i].localPosition = obstaclePositions[i];
        }

        foreach (var coroutine in gasCoroutines) { StopCoroutine(coroutine); }
        gasCoroutines.Clear();
        foreach (var gas in gasTransforms) 
        {
            var runningCoroutine = StartCoroutine(DeactivateObjectCoroutine(gas));
            gasCoroutines.Add(runningCoroutine);
        }
    }

    private void GenerateEnemies()
    {
        GenerateEnemyPositions();
        for (int i = 0; i < enemyCount; i++)
        {
            enemies[i].localPosition = enemyPositions[i];
        }
    }

    private void GenerateObstaclePositions()
    {
        obstaclePositions.Clear();
        GenerateObstacleCoordinates();
        for (int i = 0; i < obstacleCoordinates.Count; i++)
        {
            float x = obstacleCoordinates[i].Item1;
            float z = obstacleCoordinates[i].Item2;

            obstaclePositions.Add(new Vector3(x, 2f, z));
        }
    }

    private void GenerateEnemyPositions()
    {
        enemyPositions.Clear();
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPosition;
            do
            {
                randomPosition = SingleRandomPosition(environmentSize / 3, environmentSize / 2 - enemySize);
            } while (ObstacleTooClose(randomPosition) || TargetTooClose(randomPosition));

            enemyPositions.Add(randomPosition);
        }
    }

    private void RandomTargetPosition(Transform target)
    {
        Vector3 randomTargetPosition;

        do
        {
            randomTargetPosition = SingleRandomPosition(noSpawnRadius, environmentSize/ 2);
        } while(ObstacleTooClose(randomTargetPosition) || EnemyTooClose(randomTargetPosition));

        target.localPosition = randomTargetPosition;
        targetPosition = randomTargetPosition;
    }

    private void GenerateObstacleCoordinates()
    {
        obstacleCoordinates.Clear();

        int cells = environmentSize / obstacleSize;

        int minCoordinate = -cells / 2 + 2;
        int maxCoordinate = cells / 2 - 2;

        while (obstacleCoordinates.Count < obstacleCount + rotatingWallCount + gasCount)
        {
            int randomX = UnityEngine.Random.Range(minCoordinate, maxCoordinate) * obstacleSize;
            int randomZ = UnityEngine.Random.Range(minCoordinate, maxCoordinate) * obstacleSize;
            if ((Math.Abs(randomX) >= noSpawnRadius || Math.Abs(randomZ) >= noSpawnRadius)
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

    private bool TargetTooClose(Vector3 position)
    {
        if(Vector3.Distance(position, targetPosition) < minObstacleDistance)
        {
            return true;
        }
        return false;
    }

    private bool EnemyTooClose(Vector3 position)
    {
        foreach(var enemyPosition in enemyPositions)
        {
            if(Vector3.Distance(position, enemyPosition) < minObstacleDistance)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator DeactivateObjectCoroutine(Transform gasTransform)
    {

        float activationInterval = UnityEngine.Random.Range(2f, 4f);
        float deactivationInterval = UnityEngine.Random.Range(2f, 4f);

        while (true)
        {
            if (gasTransform.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(deactivationInterval);
                gasTransform.gameObject.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(activationInterval);
                gasTransform.gameObject.SetActive(true);
            }
        }
    }
}
