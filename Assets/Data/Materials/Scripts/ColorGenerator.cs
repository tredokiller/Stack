using UnityEngine;

namespace Data.Materials.Scripts
{
    public static class ColorGenerator
    {
        public static Color GetRandomColor()
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            
            return new Color(r, g, b);
        }

        public static Material GetBasedMaterialWithRandomColor(Material basedMaterial)
        {
            Color color = GetRandomColor();
            Material material = new Material(basedMaterial);

            material.color = color;
            return material;
        }
    }
}
