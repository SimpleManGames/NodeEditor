using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class BoolNode : BaseInputNode
{
    bool value;

    public BoolNode() {
        WindowTitle = "Bool";
        Resizable = true;
    }

    public override void DrawWindow() {
        base.DrawWindow();

        Event e = Event.current;

        value = GUILayout.Toggle(value, "Toggle");

        nodeResult = value.ToString().ToLower();
    }

    public override void Tick(float deltaTime) { }

    public override void DrawCurves() { }
}
