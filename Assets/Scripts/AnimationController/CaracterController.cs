using UnityEngine;
using UnityEngine.Animations.Rigging;

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
    public Transform cameraTransform;

    [Header("Animation")]
    public Animator animator;
    public string speedParam = "Speed";
    public float animDampTime = 0.10f;
    public string equippedParam = "IsEquipped";
    public string equipTrigger = "Equip";

    [Header("Rigging")]
    public TwoBoneIKConstraint rightArmIK;
    public TwoBoneIKConstraint leftArmIK;
    public MultiAimConstraint chestAim;
    [Range(0f, 1f)] public float chestAimWeight = 0.25f;

    [Header("Weapon")]
    public Transform weaponSocket;
    public Transform weapon;
    public bool disableWeaponCollidersWhenEquipped = true;
    public Transform holsterSocket;

    private bool isEquipped = false;
    private bool armsIKEnabled = true;

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

        SetChestAim(chestAimWeight);

        SetArmsIK(1f);
        armsIKEnabled = true;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && animator != null)
        {
            if (!isEquipped)
            {
                EquipNow();
            }
            else
            {
                waitingSecondPressToEquip = false;
                UnequipNow();
            }
        }

        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;

        Vector3 input = Vector3.ClampMagnitude(new Vector3(x, 0f, z), 1f);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 moveDir;
        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward; camForward.y = 0f; camForward.Normalize();
            Vector3 camRight = cameraTransform.right;     camRight.y = 0f;   camRight.Normalize();
            moveDir = (camRight * input.x + camForward * input.z);
        }
        else moveDir = input;

        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
            {
                forward.Normalize();
                Quaternion targetRot = Quaternion.LookRotation(forward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }
        }

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
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

    void EquipNow()
    {
        isEquipped = true;

        animator.SetBool(equippedParam, true);
        animator.ResetTrigger(equipTrigger);
        animator.SetTrigger(equipTrigger);
        animator.Update(0f);

        AttachWeaponToSocket();

        armsIKEnabled = !armsIKEnabled;
        SetArmsIK(armsIKEnabled ? 1f : 0f);

        SetChestAim(chestAimWeight);
    }

    void UnequipNow()
    {
        isEquipped = false;

        animator.SetBool(equippedParam, false);
        animator.ResetTrigger(equipTrigger);
        animator.SetTrigger(equipTrigger);
        animator.Update(0f);

        DetachWeapon();

        armsIKEnabled = !armsIKEnabled;
        SetArmsIK(armsIKEnabled ? 1f : 0f);

        SetChestAim(chestAimWeight);
    }

    void SetArmsIK(float w)
    {
        if (rightArmIK != null) rightArmIK.weight = w;
        if (leftArmIK != null) leftArmIK.weight = w;
    }

    void SetChestAim(float w)
    {
        if (chestAim != null) chestAim.weight = w;
    }

    void AttachWeaponToSocket()
    {
        if (weapon == null || weaponSocket == null) return;

        weapon.SetParent(weaponSocket, false);
        weapon.localPosition = Vector3.zero;
        weapon.localRotation = Quaternion.identity;
        weapon.localScale = Vector3.one;

        if (disableWeaponCollidersWhenEquipped) SetWeaponCollidersEnabled(false);
    }

    void DetachWeapon()
    {
        if (weapon == null) return;

        Transform targetParent = holsterSocket != null ? holsterSocket : weaponOriginalParent;
        if (targetParent != null)
        {
            weapon.SetParent(targetParent, false);
            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.identity;
            weapon.localScale = Vector3.one;
        }
        else weapon.SetParent(null);

        if (disableWeaponCollidersWhenEquipped) SetWeaponCollidersEnabled(true);
    }

    void SetWeaponCollidersEnabled(bool enabled)
    {
        if (weapon == null) return;

        foreach (var c in weapon.GetComponentsInChildren<Collider>(true))
            c.enabled = enabled;

        var rb = weapon.GetComponentInChildren<Rigidbody>();
        if (rb != null) rb.isKinematic = !enabled;
    }
}
