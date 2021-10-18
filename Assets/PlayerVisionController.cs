using UnityEngine;

// This script is here simply to loop through all renderers and enable vision culling if it's an available shader property
public class PlayerVisionController : MonoBehaviour
{
    void Awake()
    {
        Renderer[] renderers = Object.FindObjectsOfType<Renderer>();

        foreach(Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("EnableCulling"))
                {
                    material.SetInt("EnableCulling", 1);
                }
            }
        }
        
    }
}
