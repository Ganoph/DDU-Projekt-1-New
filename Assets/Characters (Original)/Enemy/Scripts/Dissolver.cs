using System.Collections;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    [SerializeField] private float dissolveDuration = 2f;
    [SerializeField] private float dissolveStrength;

    private Material[] dissolveMaterials;

    private void Awake()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Collect all materials from all child renderers
        var materials = new System.Collections.Generic.List<Material>();

        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        dissolveMaterials = materials.ToArray();
    }

    // Called by the Animation Event
    public void DissolveEvent()
    {
        StartCoroutine(DissolverCoroutine());
    }

    // Destroy the character after the dissolve finishes
    public GameObject ObjectToDestroy;
    void DestroyFunc()
    {
        Destroy(ObjectToDestroy);
    }

    private IEnumerator DissolverCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;

            dissolveStrength = Mathf.Lerp(0f, 1f, elapsedTime / dissolveDuration);

            foreach (Material material in dissolveMaterials)
            {
                material.SetFloat("_DissolveStrength", dissolveStrength);
            }

            yield return null;
        }

        // Make sure everything ends exactly at 1
        foreach (Material material in dissolveMaterials)
        {
            material.SetFloat("_DissolveStrength", 1f);
        }

        // Destroy the character after the dissolve finishes
        DestroyFunc();
    }
}