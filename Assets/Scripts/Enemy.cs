using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy : MonoBehaviour
    { 
        List<Enemy> enemies = new List<Enemy>();
        
        public static event Action<Enemy, bool> onDeath;
        public float health = 10f;
        private PlayerController player;
        
        private void Start()
        {
            onDeath += OnDeathHandler;
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }

        private void OnDeathHandler(Enemy obj, bool isBoss)
        {
            
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0f)
            {
                Destroy(gameObject);
                player.AddScore();
                onDeath?.Invoke(this, false);
            }
        }
    }
}