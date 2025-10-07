using System.Collections;
using UnityEngine;

namespace IESB.Plat3D.Controllers
{
    public class SpawnController : MonoBehaviour    
    {
        public GameObject enemyPrefab;
        public int maxAlive = 5; 
        public float spawnCooldown = 2;
        public float spawnRadius = 5;

        private int _currentAmount = 0;

        private void Start()
        {
            StartCoroutine(SpawnEnemy());
        }
        
        private IEnumerator SpawnEnemy()
        {
            while (true)
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