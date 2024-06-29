using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    [CreateAssetMenu(fileName = "NewTargetedAbility", menuName = "Ability/Targeted")]
    public class TargetedAbilitySO : AbilityScriptableObject
    {
        [Space(10)]
        [Header("Targeted Ability")]
        [SerializeField] private EditorAbilityTargetType targetType;
        [SerializeField] protected float range;

        [Space(20)]
        [SerializeField] private EffectList launchEffects;


        public override void SubscribeToStateEvents(AbilityHolder holder)
        {
            holder.OnWindupFinished += launchEffects.ApplyEffects;
        }

        public override bool IsInRange(CasterController caster, Target target)
        {
            float distance = Vector3.Distance(caster.transform.position, target.GetPosition());
            return distance <= range;
        }

        // I'm not making an entire custom editor to remove one dropdown option from an enum.
        private enum EditorAbilityTargetType
        {
            Actor,
            Location
        }

        public override AbilityTargetType GetTargetType()
        {
            return (AbilityTargetType)targetType;
        }

    }
}
