using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    
    public class AutoAttackHolder : MonoBehaviour
    {
        [SerializeField] private float windupPercentage;
        [SerializeField] private float range;

        [SerializeField] private EffectList launchEffects;

        protected ActorController attacker;
        protected TargetActor target;

        private void Awake()
        {
            attacker = GetComponent<ActorController>();
        }

        private void Update()
        {
            AttackCycle_FSM();
        }

        public void StartAttackCycle(TargetActor target)
        {
            this.target = target;
        }
        public void ExitAttackCycle()
        {
            target = null;
            state = State.Start;
            if (!IsWindupFinished())
                attackTimer = 0;
        }

        private void Launch()
        {
            EffectOrigin origin = new EffectOrigin(attacker, attacker.transform.position, attacker.transform.position);
            launchEffects.ApplyEffects(origin, target);
        }

        private enum State
        {
            Start,
            Windup,
            Cooldown
        }

        private State state;

        private float attackTimer;

        public void AttackCycle_FSM()
        {
            if (attackTimer > 0)
                attackTimer -= Time.deltaTime;
            else
                attackTimer = 0;

            if (target == null) return;

            switch (state)
            {
                case State.Start:

                    if (IsInRange())
                    {
                        attacker.SetMovePoint(attacker.transform.position);

                        if (attackTimer <= 0)
                        {
                            attackTimer = AttackSpeedToTime();

                            state = State.Windup;
                            goto case State.Windup;
                        }
                    }
                    else
                        attacker.SetMovePoint(target.GetPosition());

                    break;
                case State.Windup:

                    if (IsWindupFinished())
                    {
                        Launch();

                        state = State.Cooldown;
                        goto case State.Cooldown;
                    }

                    break;
                case State.Cooldown:

                    if (attackTimer <= 0)
                    {

                        state = State.Start;
                        goto case State.Start;
                    }

                    break;

            }

        }

        private bool IsInRange()
        {
            float distance = Vector3.Distance(attacker.transform.position, target.GetPosition());
            return distance <= GetEdgeRange();
        }
        private float GetEdgeRange()
        {
            return range + (attacker.size.Value / 2) + (target.Actor.size.Value / 2);
        }

        private float AttackSpeedToTime()
        {
            return 1; // 1 / AttackSpeed
        }

        private bool IsWindupFinished()
        {
            return attackTimer < AttackSpeedToTime() - (AttackSpeedToTime() * windupPercentage);
        }
    }

}
