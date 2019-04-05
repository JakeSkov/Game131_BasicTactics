using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Actor))]
public class Editor_Actor : Editor
{
    bool left;
    bool right;
    bool editPosition = false;
    bool tL = false;
    bool tR = false;
    bool mL = false;
    bool mR = false;
    bool bL = false;
    bool bR = false;

    public override void OnInspectorGUI()
    {
        Actor editActor = target as Actor;

        string actName = editActor.actorName;
        int maxHitPoints = editActor.maxHitPoints; 

        editPosition = EditorGUILayout.Foldout(editPosition, "Position");
        if (editPosition)
        {
            //Left and Right Toggles
            EditorGUILayout.BeginHorizontal();
            left = GUILayout.Toggle(left, "Edit left side of board");
            EditorGUILayout.EndHorizontal();

            #region Position Array
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            tL = GUILayout.Toggle(tL, "top left");
            mL = GUILayout.Toggle(mL, "mid left");
            bL = GUILayout.Toggle(bL, "bot left");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            tR = GUILayout.Toggle(tR, "top right");
            mR = GUILayout.Toggle(mR, "mid right");
            bR = GUILayout.Toggle(bR, "bot right");
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            #endregion

            EditorGUILayout.BeginHorizontal();
            maxHitPoints = EditorGUILayout.IntSlider(maxHitPoints, 1, 10000);

        }

    }
}
