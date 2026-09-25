
using System.Collections.Generic;
using UnityEngine;

public class EmissionController : MonoBehaviour
{
    [SerializeField] private float normalEmissionStrength = 1f;

    private class EmissionMaterial
    {
        public Material material;
        public Color emissionColor;

        public EmissionMaterial(Material material)
        {
            this.material = material;

            // Store only the color, removing the original HDR intensity.
            Color color = material.GetColor("_EmissionColor");

            float intensity = Mathf.Max(color.r, color.g, color.b);

            if (intensity > 0f)
                color /= intensity;

            color.a = 1f;

            emissionColor = color;
        }
    }

    private readonly List<EmissionMaterial> emissionMaterials =
        new List<EmissionMaterial>();

    private void Awake()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            foreach (Material material in materials)
            {
                if (material != null &&
                    material.HasProperty("_EmissionColor"))
                {
                    emissionMaterials.Add(new EmissionMaterial(material));
                }
            }
        }
    }

    // Animation Event
    public void SetEmissionStrength(float strength)
    {
        foreach (EmissionMaterial entry in emissionMaterials)
        {
            if (entry.material == null)
                continue;

            entry.material.EnableKeyword("_EMISSION");

            entry.material.SetColor("_EmissionColor", entry.emissionColor * strength);
        }
    }

    private void OnDisable()
    {
        SetEmissionStrength(normalEmissionStrength);
    }
}

