using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("JU Voxel/Extras/First Person Controller")]
public class BasicFirstPersonCharacter : MonoBehaviour
{
    #region Variables

    Rigidbody rb;

    [JUSubHeader("Movement Settings")]
    public float Velocity = 2.5f;
    public float JumpForce = 7;

    float VelocityMultiplier;
    float InputX;
    float InputZ;

    [JUSubHeader("Camera Settings")]
    public Transform Camera;
    public float RotationVelocity = 150f;

    float RotX, RotY, xt, yt;

    [JUSubHeader("Collision Check")]
    public TriggerCollisionChecker GroundCheck;

    [JUSubHeader("States")]
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool IsJumping;
    [HideInInspector] public bool IsWalking;
    [HideInInspector] public bool IsRunning;

    #endregion
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
    }

    #region MyVoids

    public void Move()
    {
        //Movement Input
        InputX = Input.GetAxis("Horizontal") * Velocity * Time.deltaTime;
        InputZ = Input.GetAxis("Vertical") * Velocity * Time.deltaTime;
        IsRunning = Input.GetKey(KeyCode.LeftShift);

        //Rotation and Camera
        xt = Input.GetAxis("Mouse Y");
        yt = Input.GetAxis("Mouse X");

        RotX -= xt * RotationVelocity * Time.deltaTime;
        RotY += 1.3f * yt * RotationVelocity * Time.deltaTime;
        RotX = Mathf.Clamp(RotX, -89, 89);

        Vector3 EulerRotation = transform.eulerAngles;
        EulerRotation.y = RotY;
        transform.eulerAngles = EulerRotation;

        Vector3 LocalCameraRotation = Camera.localEulerAngles;
        LocalCameraRotation.x = RotX;
        Camera.localEulerAngles = LocalCameraRotation;



        //Movement
        transform.Translate(InputX * VelocityMultiplier, 0, InputZ * VelocityMultiplier);

        IsGrounded = GroundCheck.Collliding;
        if (IsGrounded == true)
        {
            if (Mathf.Abs(InputX) > 0 || Mathf.Abs(InputZ) > 0)
            {
                IsWalking = true;
            }
            else
            {
                IsWalking = false;
            }

            if (IsWalking)
            {
                if (IsRunning)
                {
                    VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 3, 3f * Time.deltaTime);
                }
                else
                {
                    VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 1f, 3f * Time.deltaTime);
                }
            }
            else
            {
                VelocityMultiplier = Mathf.Lerp(VelocityMultiplier, 0, 10 * Time.deltaTime);
            }

            //Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddRelativeForce(0, 50 * JumpForce, 0);
                IsGrounded = false;
                IsJumping = true;
                Invoke("DisableJump", 0.5f);
            }
        }
    }


    #endregion


    #region InvokeEvents

    public void DisableJump()
    {
        IsJumping = false;
    }

    #endregion
}
