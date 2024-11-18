using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace InputHandling
{
    public class InputHandler : IPlayerInputHandler
    {
        private static InputHandler instance;
        public static InputHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputHandler();
                }

                return instance;
            }
        }

        private float horizontal;
        private float vertical;
        private bool fire;
        private bool jump;
        private float aimHorizontal;
        private float aimVertical;
        private bool menu;

        private InputHandler()
        {
        }

        public void SetHorizontal(float value)
        {
            this.horizontal = value;
        }

        public void SetVertical(float value)
        {
            this.vertical = value;
        }

        public void SetFire(bool value)
        {
            this.fire = value;
        }

        public void SetJump(bool value)
        {
            this.jump = value;
        }

        public void SetAimHorizontal(float value)
        {
            this.aimHorizontal = value;
        }

        public void SetAimVertical(float value)
        {
            this.aimVertical = value;
        }

        public void SetMenu(bool value)
        {
            this.menu = value;
        }

        public bool Menu()
        {
            return menu;
        }

        public bool Fire()
        {
            return fire;
        }

        public float AimHorizontal()
        {
            return aimHorizontal;
        }

        public float AimVertical()
        {
            return aimVertical;
        }

        public float MoveHorizontal()
        {
            return horizontal;
        }

        public float MoveVertical()
        {
            return vertical;
        }

        public bool Jump()
        {
            return jump;
        }
    }

}
