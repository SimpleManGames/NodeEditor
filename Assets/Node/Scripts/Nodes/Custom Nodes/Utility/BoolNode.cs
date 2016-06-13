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

<<<<<<< HEAD
        Event e = Event.current;
=======
        //Event e = Event.current;
>>>>>>> 27624d0721bc4461d34bcd0f50c232b8582de52d

        value = GUILayout.Toggle(value, "Toggle");

        nodeResult = value.ToString().ToLower();
    }

    public override void Tick(float deltaTime) { }

    public override void DrawCurves() { }
}
