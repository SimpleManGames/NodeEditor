using UnityEditor;
using System;
using UnityEngine;

public class FixedWidthLabel : IDisposable {

    private readonly ZeroIndent indentReset; //helper class to reset and restore indentation
    private static int testValue = 0;

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
    { }

    public void Dispose() //restore GUI state
    {
        indentReset.Dispose();//restore indentation
        EditorGUILayout.EndHorizontal();//finish horizontal group
    }

    // Do not use
    public static void TestLabels() {
        testValue = EditorGUILayout.IntField("This is a label:", testValue);
        testValue = EditorGUILayout.IntField("This is a much longer label:", testValue);
        testValue = EditorGUILayout.IntField("Min width:", testValue, GUILayout.MinWidth(300));
        testValue = EditorGUILayout.IntField("Max width:", testValue, GUILayout.MaxWidth(300));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Horizontal group:");
        testValue = EditorGUILayout.IntField(testValue);
        EditorGUILayout.EndHorizontal();
        using (new FixedWidthLabel("Fixed width (label only):")) {
            testValue = EditorGUILayout.IntField(testValue);
        }
        using (new FixedWidthLabel("This is a longer fixed width label:")) {
            testValue = EditorGUILayout.IntField(testValue);
        }
        using (new FixedWidthLabel("Two vertical input boxes:")) {
            EditorGUILayout.BeginVertical();
            testValue = EditorGUILayout.IntField(testValue);
            testValue = EditorGUILayout.IntField(testValue);
            EditorGUILayout.EndVertical();
        }
        using (new FixedWidthLabel("Two horizontal input boxes:")) {
            testValue = EditorGUILayout.IntField(testValue);
            testValue = Convert.ToInt32(EditorGUILayout.TextField(testValue + ""));
        }
        using (new FixedWidthLabel("Nesting:")) {
            testValue = EditorGUILayout.IntField(testValue);
            using (new FixedWidthLabel("Testing:")) {
                testValue = EditorGUILayout.IntField(testValue);
            }
        }
        using (new FixedWidthLabel("Vertical + nested:")) {
            EditorGUILayout.BeginVertical();
            testValue = EditorGUILayout.IntField(testValue);
            testValue = EditorGUILayout.IntField(testValue);
            using (new FixedWidthLabel("Nesting:")) {
                testValue = EditorGUILayout.IntField(testValue);
                using (new FixedWidthLabel("Testing:")) {
                    testValue = EditorGUILayout.IntField(testValue);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
