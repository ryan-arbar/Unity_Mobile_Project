using UnityEngine;

namespace FIMSpace.Basics
{
    public class FBasics_RigidbodyMover : FBasics_RigidbodyMoverBase
    {
        public float MovementSpeed = 4f;
        public float RotationSpeed = 10f;
        public float JumpPower = 7f;
        [Tooltip("Use keyboard keys movement implementation for quick debugging?")]
        public bool WSADMovement = true;

        [Range(0f, 0.5f)]
        [Tooltip("How slow accelerate/decelerate should be")]
        public float accelerationTime = 0.1f;
        [Tooltip("Always rotate head towards movement direction")]
        public bool RotateInDir = true;

        private float offsetRotY = 0f;
        private Vector3 moveDir = Vector3.zero;
        private Vector3 targetRot;

        protected override void Start()
        {
            base.Start();

            targetRot = transform.rotation.eulerAngles;
        }

        protected virtual void Update()
        {
            UpdateMotor();
        }


        protected override void UpdateMotor()
        {
            moveSpeed = MovementSpeed;

            moveDir = Vector3.zero;
            offsetRotY = 0f;


            if (WSADMovement)
            {
                // Move forward / back
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    moveDir += Vector3.forward;
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    moveDir += Vector3.back;

                // Move left / right
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    moveDir += Vector3.left;
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    moveDir += Vector3.right;

                // Debug sprint
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) moveSpeed *= 1.5f;


                // Triggering jump to be executed in next fixed update
                if (isGrounded) if (Input.GetKeyDown(KeyCode.Space)) triggerJumping = JumpPower;
            }


            // Defining rotation for object
            if (moveDir != Vector3.zero)
            {
                moveDir.Normalize();

                if (RotateInDir)
                {
                    targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.TransformDirection(moveDir), Vector3.up)).eulerAngles;
                    moveDir = Vector3.forward;
                }
                else
                    targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up)).eulerAngles;

                targetRot.y += offsetRotY;
            }


            // Calculating smooth acceleration value to be used in next fixed update frame
            smoothedAcceleration = Vector3.SmoothDamp(smoothedAcceleration, moveDir, ref veloHelper, accelerationTime, Mathf.Infinity, Time.deltaTime);

            // Calculating smooth rotation to be applied in fixes update
            smoothedRotation = Quaternion.Lerp(rigbody.rotation, Quaternion.Euler(targetRot), Time.deltaTime * RotationSpeed);
        }

    }
}