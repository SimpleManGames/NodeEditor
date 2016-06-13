using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class GameObjectNode : BaseInputNode {

    private GameObject controlledObject;

    

    public GameObjectNode() {
        WindowTitle = "GameObject Node";
        HasInputs = true;
        Resizable = true;
    }

    public override void DrawWindow() {
        base.DrawWindow();

<<<<<<< HEAD
        Event e = Event.current;
=======
        //Event e = Event.current;
>>>>>>> 27624d0721bc4461d34bcd0f50c232b8582de52d
        if (controlledObject == null) 
            if (GUILayout.Button("Create GameObject"))
                controlledObject = new GameObject();
        
        controlledObject = (GameObject)EditorGUILayout.ObjectField(controlledObject, typeof(GameObject), true);


    }

    public override void Tick(float deltaTime) {
        throw new NotImplementedException();
    }
}
