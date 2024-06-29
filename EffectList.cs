using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    [System.Serializable]
    public class EffectList
    {
        [SerializeReference] private List<Effect> effects = new List<Effect>();

        private EffectOrigin origin;
        private Target target;

        public EffectList Clone()
        {
            EffectList clonedList = new EffectList();

            clonedList.effects = effects;
            clonedList.origin = origin;
            clonedList.target = target;

            return clonedList;
        }

        public void Add(Effect effect)
        {
            effects.Add(effect);
        }

        public void Remove()
        {
             
        }

        public void CacheOriginAndTarget(EffectOrigin origin, Target target)
        {
            this.origin = origin;
            this.target = target;
        }

        public void ApplyEffects(EffectOrigin origin, Target target)
        {
            foreach (Effect effect in effects)
            {
                effect.Apply(origin, target);
            }
        }

        public void ApplyEffectsCached()
        {
            ApplyEffects(origin, target);
        }
    }
}
