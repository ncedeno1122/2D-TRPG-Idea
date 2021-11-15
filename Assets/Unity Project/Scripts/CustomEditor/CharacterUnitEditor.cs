using System;
using UnityEditor;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

namespace Unity_Project.Scripts.CustomEditor
{
    [UnityEditor.CustomEditor(typeof(CharacterUnitScript))]
    public class CharacterUnitEditor : Editor
    {
        /*
        private static bool m_ShowUnitStats = true;
        private static bool m_ShowProtoStats = true;
        
        private SerializedProperty m_DataSO;

        private void OnEnable()
        {
            m_DataSO = serializedObject.FindProperty("UnitData");
        }
        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            
            // Custom GUIStyle
            GUIStyle richStyle = new GUIStyle();
            richStyle.richText = true;
            
            var unit = (CharacterUnitScript)target;
            var unitData = unit.UnitData;
            var unitProto = unitData.Prototype;
            
            // Unit Information
            GUILayout.Label($"<color='white'><b>{(unit != null? unitData.Name : "No Unit Data Set")}</b></color>", richStyle);
            EditorGUILayout.PropertyField(m_DataSO, new GUIContent("UnitData"));
            
            // Unit Stat Display
            m_ShowUnitStats = EditorGUILayout.Foldout(m_ShowUnitStats, new GUIContent("Unit Stats"));
            if (m_ShowUnitStats)
            {
                EditorGUI.indentLevel++;
                // Prototype Stat Display
                m_ShowProtoStats = EditorGUILayout.Foldout(m_ShowProtoStats, new GUIContent("Prototype Stats"));
                if (m_ShowProtoStats)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Unit Class", unitProto.name);
                    EditorGUILayout.LabelField("Max HP", unitProto.MaxHP.ToString());
                    EditorGUILayout.LabelField("Phys. Attack", unitProto.PhysicalAttack.ToString());
                    EditorGUILayout.LabelField("Phys. Defense", unitProto.PhysicalDefense.ToString());
                    EditorGUILayout.LabelField("Mag. Attack", unitProto.MagicalAttack.ToString());
                    EditorGUILayout.LabelField("Mag. Defense", unitProto.MagicalDefense.ToString());
                    EditorGUILayout.LabelField("MoveRange", $"{unitProto.MoveRange} tiles");
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            
            //GUILayout.Label("Unit Stat Overrides");
            
            
            
            
            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    */
    }
}