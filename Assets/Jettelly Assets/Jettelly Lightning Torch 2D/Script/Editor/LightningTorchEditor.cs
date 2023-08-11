using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightningTorchController))]
public class LightningTorchEditor : Editor
{
    LightningTorchController _target;
    private static Vector3[] v;

    private SerializedProperty _propColorBottom;
    private SerializedProperty _propColorMiddle;
    private SerializedProperty _propColorTop;
    private SerializedProperty _propFlameScale;
    private SerializedProperty _propFlameThreshold;
    private SerializedProperty _propFlameSpeed;
    private SerializedProperty _propFlameVOffset;
    private SerializedProperty _propFlameUOffset;
    private SerializedProperty _propFlameVolume;

    private SerializedProperty _propShowVertices;

    private void OnEnable()
    {
        _target = (LightningTorchController) target;
        _target.InitMesh();

        _propColorBottom = serializedObject.FindProperty("ColorBottom");
        _propColorMiddle = serializedObject.FindProperty("ColorMiddle");
        _propColorTop = serializedObject.FindProperty("ColorTop");
        _propFlameScale = serializedObject.FindProperty("FlameScale");
        _propFlameThreshold = serializedObject.FindProperty("FlameThreshold");
        _propFlameSpeed = serializedObject.FindProperty("FlameSpeed");
        _propFlameVolume = serializedObject.FindProperty("FlameVolume");
        _propFlameVOffset = serializedObject.FindProperty("FlameVOffset");
        _propFlameUOffset = serializedObject.FindProperty("FlameUOffset");
        _propShowVertices = serializedObject.FindProperty("ShowVertices");

        _target.SetMeshColor();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(_propShowVertices);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_propColorTop);
        EditorGUILayout.PropertyField(_propColorMiddle);
        EditorGUILayout.PropertyField(_propColorBottom);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_propFlameScale);
        EditorGUILayout.PropertyField(_propFlameThreshold);
        EditorGUILayout.PropertyField(_propFlameSpeed);
        EditorGUILayout.PropertyField(_propFlameVolume);
        EditorGUILayout.PropertyField(_propFlameVOffset);
        EditorGUILayout.PropertyField(_propFlameUOffset);

        if (serializedObject.ApplyModifiedProperties())
        {
            _target.SetMeshColor();
        }
    }

    private void OnSceneGUI()
    {
        _target.SetMeshShape();
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    private static void CustomOnDrawGizmos(LightningTorchController lightningTorch, GizmoType type)
    {
        if (lightningTorch.ShowVertices)
        {
            float r = 0.015f;
            v = new Vector3[lightningTorch.Vertices.Length];
            Gizmos.color = Color.white;

            for (int i = 0; i < lightningTorch.Vertices.Length; i++)
            {
                v[i] = lightningTorch.Vertices[i];
                Gizmos.DrawSphere(v[i], r);
            }

            Handles.color = Color.gray;
            Handles.DrawAAPolyLine(v[0], v[1]);
            Handles.DrawAAPolyLine(v[1], v[4]);
            Handles.DrawAAPolyLine(v[0], v[2]);
            Handles.DrawAAPolyLine(v[2], v[3]);
            Handles.DrawAAPolyLine(v[3], v[5]);
            Handles.DrawAAPolyLine(v[4], v[6]);
            Handles.DrawAAPolyLine(v[5], v[7]);
            Handles.DrawAAPolyLine(v[7], v[9]);
            Handles.DrawAAPolyLine(v[8], v[9]);
            Handles.DrawAAPolyLine(v[6], v[8]);
        }        
    }
}
