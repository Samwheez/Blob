using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class PushTargetAwayFromOriginEffect : Effect
    {
        [SerializeField] private float range;
        [SerializeField] private float speed;

        [SerializeReference] private EffectList endEffects = new EffectList();


        // Instead of using DashTo, should use a different method regarding knocks.  Knockbacks and knockups all have a set duration.  If a character is pushed into a wall
        // and the endpoint is not within the navmesh, it should move them to the wall, but keep them knocked up until the duration ends.
        public override void Apply(EffectOrigin origin, Target target)
        {
            TargetActor targetActor = target as TargetActor;
            if (targetActor == null)
            {
                Debug.LogError("Push Target Away From Origin Effect used on a non actor target");
            }

            Vector3 fixedOriginPos = new Vector3();
            fixedOriginPos.Set(origin.Position.x, targetActor.GetPosition().y, origin.Position.z);

            Vector3 targetDir = fixedOriginPos - targetActor.GetPosition();
            targetDir.Normalize();

            targetDir = -targetDir;

            EffectList clonedList = endEffects.Clone();
            clonedList.CacheOriginAndTarget(origin, target);

            Vector3 targetPos = targetActor.GetPosition() + targetDir * range;
            targetActor.Actor.DashTo(targetPos, speed, clonedList);
            
            
        }
    }
}
