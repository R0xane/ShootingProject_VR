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

    [Header("View")]
    public Transform cameraTransform; // Main Camera

    [Header("Animation")]
    public Animator animator;
    public string speedParam = "Speed";
    public float animDampTime = 0.10f;
    public string equippedParam = "IsEquipped";
    public string equipTrigger = "Equip";

    private bool isEquipped;
    [SerializeField] float equipCooldown = 0.15f;
    private float nextEquipTime = 0f;

    [Header("Weapon")]
    public Transform weaponSocket;
    public Transform weapon;
    public bool disableWeaponCollidersWhenEquipped = true;

    public Transform holsterSocket;
    private Transform weaponOriginalParent;

    private CharacterController controller;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (weapon != null)
            weaponOriginalParent = weapon.parent;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && animator != null && Time.time >= nextEquipTime)
        {
            nextEquipTime = Time.time + equipCooldown;
            isEquipped = !isEquipped;

            animator.ResetTrigger(equipTrigger);
            animator.SetBool(equippedParam, isEquipped);
            if (isEquipped) animator.SetTrigger(equipTrigger);

            if (isEquipped) AttachWeaponToSocket();
            else DetachWeapon();
        }

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

        Vector3 moveDir;
        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            moveDir = (camRight * input.x + camForward * input.z);
        }
        else
        {
            moveDir = input;
        }
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        // Gravity
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

    void AttachWeaponToSocket()
    {
        if (weapon == null || weaponSocket == null) return;

        weapon.SetParent(weaponSocket, worldPositionStays: false);
        weapon.localPosition = Vector3.zero;
        weapon.localRotation = Quaternion.identity;
        weapon.localScale = Vector3.one;

        if (disableWeaponCollidersWhenEquipped)
            SetWeaponCollidersEnabled(false);
    }

    void DetachWeapon()
    {
        if (weapon == null) return;

        Transform targetParent = holsterSocket != null ? holsterSocket : weaponOriginalParent;
        if (targetParent != null)
        {
            weapon.SetParent(targetParent, worldPositionStays: false);
            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.identity;
            weapon.localScale = Vector3.one;
        }
        else
        {
            weapon.SetParent(null);
        }

        if (disableWeaponCollidersWhenEquipped)
            SetWeaponCollidersEnabled(true);
    }

    void SetWeaponCollidersEnabled(bool enabled)
    {
        if (weapon == null) return;

        var cols = weapon.GetComponentsInChildren<Collider>(true);
        foreach (var c in cols) c.enabled = enabled;

        var rb = weapon.GetComponentInChildren<Rigidbody>();
        if (rb != null) rb.isKinematic = !enabled;
    }
}
