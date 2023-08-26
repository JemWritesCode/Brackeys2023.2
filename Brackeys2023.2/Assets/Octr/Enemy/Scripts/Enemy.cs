using UnityEngine;
using UnityEngine.AI;

namespace octr.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        public NavMeshAgent Agent => _agent;
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public Collider DealDamage => _dealDamage;
        public Collider RecieveDamage => _recieveDamage;

        [Header("Stats")]
        [SerializeField] private int _currentHealth = 100;
        [SerializeField] private int _maxHealth = 100;

        [Header("References")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Collider _dealDamage;
        [SerializeField] private Collider _recieveDamage;

        private void OnValidate()
        {
            _agent = gameObject.GetComponent<NavMeshAgent>();
        }
    }
}