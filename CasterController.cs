using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BlobBuddies.Runtime
{
    public abstract class CasterController : ActorController
    {
        
        [SerializeField] private List<AbilityScriptableObject> abilityDatas = new List<AbilityScriptableObject>();
        private List<AbilityHolder> abilityHolders = new List<AbilityHolder>();


        private AbilityHolder currentCast;
        private AbilityHolder queuedCast;


        protected void UseAbility(int index, Target target)
        {
            AbilityHolder ability = abilityHolders[index];

            // If the ability is on cooldown, do nothing.
            if (!ability.IsOffCooldown()) return;

            // If you are already casting this ability, do nothing.
            if (ability == currentCast) return;

            // If the target is null and the ability needs a target, do nothing.
            if (target == null && ability.GetTargetType() != AbilityTargetType.None) return;


            // Cancel the current cast or queue this ability
            if (currentCast != null && currentCast.IsCancelable())
            {
                currentCast.Cancel();
                currentCast = null;
            }
            else if (currentCast != null)
            {
                queuedCast = currentCast.CanQueue(ability) ? ability : null;
                if (queuedCast != null) queuedCast.SetTarget(target);
                return;
            }


            // If the current cast was canceled or is already null
            if (currentCast == null)
            {
                ability.SetTarget(target);
                ability.Use();
                currentCast = ability;
                queuedCast = null;
            }

        }

        public void FinishCurrentCast()
        {
            currentCast = queuedCast;
            queuedCast = null;
            if (currentCast != null) currentCast.Use();
        }

        protected AbilityTargetType AbilityTargetTypeByIndex(int index)
        {
            return abilityHolders[index].GetTargetType();
        }

        private void InitializeAbilities()
        {
            foreach (AbilityScriptableObject abilityData in abilityDatas)
            {
                abilityHolders.Add(new AbilityHolder(abilityData, this));
            }
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeAbilities();
        }

        protected override void Update()
        {
            base.Update();
            foreach (AbilityHolder ability in abilityHolders)
            {
                ability.TickTimers();
            }
            if (currentCast != null)
                currentCast.Ability_FSM();
        }


    }

}
