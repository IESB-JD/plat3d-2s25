using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy
    { 
        public string Name;
        private int Health;
        public int HealthPublic;
        
        public float GetHealth()
        {
            return Health;
        }
        
        public void SetHealth(int health)
        {
            Health = health;
            Debug.Log(Name + " health set to: " + health);
        }
    }

    public class EnemyController
    {
        private Enemy Orc;

        public void Blah()
        {
            Orc.HealthPublic = 9999999;
            Orc.SetHealth(200);
            Debug.Log(Orc.GetHealth());
        }
    }
}