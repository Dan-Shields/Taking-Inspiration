using UnityEngine;

// This script is here simply to loop through all renderers and enable masking if it's an available shader property
public class DarkLightEnabler : MonoBehaviour
{
    void Awake()
    {
        Renderer[] renderers = Object.FindObjectsOfType<Renderer>();

        foreach(Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("EnableDarkLightMask"))
                {
                    material.SetInt("EnableDarkLightMask", 1);
                }
            }
        }
        
    }
}
