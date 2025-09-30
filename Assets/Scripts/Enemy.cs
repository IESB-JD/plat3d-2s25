using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy : MonoBehaviour
    { 
        private int Health = 10;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                TakeDamage(10);
            }
        }

        private void TakeDamage(int dmg)
        {
            Health -= dmg;
            if (Health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}