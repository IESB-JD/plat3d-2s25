using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace IESB.Plat3D.Controllers
{
    public class SpawnController : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public int initialCount = 2;
        public int maxAlive = 5;
        public int targetAlive = 3;
        public float spawnCooldown = 2;
        public float spawnRadius = 5;
        
        public List<GameObject> spawnedEnemies = new List<GameObject>();

        private void Start()
        {
            if (initialCount > 0 && maxAlive > 0)
            {
                int spawnCount = Math.Min(initialCount, maxAlive);
                for (int i = 0; i < spawnCount; i++)
                {
                    SpawnEnemy();
                }
            }

            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnCooldown);
                
                int currentAlive = spawnedEnemies.Count;
                if (currentAlive < targetAlive && currentAlive < maxAlive)
                {
                    SpawnEnemy();
                }
            }
        }

        private void SpawnEnemy()
        {
            Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = transform.position.y;
            var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();
            
            spawnedEnemies.Add(enemyPrefab);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
    }
}