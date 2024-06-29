using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class TargetActor : Target
    {
        public ActorController Actor {get; private set;}

        public TargetActor(ActorController target)
        {
            Actor = target;
        }
        public override Vector3 GetPosition()
        {
            return Actor.transform.position;
        }
    }

}
