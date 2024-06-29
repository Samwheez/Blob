using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class TargetLocation : Target
    {
        private Vector3 target;

        public TargetLocation(Vector3 target)
        {
            this.target = target;
        }
        
        public override Vector3 GetPosition()
        {
            return target;
        }
    }
}
