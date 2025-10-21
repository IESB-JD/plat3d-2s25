using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IESB.Plat3D.Controllers
{
    public class SpawnController : MonoBehaviour    
    {
        public GameObject enemyPrefab;
        public int maxAlive = 5; 
        public float spawnCooldown = 2;
        public float spawnRadius = 5;

        private int _currentAmount = 0;
        
        public bool canSpawn = true;
        
        

        private void Start()
        {
            StartCoroutine(SpawnEnemy());
            
        }

        private void OnEnable()
        {
            PlayerController.OnPlayerDied += OnPlayerDied;
        }

        private void OnDisable()
        {
            PlayerController.OnPlayerDied -= OnPlayerDied;
        }

        private void OnPlayerDied()
        {
            Debug.Log($"Player morreu, parando de criar inimigos");
            canSpawn = false;
        }
        
        private IEnumerator SpawnEnemy()
        {
            while (canSpawn)
            {
                yield return new WaitForSeconds(spawnCooldown);
                if (_currentAmount < maxAlive)
                {
                    Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * spawnRadius);
                    spawnPosition.y = transform.position.y;
                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                    //_currentAmount = _currentAmount + 1;
                    //_currentAmount += 1;
                    _currentAmount++;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
    }
}