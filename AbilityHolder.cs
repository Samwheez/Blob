using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class AbilityHolder
    {
        private AbilityScriptableObject ability;
        private CasterController caster;

        public AbilityHolder(AbilityScriptableObject ability, CasterController caster)
        {
            this.ability = ability;
            this.caster = caster;

            ability.SubscribeToStateEvents(this);
        }

        private Target target;

        private AbilityState state;

        private float castTimer;
        private float lockoutTimer;
        private float cooldownTimer;

        public void SetTarget(Target target)
        {
            this.target = target;
        }
        public void Use()
        {
            state = AbilityState.Start;
        }

        public bool IsCancelable()
        {
            return ability.Cancelable(state);
        }
        public void Cancel()
        {
            state = AbilityState.Inactive;
            target = null;
            castTimer = 0;
            lockoutTimer = 0;
        }

        public bool CanQueue(AbilityHolder queuedAbility)
        {
            return GetRemainingTime() <= queuedAbility.GetQueueableTime();
        }
        private float GetRemainingTime()
        {
            return castTimer + lockoutTimer;
        }
        private float GetQueueableTime()
        {
            return ability.GetQueueableTime();
        }

        public delegate void OnWindupFinishedDelegate(EffectOrigin origin, Target target);
        public OnWindupFinishedDelegate OnWindupFinished;

        public void Ability_FSM()
        {
            switch (state)
            {
                case AbilityState.Start:

                    if (ability.IsInRange(caster, target))
                    {
                        castTimer = ability.GetWindupTime(caster);
                        state = AbilityState.Windup;
                        goto case AbilityState.Windup;
                    }
                    else
                        caster.SetMovePoint(target.GetPosition());

                    break;

                case AbilityState.Windup:

                    if (castTimer > 0)
                        caster.SetMovePoint(caster.transform.position);
                    else
                    {
                        EffectOrigin origin = new EffectOrigin(caster, caster.transform.position, caster.transform.position);

                        OnWindupFinished?.Invoke(origin, target);

                        lockoutTimer = ability.GetLockoutTime(caster);
                        cooldownTimer = ability.GetCooldownTime(caster);
                        state = AbilityState.Lockout;
                        goto case AbilityState.Lockout;
                    }

                    break;

                case AbilityState.Lockout:

                    if (lockoutTimer > 0)
                        caster.SetMovePoint(caster.transform.position);
                    else
                    {
                        state = AbilityState.Inactive;
                        caster.FinishCurrentCast();
                    }

                    break;
            }
        }

        public void TickTimers()
        {
            TickTimer(ref castTimer);
            TickTimer(ref lockoutTimer);
            TickTimer(ref cooldownTimer);
        }

        private void TickTimer(ref float timer)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else if (timer < 0)
                timer = 0;
        }

        public AbilityTargetType GetTargetType()
        {
            return ability.GetTargetType();
        }

        public bool IsOffCooldown()
        {
            return cooldownTimer <= 0;
        }

    }
    public enum AbilityState
    {
        Inactive,
        Start,
        Windup,
        Lockout
    }
}
