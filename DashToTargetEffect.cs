using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class DashToTargetEffect : Effect
    {
        [SerializeField] private float speed;

        [SerializeReference] private EffectList endEffects = new EffectList();

        public override void Apply(EffectOrigin origin, Target target)
        {
            EffectList clonedList = endEffects.Clone();
            clonedList.CacheOriginAndTarget(origin, target);

            origin.Actor.DashTo(target.GetPosition(), speed, clonedList);
        }
    }
}
