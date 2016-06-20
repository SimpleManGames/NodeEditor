using UnityEngine;
using UnityEditor;
using System.Collections;

public class CalcNode : BaseInputNode {

    //variables for our input nodes
    private BaseInputNode _input1;
    private Rect _input1Rect;

    private BaseInputNode _input2;
    private Rect _input2Rect;

    //the calculation types we want to have
    private CalculationType _calculationType;

    public enum CalculationType {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    //give a title to the window and set it to have input
    public CalcNode() {
        WindowTitle = "Calculation Node";
        HasInputs = true;
        Resizable = true;
    }

    public override void DrawWindow() {
        base.DrawWindow();

        //check for events
        Event e = Event.current;
        //make a popup for the user to select the calculation type
        _calculationType = (CalculationType)EditorGUILayout.EnumPopup("Calculation Type", _calculationType);


        string input1Title = "None";

        //if there is input get the result
        if(_input1) { input1Title = _input1.getResult(); }

        //draw a label
        GUILayout.Label("Input 1: " + input1Title);

        if(e.type == EventType.Repaint) {
            _input1Rect = GUILayoutUtility.GetLastRect();
            _input1Rect.width = 50;
        }

        string input2Title = "None";
        if(_input2) { input2Title = _input2.getResult(); }

        GUILayout.Label("Input 2: " + input2Title);

        if(e.type == EventType.Repaint) {
            _input2Rect = GUILayoutUtility.GetLastRect();
            _input2Rect.width = 50;
        }
    }


    public override void SetInput(BaseInputNode input, Vector2 clickPos) {
        clickPos.x -= WindowRect.x;
        clickPos.y -= WindowRect.y;

        if(_input1Rect.Contains(clickPos)) { _input1 = input; } 
        else if(_input2Rect.Contains(clickPos)) { _input2 = input; }
    }

    public override void DrawCurves() {
        if(_input1) {
            Rect rect = WindowRect;
            rect.x += _input1Rect.x;
            rect.y += _input1Rect.y + _input2Rect.height / 2;
            rect.width = 1;
            rect.height = 1;

            NodeEditor.DrawNodeCurve(_input1.WindowRect, rect);
        }

        if(_input2) {
            Rect rect = WindowRect;
            rect.x += _input2Rect.x;
            rect.y += _input2Rect.y + _input2Rect.height / 2;
            rect.width = 1;
            rect.height = 1;

            NodeEditor.DrawNodeCurve(_input2.WindowRect, rect);
        }
    }

    public override void Tick(float deltaTime) {

        float input1Value = 0;
        float input2Value = 0;

        if(_input1) {
            //get the result from the first input
            string input1Raw = _input1.getResult();
            //try to make it a float
            float.TryParse(input1Raw, out input1Value);
        }

        //same as above
        if(_input2) {
            string input2Raw = _input2.getResult();
            float.TryParse(input2Raw, out input2Value);
        }

        //by default the result is falce
        string result = "false";

        //switch statement for each calculation type
        switch(_calculationType) {
            case CalculationType.Addition: result = ( input1Value + input2Value ).ToString(); break;
            case CalculationType.Division: result = ( input1Value / input2Value ).ToString(); break;
            case CalculationType.Multiplication: result = ( input1Value * input2Value ).ToString(); break;
            case CalculationType.Subtraction: result = ( input1Value - input2Value ).ToString(); break;
        }

        nodeResult = result;
    }

    public override BaseInputNode ClickedOnInput(Vector2 pos) {
        BaseInputNode retVal = null;

        pos.x -= WindowRect.x;
        pos.y -= WindowRect.y;

        if(_input1Rect.Contains(pos)) {
            retVal = _input1;
            _input1 = null;
        } else if(_input2Rect.Contains(pos)) {
            retVal = _input2;
            _input2 = null;
        }

        return retVal;
    }

    public override void NodeDeleted(BaseNode node) {
        if(node.Equals(_input1)) { _input1 = null; }
        if(node.Equals(_input2)) { _input2 = null; }
    }
}
