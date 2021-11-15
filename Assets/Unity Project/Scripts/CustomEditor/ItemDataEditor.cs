using UnityEditor;
using UnityEngine;

namespace Unity_Project.Scripts.CustomEditor
{
    [UnityEditor.CustomEditor(typeof(ItemDataBuilder))]
    public class ItemDataEditor : Editor
    {
        public TextAsset ItemCSV;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ItemDataBuilder itemDataBuilder = (ItemDataBuilder)target;

            if (GUILayout.Button("Create Items from CSV"))
            {
                itemDataBuilder.BuildItemsFromCSV(ItemCSV.text);
            }
        }
    }
}
