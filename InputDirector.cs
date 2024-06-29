using UnityEngine;
using UnityEngine.InputSystem;

namespace BlobBuddies.Runtime
{
    [CreateAssetMenu(fileName = "InputDirector", menuName = "ScriptableObject/InputDirector")]
    public class InputDirector : ScriptableObject, Controls.IGameActions
    {
        private Controls controls;

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new Controls();
                controls.Game.SetCallbacks(instance: this);
                controls.Game.Enable();

            }
        }

        public delegate void MoveCommandDelegate(Vector2 mousePos);
        public MoveCommandDelegate MoveCommand;
        public delegate void MouseMovedDelegate(Vector2 mousePos);
        public MouseMovedDelegate MouseMoved;
        public delegate void ZoomedDelegate(float zoomInput);
        public ZoomedDelegate Zoomed;
        public delegate void ToggleCameraFollowDelegate();
        public ToggleCameraFollowDelegate ToggleCameraFollow;

        public delegate void Slot1Delegate(Vector2 mousePos);
        public Slot1Delegate HotbarSlot1Used;



        public void OnMoveCommand(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                MoveCommand?.Invoke(GetMousePosition());
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                MouseMoved?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                Zoomed?.Invoke(context.ReadValue<float>());
        }

        public void OnCameraFollow(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                ToggleCameraFollow?.Invoke();
            if (context.phase == InputActionPhase.Canceled)
                ToggleCameraFollow?.Invoke();
        }

        public void OnToggleCameraFollow(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                ToggleCameraFollow?.Invoke();
        }

        public void OnHotbarSlot1(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                HotbarSlot1Used?.Invoke(GetMousePosition());
        }


        public Vector2 GetMousePosition()
        {
            return controls.Game.MousePosition.ReadValue<Vector2>();
        }

    }
}
