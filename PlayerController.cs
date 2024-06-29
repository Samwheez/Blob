using UnityEngine;
using Unity.Netcode;

namespace BlobBuddies.Runtime
{
    public class PlayerController : CasterController
    {
        // Constants
        private readonly LayerMask ActorLayer = 1 << 6;
        private readonly LayerMask EnvironmentLayer = 1 << 7;


        // Technically, the only synced thing across client and server needs to be the player's agent destination, and stopping point for movement.
        // Doing this method means this class needs to be enabled for everyone.

        // NavMeshAgent pathfinding is deterministic for the most part.  Possible floating point precision errors between hardware, but nothing so imprecise as to negate the benefits of syncing only movePoint

        [SerializeField] private InputDirector inputDirector;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner) return;

            CameraController.Instance.SetTarget(transform);

            inputDirector.MoveCommand += MoveCommand;

            inputDirector.HotbarSlot1Used += CastAbility1;
        }


        private void MoveCommand(Vector2 mousePos)
        {
            Ray mouseRay = CameraController.Instance.GetCamera().ScreenPointToRay(mousePos);

            ProcessMoveCommandServerRpc(mouseRay);
        }

        [ServerRpc]
        private void ProcessMoveCommandServerRpc(Ray mouseRay)
        {

            TargetActor targetActor = CreateTargetActor(mouseRay, false);

            if (targetActor != null)
            {
                autoAttack.StartAttackCycle(targetActor);
                return;
            }


            Target targetLocation = CreateTargetLocation(mouseRay);

            if (targetLocation != null)
            {
                autoAttack.ExitAttackCycle();
                SetMovePoint(targetLocation.GetPosition());
                return;
            }

        }

        private void CastAbility1(Vector2 mousePos)
        {
            Ray mouseRay = CameraController.Instance.GetCamera().ScreenPointToRay(mousePos);

            ProcessCastCommandServerRpc(mouseRay, 0);
        }

        [ServerRpc]
        private void ProcessCastCommandServerRpc(Ray mouseRay, int index)
        {
            AbilityTargetType targetType = AbilityTargetTypeByIndex(index);

            Target target = null;

            if (targetType == AbilityTargetType.Actor)
                target = CreateTargetActor(mouseRay, false);
            if (targetType == AbilityTargetType.Location)
                target = CreateTargetLocation(mouseRay);

            UseAbility(index, target);
        }

        private TargetActor CreateTargetActor(Ray mouseRay, bool targetTeammates)
        {
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity, ActorLayer))
            {
                ActorController targetActor = hit.collider.GetComponent<ActorController>();

                if (targetActor == null)
                {
                    Debug.LogError(hit.collider.gameObject + " is in Actor layer without ActorController component.");
                    return null;
                }

                if (!targetTeammates && GetTeam() == targetActor.GetTeam())
                    return null;

                return new TargetActor(targetActor);
            }

            return null;
        }

        private TargetLocation CreateTargetLocation(Ray mouseRay)
        {
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity, EnvironmentLayer))
            {
                return new TargetLocation(hit.point);
            }

            return null;
        }


        protected override void Death()
        {
            Debug.Log("Died");
        }

        public override Team GetTeam()
        {
            return Team.Player;
        }
    }

}
