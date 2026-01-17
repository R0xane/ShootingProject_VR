using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CaracterController : MonoBehaviour
{
     [Header("Movement")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5.0f;
    public float turnSpeed = 12f;

    [Header("Gravity")]
    public float gravity = -9.81f;

    [Header("Animation")]
    public Animator animator;
    public string speedParam = "Speed";
    public float animDampTime = 0.10f; 

    private CharacterController controller;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;

        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 moveDir = input; 

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDir * currentSpeed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        if (animator != null)
        {
            float planarSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;
            float normalized = (runSpeed <= 0.0001f) ? 0f : Mathf.Clamp01(planarSpeed / runSpeed);
            animator.SetFloat(speedParam, normalized, animDampTime, Time.deltaTime);
        }
    }
}
