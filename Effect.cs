using System;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    [Serializable]
    public abstract class Effect
    {
        public abstract void Apply(EffectOrigin origin, Target target);

    }
}
