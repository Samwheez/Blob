using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    public abstract class Target
    {
        public abstract Vector3 GetPosition();

        public void ApplyEffect(EffectOrigin origin, Effect effect)
        {
            effect.Apply(origin, this);
        }


    }
}
