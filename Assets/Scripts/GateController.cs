using UnityEngine;

public class GateController : MonoBehaviour
{
    [SerializeField] private int cristalsToActivate;
    [SerializeField] private Material inactivatedMaterial;
    [SerializeField] private Material activatedMaterial;
    [SerializeField] private MeshRenderer meshRendererLOD0;
    [SerializeField] private MeshRenderer meshRendererLOD1;
    [SerializeField] private BoxCollider boxCollider;

    private void Start()
    {
        meshRendererLOD0.material = inactivatedMaterial;
        meshRendererLOD1.material = inactivatedMaterial;
        boxCollider.isTrigger = false;
    }

    private void OnEnable()
    {
        PlayerController.OnCristalCollected += OnCristalCollected;
    }

    private void OnDisable()
    {
        PlayerController.OnCristalCollected -= OnCristalCollected;
    }
    
    private void OnCristalCollected(int cristalAmount)
    {
        if (cristalAmount >= cristalsToActivate)
        {
            meshRendererLOD0.material = activatedMaterial;
            meshRendererLOD1.material = activatedMaterial;
            boxCollider.isTrigger = true;
        }
    }
}
