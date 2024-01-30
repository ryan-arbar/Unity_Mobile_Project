using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FC: Base class for rigidbody movement controllers
    /// </summary>
    public abstract class FBasics_RigidbodyMoverBase : MonoBehaviour
    {
        protected float moveSpeed = 5f;
        protected float rotateSpeed = 10f;

        protected Vector3 smoothedAcceleration;
        protected Quaternion smoothedRotation;

        /// <summary> Variable for helping smooth damp </summary>
        protected Vector3 veloHelper = Vector3.zero;

        protected bool isGrounded = false;

        /// <summary> Jump power to be executed in next fixed update frame </summary>
        protected float triggerJumping = 0f;

        /// <summary> Base reference to rigidbody component </summary>
        protected Rigidbody rigbody;
        /// <summary> Base reference to collider component </summary>
        protected Collider charCollider;


        protected virtual void Start()
        {
            rigbody = GetComponentInChildren<Rigidbody>();
            charCollider = GetComponentInChildren<Collider>();

            // Making character slide on colliders when jumping forward vertical walls etc.
            if (charCollider)
                charCollider.material = FEngineering.PMFrict;

            rigbody.interpolation = RigidbodyInterpolation.Interpolate; // Interpolation for smooth motion with rig.velocity and rig.angularVelocity changes
            rigbody.maxAngularVelocity = 100f; // Allowing angular velocity rotate fast towards target rotation

            smoothedAcceleration = Vector3.zero;
            smoothedRotation = transform.rotation;
        }


        protected virtual void UpdateMotor()
        {
            // Example to be overrided ----------------------------------

            Vector3 targetMoveDir = transform.forward;
            Vector3 targetRot = transform.eulerAngles; // target rotation

            float accelerateTime = 0.1f;
            float rotateSpeed = 10f;

            // Calculating smooth acceleration value to be used in next fixed update frame
            smoothedAcceleration = Vector3.SmoothDamp(smoothedAcceleration, targetMoveDir, ref veloHelper, accelerateTime, Mathf.Infinity, Time.deltaTime);

            // Calculating smooth rotation to be applied in fixes update
            smoothedRotation = Quaternion.Lerp(rigbody.rotation, Quaternion.Euler(targetRot), Time.deltaTime * rotateSpeed);
        }



        protected virtual void FixedUpdate()
        {
            Vector3 velocityMemory = rigbody.velocity;

            // Supporting model scale and moving in forward direction with computed acceleration
            Vector3 targetVelo = transform.TransformVector(smoothedAcceleration) * moveSpeed;

            if (triggerJumping != 0f)
            {
                targetVelo.y = triggerJumping;
                rigbody.MovePosition(rigbody.position + transform.up * triggerJumping * 0.05f);
                OnJump();
                triggerJumping = 0f;
                isGrounded = false;
            }
            else targetVelo.y = velocityMemory.y; // Not changing velocity in y only accelerate in x and z


            // Applying sliding material to mover when there is desired movement
            if (!isGrounded || targetVelo.sqrMagnitude > moveSpeed * 0.2f)
                charCollider.material = FEngineering.PMSliding;
            else // When stopped we preventing from sliding on walls
                charCollider.material = FEngineering.PMFrict;


            rigbody.velocity = targetVelo;
            rigbody.angularVelocity = FEngineering.QToAngularVelocity(smoothedRotation * Quaternion.Inverse(transform.rotation)) * rotateSpeed * 10f;
        }



        /// <summary>
        /// Checking grounded state
        /// </summary>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!isGrounded)
                if (collision != null)
                    if (collision.contacts.Length > 0)
                    {
                        for (int i = 0; i < collision.contacts.Length; i++)
                        {
                            float dot = Vector3.Dot(transform.up, collision.contacts[i].normal);
                            // Dot == 1 -> Standing on flat surface
                            // Dot == 0 -> pushing against vertical wall
                            // Dot == 0.5 -> standing on 45 degrees wall

                            if (dot > 0.25f) // Standing max on 22,5 degrees wall
                            {
                                OnGrounded();
                                return;
                            }
                        }
                    }
        }


        /// <summary>
        /// Executed when character triggers jumping
        /// </summary>
        protected virtual void OnJump()
        {
        }

        /// <summary>
        /// To be overrided for particel effects / animations
        /// </summary>
        protected virtual void OnGrounded()
        {
            isGrounded = true;
        }

    }
}