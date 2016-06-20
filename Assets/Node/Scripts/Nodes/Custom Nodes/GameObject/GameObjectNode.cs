using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class GameObjectNode : BaseInputNode {

    private GameObject controlledObject;
    private string controlledObjectName = "Defalut";

    public GameObjectNode() {
        WindowTitle = "GameObject Node";
        HasInputs = true;
        Resizable = true;
    }

    public override void DrawWindow() {
        base.DrawWindow();

        //Event e = Event.current;
        if (controlledObject == null) 
            if (GUILayout.Button("Create GameObject"))
                controlledObject = new GameObject();
        
        controlledObject = (GameObject)EditorGUILayout.ObjectField(controlledObject, typeof(GameObject), true);
        if (controlledObject != null)
            controlledObjectName = (string)EditorGUILayout.TextField(controlledObjectName);

        if (GUI.changed)
            controlledObject.name = controlledObjectName;
    }

    public override void Tick(float deltaTime) {

    }
}
