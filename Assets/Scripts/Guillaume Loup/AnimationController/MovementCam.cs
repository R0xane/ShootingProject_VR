using UnityEngine;

public class MovementCam : MonoBehaviour
{
    [Header("Mouse Look")]
    public float sensitivity = 2.0f;
    public float maxPitch = 85f;

    private float pitch = 0f;
    private float yaw = 0f;

    [Header("Simulated Targets (C1)")]
    public Transform T_Head;
    public Transform T_RightHand;
    public Transform T_LeftHand;

    [Header("Hand offsets (camera space, meters)")]
    public Vector3 rightHandOffset = new Vector3(0.25f, -0.15f, 0.55f);
    public Vector3 leftHandOffset  = new Vector3(-0.25f, -0.15f, 0.55f);

    [Header("Damping (anti-jitter)")]
    [Tooltip("0 = pas de lissage. 10-20 = bien lissé.")]
    public float handPosDamp = 15f;
    public float handRotDamp = 15f;

    private Vector3 _rVel, _lVel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 e = transform.eulerAngles;
        yaw = e.y;

        pitch = e.x;
        if (pitch > 180f) pitch -= 360f;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void LateUpdate()
    {

        if (T_Head != null)
        {
            T_Head.position = transform.position;
            T_Head.rotation = transform.rotation;
        }

        UpdateHandTarget(T_RightHand, rightHandOffset, ref _rVel);
        UpdateHandTarget(T_LeftHand, leftHandOffset, ref _lVel);
    }

    private void UpdateHandTarget(Transform target, Vector3 offset, ref Vector3 vel)
    {
        if (target == null) return;

        Vector3 desiredPos = transform.TransformPoint(offset);
        Quaternion desiredRot = transform.rotation;

        if (handPosDamp <= 0f)
        {
            target.position = desiredPos;
        }
        else
        {
            float smoothTime = 1f / Mathf.Max(0.0001f, handPosDamp);
            target.position = Vector3.SmoothDamp(target.position, desiredPos, ref vel, smoothTime);
        }

        if (handRotDamp <= 0f)
        {
            target.rotation = desiredRot;
        }
        else
        {
            float t = 1f - Mathf.Exp(-handRotDamp * Time.deltaTime);
            target.rotation = Quaternion.Slerp(target.rotation, desiredRot, t);
        }
    }
}
