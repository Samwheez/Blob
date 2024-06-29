using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class EffectOrigin 
    {
        public ActorController Actor {get; private set;}
        public Vector3 ActorPosition {get; private set;}
        public Vector3 Position {get; private set;}

        public EffectOrigin(ActorController actor, Vector3 actorPosition, Vector3 position)
        {
            Actor = actor;
            ActorPosition = actorPosition;
            Position = position;
        }

        

    }
}
