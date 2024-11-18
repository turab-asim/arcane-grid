using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace InputHandling
{
    public class InputReader : MonoBehaviour
    {
        private const float AimAxisThreshold = 0.1f;

        private InputHandler inputHandler = InputHandler.Instance;

        private void LateUpdate()
        {
            inputHandler.SetMenu(false);
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue inputValue)
        {
            var vec = inputValue.Get<Vector2>();
            inputHandler.SetHorizontal(InputWithThreshold(vec.x, AimAxisThreshold));
            inputHandler.SetVertical(InputWithThreshold(vec.y, AimAxisThreshold));
        }

        public void OnFire(InputValue inputValue)
        {
            inputHandler.SetFire(inputValue.isPressed);
        }

        public void OnJump(InputValue inputValue)
        {
            inputHandler.SetJump(inputValue.isPressed);
        }

        public void OnAim(InputValue inputValue)
        {
            var vec = inputValue.Get<Vector2>();
            inputHandler.SetAimHorizontal(InputWithThreshold(vec.x, AimAxisThreshold));
            inputHandler.SetAimVertical(InputWithThreshold(vec.y, AimAxisThreshold));
        }

        public void OnMenu(InputValue inputValue)
        {
            inputHandler.SetMenu(inputValue.isPressed);
        }
#endif

        private float InputWithThreshold(float input, float threshold)
        {
            return Mathf.Abs(input) > threshold ? input : 0;
        }
    }

}
