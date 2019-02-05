using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PortalBehaviour))]
public class PortalDebug : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Connection"))
        {
            PortalBehaviour portal = (PortalBehaviour)target;
            portal.PrintTarget();
        }
    }

}
