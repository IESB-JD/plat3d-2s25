using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy : MonoBehaviour
    { 
        public float health = 10f;
        
        private void Start()
        {
            //PlayerController.OnDie += 
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
            }
        }
    }
}