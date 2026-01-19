using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class ScriptShaderController : MonoBehaviour
{
    [Header("Renderers à piloter (vide = auto enfants)")]
    [SerializeField] private Renderer[] targetRenderers;

    [Header("Distance (Hover proche au sol)")]
    [SerializeField] private Transform player;              
    [SerializeField] private float hoverDistance = 1.5f;    

    [Header("Raycast (optionnel)")]
    [SerializeField] private bool useLineOfSight = true;    
    [SerializeField] private LayerMask obstructionMask = ~0;

    [Header("Socket (Equipped => Grab)")]
    [SerializeField] private Transform socketPoint;        

    [Header("Transition (vitesses)")]
    [SerializeField] private float hoverInSpeed = 10f;
    [SerializeField] private float hoverOutSpeed = 12f;
    [SerializeField] private float grabInSpeed  = 14f;
    [SerializeField] private float grabOutSpeed = 16f;

    private float hoverValue, grabValue;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable xri;
    private MaterialPropertyBlock mpb;

    private static readonly int HoverID = Shader.PropertyToID("_Hover");
    private static readonly int GrabID  = Shader.PropertyToID("_Grab");

    private void Awake()
    {
        xri = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        mpb = new MaterialPropertyBlock();

        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Update()
    {
        bool equippedInSocket = IsInSocket();
        bool grabbedByXR = (xri != null && xri.isSelected);

        float grabTarget = (grabbedByXR || equippedInSocket) ? 1f : 0f;

        bool closeEnough = IsPlayerCloseWithOptionalRaycast();
        float hoverTarget = (closeEnough && grabTarget < 0.5f) ? 1f : 0f;

        float hs = (hoverTarget > hoverValue) ? hoverInSpeed : hoverOutSpeed;
        float gs = (grabTarget  > grabValue) ? grabInSpeed  : grabOutSpeed;

        hoverValue = Mathf.MoveTowards(hoverValue, hoverTarget, hs * Time.deltaTime);
        grabValue  = Mathf.MoveTowards(grabValue,  grabTarget,  gs * Time.deltaTime);

        ApplyToAllRenderers(hoverValue, grabValue);
    }

    private bool IsInSocket()
    {
        if (socketPoint == null) return false;
        return transform == socketPoint || transform.IsChildOf(socketPoint);
    }

    private bool IsPlayerCloseWithOptionalRaycast()
    {
        if (player == null) return false;

        float sqrDist = (player.position - transform.position).sqrMagnitude;
        if (sqrDist > hoverDistance * hoverDistance) return false;

        if (!useLineOfSight) return true;

        Vector3 origin = player.position;
        Vector3 target = transform.position;

        Vector3 dir = target - origin;
        float len = dir.magnitude;
        if (len < 0.001f) return true;
        dir /= len;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, len, obstructionMask, QueryTriggerInteraction.Ignore))
        {
            return (hit.transform == transform || hit.transform.IsChildOf(transform));
        }

        return true;
    }

    private void ApplyToAllRenderers(float hover, float grab)
    {
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            var r = targetRenderers[i];
            if (!r) continue;

            r.GetPropertyBlock(mpb);
            mpb.SetFloat(HoverID, hover);
            mpb.SetFloat(GrabID, grab);
            r.SetPropertyBlock(mpb);
        }
    }
}
