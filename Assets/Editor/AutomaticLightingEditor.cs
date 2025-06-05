using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(AutomaticLighting))]
public class AutomaticLightingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        AutomaticLighting lighting = (AutomaticLighting)target;

        GUILayout.Space(10);

        // Generate Button
        if (GUILayout.Button("Generate Light Probes"))
        {
            lighting.Generate();
        }

        // Bake Button
        if (GUILayout.Button("Bake Light Probes"))
        {
            // This triggers Unity's light probe baking
            Lightmapping.BakeAsync();
        }
    }
}
#endif