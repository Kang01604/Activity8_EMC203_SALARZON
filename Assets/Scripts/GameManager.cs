// formerly ItemSpawner.cs

using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Obstacle obstaclePrefab;
    public PlayerController player;
    
    [Header("Pool Settings")]
    public int initialPoolSize = 20; // Change to increase the starting pool count for obstacles
    private Queue<Obstacle> obstaclePool = new Queue<Obstacle>();

    [Header("Game Settings")]
    public float spawnDistanceZ = 120f; 
    public float speed = 20f; 
    public float spawnInterval = 0.6f;
    private float spawnTimer = 0f;
    private List<Obstacle> activeObstacles = new List<Obstacle>();

    private void Start()
    {
        // Fills the pool immediately when hitting play
        PrewarmPool();
    }

    private void PrewarmPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            Obstacle obs = Instantiate(obstaclePrefab, transform);
            obs.gameObject.SetActive(false);
            obstaclePool.Enqueue(obs);
        }
    }

    private void Update()
    {
        if (player.isDead) return; // FREEZES GAME ON DEATH

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval) { SpawnObstacle(); spawnTimer = 0f; }

        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            Obstacle obs = activeObstacles[i];
            obs.position3D.z -= speed * Time.deltaTime;

            // Collision logic
            if (Mathf.Abs(obs.position3D.z - player.position3D.z) < 1.5f && !obs.hasHitPlayer)
            {
                if (Mathf.Abs(obs.position3D.x - player.position3D.x) < 1f && player.position3D.y < 2f)
                {
                    player.TakeDamage(5f);
                    obs.hasHitPlayer = true;
                }
            }

            // Return to Pool
            if (obs.position3D.z < -10f)
            {
                obs.gameObject.SetActive(false);
                obstaclePool.Enqueue(obs);
                activeObstacles.RemoveAt(i);
            }
        }
        
        activeObstacles.Sort((a, b) => b.position3D.z.CompareTo(a.position3D.z));
        for (int i = 0; i < activeObstacles.Count; i++) activeObstacles[i].transform.SetSiblingIndex(i);
    }

    private void SpawnObstacle()
    {
        Obstacle obs;

        if (obstaclePool.Count > 0)
        {
            obs = obstaclePool.Dequeue();
        }
        else
        {
            // If pool somehow runs out, this creates one more so the game doesn't crash
            obs = Instantiate(obstaclePrefab, transform);
        }

        obs.gameObject.SetActive(true);
        obs.position3D = new Vector3(Random.Range(-1, 2) * player.laneWidth, 0f, spawnDistanceZ);
        obs.ResetObstacle();
        activeObstacles.Add(obs);
    }
}