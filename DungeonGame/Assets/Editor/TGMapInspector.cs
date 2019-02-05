using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TGMap))]
public class TGMapInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Regenerate"))
        {
            TGMap map = (TGMap)target;
            map.BuildMesh();
        }
    }

}
