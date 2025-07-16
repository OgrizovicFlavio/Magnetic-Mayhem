using System.Collections;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    private Renderer[] renderers;
    private Color[] originalEmissionColors;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        originalEmissionColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_EmissionColor"))
                originalEmissionColors[i] = renderers[i].material.GetColor("_EmissionColor");
        }
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        foreach (var rend in renderers)
        {
            if (rend.material.HasProperty("_EmissionColor"))
                rend.material.SetColor("_EmissionColor", flashColor);
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_EmissionColor"))
                renderers[i].material.SetColor("_EmissionColor", originalEmissionColors[i]);
        }
    }
}
