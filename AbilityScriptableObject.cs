using UnityEngine;

namespace BlobBuddies.Runtime
{
    public abstract class AbilityScriptableObject : ScriptableObject
    {
        [Header("Base Ability Stats")]
        [SerializeField] private float baseWindupTime;
        [SerializeField] private float baseLockoutTime;
        [SerializeField] private float baseCooldownTime;
        [Space(10)]
        [SerializeField] private bool canCancelWindupAndLockout;
        [Space(10)]
        [SerializeField] private float queueableTime;


        public abstract void SubscribeToStateEvents(AbilityHolder holder);

        public abstract bool IsInRange(CasterController caster, Target target);

        public float GetWindupTime(CasterController caster)
        {
            return baseWindupTime;
        }
        public float GetLockoutTime(CasterController caster)
        {
            return baseLockoutTime;
        }
        public float GetCooldownTime(CasterController caster)
        {
            return baseCooldownTime;
        }

        public float GetQueueableTime()
        {
            return queueableTime;
        }

        public bool Cancelable(AbilityState state)
        {
            if (state == AbilityState.Windup)
                return canCancelWindupAndLockout;
            if (state == AbilityState.Lockout)
                return canCancelWindupAndLockout;

            return true;
        }

        public abstract AbilityTargetType GetTargetType();

    }
    public enum AbilityTargetType
    {
        Actor,
        Location,
        None
    }
}
