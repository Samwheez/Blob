using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobBuddies.Runtime
{
    public class TestEnemy : ActorController
    {
        public override Team GetTeam()
        {
            return Team.Enemy;
        }

        protected override void Death()
        {
            Debug.Log("Died");
        }
    }
}
