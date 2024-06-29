using System;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    [Serializable]
    public class DamageEffect : Effect
    {

        [SerializeField] private float baseDamage;
        [SerializeField] private float attackDamageScaling;
        [SerializeField] private float abilityPowerScaling;

        public override void Apply(EffectOrigin origin, Target target)
        {
            TargetActor targetActor = target as TargetActor;
            if (targetActor == null)
            {
                Debug.LogError("Damage effect used a non actor target");
                return;
            }


            float damage = baseDamage;

            damage += origin.Actor.attackDamage.Value * attackDamageScaling;
            damage += origin.Actor.abilityPower.Value * abilityPowerScaling;
            

            targetActor.Actor.currentHealth.Value -= damage;

        }
    }
}
