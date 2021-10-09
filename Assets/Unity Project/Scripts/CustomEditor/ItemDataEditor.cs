using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemDataBuilder))]
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
