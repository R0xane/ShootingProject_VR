using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]


public class script_shader : MonoBehaviour
{
    [Header("Renderers à piloter (vide = auto enfants)")]
    [SerializeField] private Renderer[] targetRenderers;

    [Header("Transition (vitesses)")]
    [SerializeField] private float hoverInSpeed = 10f;
    [SerializeField] private float hoverOutSpeed = 12f;
    [SerializeField] private float grabInSpeed  = 14f;
    [SerializeField] private float grabOutSpeed = 16f;

    private float hoverTarget, grabTarget;
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

    private void OnEnable()
    {
        xri.hoverEntered.AddListener(OnHoverEntered);
        xri.hoverExited.AddListener(OnHoverExited);
        xri.selectEntered.AddListener(OnSelectEntered);
        xri.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        xri.hoverEntered.RemoveListener(OnHoverEntered);
        xri.hoverExited.RemoveListener(OnHoverExited);
        xri.selectEntered.RemoveListener(OnSelectEntered);
        xri.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs _) => hoverTarget = 1f;
    private void OnHoverExited(HoverExitEventArgs _)   => hoverTarget = 0f;

    private void OnSelectEntered(SelectEnterEventArgs _) => grabTarget = 1f;
    private void OnSelectExited(SelectExitEventArgs _)   => grabTarget = 0f;

    private void Update()
    {
        float effectiveHoverTarget = Mathf.Max(hoverTarget, grabTarget);

        float hs = (effectiveHoverTarget > hoverValue) ? hoverInSpeed : hoverOutSpeed;
        float gs = (grabTarget > grabValue) ? grabInSpeed : grabOutSpeed;

        hoverValue = Mathf.MoveTowards(hoverValue, effectiveHoverTarget, hs * Time.deltaTime);
        grabValue  = Mathf.MoveTowards(grabValue,  grabTarget,          gs * Time.deltaTime);

        ApplyToAllRenderers(hoverValue, grabValue);
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
