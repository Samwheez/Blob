using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    [System.Serializable]
    public class SpawnTargetedProjectileEffect : Effect
    {
        [SerializeField] private TargetedProjectile projectilePrefab;
        [SerializeField] private float projectileSpeed;
        
        [SerializeReference] private EffectList landEffects = new EffectList();

        public override void Apply(EffectOrigin origin, Target target)
        {
            TargetedProjectile projectile = GameObject.Instantiate(projectilePrefab);
            
            projectile.Initialize(origin, landEffects, target, projectileSpeed);
        }
    }
}
