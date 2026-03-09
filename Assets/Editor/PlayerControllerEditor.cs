using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EllenController))]

public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EllenController controller = (EllenController)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Elle Player",EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        switch (controller.PlayerState)
        {
            case PlayerController.EPlayerState.None:
                GUI.backgroundColor = Color.black;
                break;
            case PlayerController.EPlayerState.Idle:
                GUI.backgroundColor= Color.red;
                break;
            case PlayerController.EPlayerState.Move:
                GUI.backgroundColor= Color.blue;
                break;
            case PlayerController.EPlayerState.Jump:
                GUI.backgroundColor= Color.green;
                break;
        }
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("PlayerState" , controller.PlayerState.ToString() , EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }
    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }
    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }
    private void OnEditorUpdate()
    {
        if(target != null) Repaint();
    }
}
