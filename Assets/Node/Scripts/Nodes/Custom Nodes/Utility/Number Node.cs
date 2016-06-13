using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class NumberNode : BaseInputNode {

    private ValueType valueType;
    public enum ValueType {
        Int,
        Float
    }

    private string inputValue = "";

    public NumberNode() {
        WindowTitle = "Number Node";
        Resizable = true;
    }

    public override void DrawWindow() {
        base.DrawWindow();

        valueType = (ValueType)EditorGUILayout.EnumPopup("Number type: ", valueType);

        if(valueType == ValueType.Int) {
            inputValue = EditorGUILayout.TextField("Value: ", inputValue);
            float parsed = 0;
            float.TryParse(inputValue, out parsed);
            Mathf.RoundToInt(parsed);
        } else {
            inputValue = EditorGUILayout.TextField("Value: ", inputValue);
        }
    }

    public override void DrawCurves() { }

    public override void Tick(float deltaTime) { }
}
