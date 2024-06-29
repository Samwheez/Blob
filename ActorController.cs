using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

namespace BlobBuddies.Runtime
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class ActorController : NetworkBehaviour
    {

        private NavMeshAgent agent;

        [SerializeField] private float baseMaxHealth;
        [SerializeField] private float baseSize;
        [SerializeField] private float baseMoveSpeed;
        [SerializeField] private float baseAttackDamage;
        [SerializeField] private float baseAbilityPower;

        private void InitializeStats()
        {
            maxHealth.Value = baseMaxHealth;
            currentHealth.Value = baseMaxHealth;
            size.Value = baseSize;
            moveSpeed.Value = baseMoveSpeed;
            attackDamage.Value = baseAttackDamage;
            abilityPower.Value = baseAbilityPower;
        }


        [HideInInspector] public NetworkVariable<float> maxHealth = new NetworkVariable<float>();
        [HideInInspector] public NetworkVariable<float> currentHealth = new NetworkVariable<float>();
        [HideInInspector] public NetworkVariable<float> size = new NetworkVariable<float>();
        [HideInInspector] public NetworkVariable<float> moveSpeed = new NetworkVariable<float>();
        [HideInInspector] public NetworkVariable<float> attackDamage = new NetworkVariable<float>();
        [HideInInspector] public NetworkVariable<float> abilityPower = new NetworkVariable<float>();


        
        protected AutoAttackHolder autoAttack;


        private NetworkVariable<Vector3> movePoint = new NetworkVariable<Vector3>();



        private NetworkVariable<Vector3> dashPoint = new NetworkVariable<Vector3>();
        private NetworkVariable<float> dashSpeed = new NetworkVariable<float>();
        private NetworkVariable<bool> isDashing = new NetworkVariable<bool>();
        private EffectList dashEndEffects;




        protected virtual void Awake()
        {
            autoAttack = GetComponent<AutoAttackHolder>();
            agent = GetComponent<NavMeshAgent>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            

            currentHealth.OnValueChanged += CheckDeath;
            size.OnValueChanged += ChangeScale;
            moveSpeed.OnValueChanged += ChangeAgentSpeed;

            
            InitializeStats();

            movePoint.OnValueChanged += SyncMovePointAndAgent;
            isDashing.OnValueChanged += OnDash;
        }


        protected virtual void Update()
        {
            DashUpdate();
        }


        private void CheckDeath(float previous, float value)
        {
            if (value <=0)
                Death();
        }

        private void ChangeScale(float previous, float value)
        {
            transform.localScale = new Vector3(value, value, value);
        }

        private void ChangeAgentSpeed(float previous, float value)
        {
            agent.speed = value;
        }


        public void SetMovePoint(Vector3 point)
        {
            if (!IsServer) return;

            movePoint.Value = point;
        }
        private void SyncMovePointAndAgent(Vector3 previous, Vector3 value)
        {
            agent.destination = movePoint.Value;
        }

        public void DashTo(Vector3 target, float speed, EffectList dashEndEffects)
        {
            if (!IsServer) return;

            isDashing.Value = true;
            dashPoint.Value = target;
            dashSpeed.Value = speed;

            // For use on server
            this.dashEndEffects = dashEndEffects;
        }

        private void OnDash(bool previous, bool value)
        {
            agent.enabled = !value;
        }

        // Needs to set targetpos to closest position on navmesh.  Navmesh.SamplePosition will work
        private void DashUpdate()
        {
            if (isDashing.Value)
            {

                Vector2 curPos = new Vector2(transform.position.x, transform.position.z); 
                Vector2 endPos = new Vector2(dashPoint.Value.x, dashPoint.Value.z); 

                Vector2 targetPos = Vector2.MoveTowards(curPos, endPos, dashSpeed.Value * Time.deltaTime);

                agent.Warp(new Vector3(targetPos.x, transform.position.y, targetPos.y));
                

                if (IsServer && curPos == endPos)
                {   
                    isDashing.Value = false;
                    dashEndEffects.ApplyEffectsCached();
                }
            }
        }


        protected abstract void Death();

        public abstract Team GetTeam();

    }
    public enum Team
    {
        Player,
        Enemy
    }
}
