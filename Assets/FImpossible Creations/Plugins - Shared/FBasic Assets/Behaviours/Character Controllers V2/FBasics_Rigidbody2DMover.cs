using UnityEngine;

namespace FIMSpace.Basics
{
    public class FBasics_Rigidbody2DMover : FBasics_Rigidbody2DMoverBase
    {
        public float MovementSpeed = 4;
        [Range(0f, 1f)]
        public float SmoothRotation = 0f;
        public float JumpPower = 12f;

        public bool DoubleJump = true;

        [Tooltip("Use keyboard keys movement implementation for quick debugging?")]
        public bool WSADMovement = true;

        [Range(0f, 0.5f)]
        [Tooltip("How slow accelerate/decelerate should be")]
        public float accelerationTime = 0.1f;

        private Vector3 moveDir = Vector3.zero;
        protected Quaternion targetRot;
        private int jumps = 0;

        protected override void UpdateMotor()
        {
            moveSpeed = MovementSpeed;
            moveDir = Vector3.zero;

            if (WSADMovement)
            {
                // Move left / right
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    moveDir = Vector3.right;
                    targetRot = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    moveDir = Vector3.right;
                    targetRot = Quaternion.Euler(0f, 0f, 0f);
                }

                // Debug sprint
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) moveSpeed *= 1.5f;

                // Triggering jump to be executed in next fixed update
                if (jumps < (DoubleJump ? 2 : 1))
                {
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
                        triggerJumping = JumpPower;
                }
            }

            // Calculating smooth acceleration value to be used in next fixed update frame
            smoothedAcceleration = Vector3.SmoothDamp(smoothedAcceleration, moveDir, ref veloHelper, accelerationTime, Mathf.Infinity, Time.deltaTime);

            // Calculating smooth rotation
            if (SmoothRotation > 0f)
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * (1.1f - SmoothRotation) * 60f);
            else
                transform.rotation = targetRot;
        }


        protected override void OnJump()
        {
            jumps++;
        }

        protected override void OnGrounded()
        {
            base.OnGrounded();
            jumps = 0;
        }

    }
}