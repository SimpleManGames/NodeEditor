using UnityEditor;
using System;
using UnityEngine;

public class FixedWidthLabel : IDisposable {

    private readonly ZeroIndent indentReset; //helper class to reset and restore indentation

    public FixedWidthLabel(GUIContent label)//	constructor.
    {//						state changes are applied here.
        EditorGUILayout.BeginHorizontal();// create a new horizontal group
        EditorGUILayout.LabelField(label,
            GUILayout.Width(GUI.skin.label.CalcSize(label).x +// actual label width
                9 * EditorGUI.indentLevel));//indentation from the left side. It's 9 pixels per indent level

        indentReset = new ZeroIndent();//helper class to have no indentation after the label
    }

    public FixedWidthLabel(string label)
        : this(new GUIContent(label))//alternative constructor, if we don't want to deal with GUIContents
    {
    }

    public void Dispose() //restore GUI state
    {
        indentReset.Dispose();//restore indentation
        EditorGUILayout.EndHorizontal();//finish horizontal group
    }
}
