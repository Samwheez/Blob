using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class TargetedProjectile : NetworkBehaviour
    {
        private NetworkVariable<Vector3> targetPos = new NetworkVariable<Vector3>();
        private NetworkVariable<float> speed = new NetworkVariable<float>();

        private NetworkObject networkObject;

        private EffectOrigin origin;
        private EffectList landEffects = new EffectList();
        
        private Target target;

        private bool chase;


        public void Initialize(EffectOrigin origin, EffectList effects, Target target, float speed)
        {

            this.origin = origin;
            landEffects = effects;
            this.target = target;


            TargetActor actor = target as TargetActor;
            if (actor != null) chase = true;


            transform.position = origin.Position;


            networkObject = GetComponent<NetworkObject>();
            networkObject.Spawn(destroyWithScene: true);

            targetPos.Value = target.GetPosition();
            this.speed.Value = speed;

        }

        private void CheckTargetHit()
        {
            float currentDistance = Vector3.Distance(transform.position, target.GetPosition());

            if (currentDistance <= 0)
                HitTarget();
        }

        private void HitTarget()
        {
            landEffects.ApplyEffects(origin, target);

            networkObject.Despawn(destroy: true);
        }

        
        private void Update()
        {
            if (!IsSpawned) return;

            if (chase && IsServer) targetPos.Value = target.GetPosition();

            transform.position = Vector3.MoveTowards(transform.position, targetPos.Value, speed.Value * Time.deltaTime);

            if (IsServer)
                CheckTargetHit();

        }
    }
}
