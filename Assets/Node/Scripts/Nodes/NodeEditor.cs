#define DEBUG

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//inherits from EditorWindow  
public class NodeEditor : EditorWindow
{
    private const int PANSPEED = 5;

    //list that stores our windows
    private List<BaseNode> windows = new List<BaseNode>();

    //variable to store our mousePos
    private Vector2 mousePos;

    //variable to store a selected node
    private BaseNode selectedNode;

    //variable to determine if we are on a transition mode
    private bool makeTransitionMode = false;

    private Texture2D _resizeHandle;
    private GUIContent _icon;
    private bool _nodeOption;
    private int _mainwindowID;

    private int _winMinX, _winMinY;

    private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

    float PanY;
    float PanX;

    private bool scrollWindow = false;

    //In order to be accessible the window from the menue we add a menu item
    [MenuItem("Window/Node Editor")]
    static void ShowEditor() {
        NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();

        editor.stopWatch.Start();
        editor._winMinX = editor._winMinY = 50;
        editor._resizeHandle = AssetDatabase.LoadAssetAtPath("Assets/Node/Textures/PNG/ResizeHandle.png", typeof(Texture2D)) as Texture2D;
        editor._icon = new GUIContent(editor._resizeHandle);
        editor._mainwindowID = GUIUtility.GetControlID(FocusType.Native);
    }

    void Update() {
        long dTime = stopWatch.ElapsedMilliseconds;

        float deltaTime = ((float)dTime) / 1000;

        foreach (BaseNode b in windows) { b.Tick(deltaTime); }

        stopWatch.Reset();
        stopWatch.Start();

        Repaint();
    }

    void OnGUI() {
        //check event
        Event e = Event.current;

        //check mouse position
        mousePos = e.mousePosition;

        if (e.button == 1 && !makeTransitionMode) {
            if (e.type == EventType.MouseDown) {
                bool clickedOnWindow = false;
                int selectedIndex = -1;

                //check to see if he clicked inside a window
                for (int i = 0; i < windows.Count; i++)
                    if (windows[i].WindowRect.Contains(mousePos)) {
                        //if he clicked store the i
                        selectedIndex = i;
                        //we clicked on a window
                        clickedOnWindow = true;
                        //we have a window so we don't need to check for another one
                        break;
                    }


                //if we didn't clicked a window
                if (!clickedOnWindow) {
                    //make a new menu for every different case 
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Utility/Add Input Node"), false, ContextCallback, "inputNode");
                    menu.AddItem(new GUIContent("Utility/Add Output Node"), false, ContextCallback, "outputNode");
                    menu.AddItem(new GUIContent("Math/Add Calculation Node"), false, ContextCallback, "calcNode");
                    menu.AddItem(new GUIContent("Math/Add Comparison Node"), false, ContextCallback, "compNode");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("GameObject/Add GameObject Node"), false, ContextCallback, "goObj");
                    menu.AddItem(new GUIContent("GameObject/Add GameObject Active Node"), false, ContextCallback, "goActive");
                    menu.AddItem(new GUIContent("GameObject/Add GameObject Distance Node"), false, ContextCallback, "goDistance");
                    menu.AddItem(new GUIContent("Utility/Add Timer Node"), false, ContextCallback, "timerNode");
                    menu.AddItem(new GUIContent("Utility/Add Bool Node"), false, ContextCallback, "boolNode");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Clear All Nodes"), false, ContextCallback, "clearAll");
                    menu.AddItem(new GUIContent("Reset"), false, ContextCallback, "reset");

                    menu.ShowAsContext();
                    e.Use();
                } else {
                    //if it clicked on a window add items to make transition or delete node
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");

                    //we use it so that it will show
                    menu.ShowAsContext();
                    //consumes the event
                    e.Use();
                }
            }
        } //if we are in a transition mode and there is a left click
        else if (e.button == 0 && e.type == EventType.MouseDown && makeTransitionMode) {
            bool clickedOnWindow = false;
            int selectedIndex = -1;

            //find which window was clicked
            for (int i = 0; i < windows.Count; i++)
                if (windows[i].WindowRect.Contains(mousePos)) {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }

            //if there is a click on a window and it's not the window that the transition started from
            if (clickedOnWindow && !windows[selectedIndex].Equals(selectedNode)) {
                //call the set Input on the selected window in order to evaluate if the selected node will be used as input
                windows[selectedIndex].SetInput((BaseInputNode)selectedNode, mousePos);
                makeTransitionMode = false;
                selectedNode = null;
            }

            //if we didn't clicked on a window
            if (!clickedOnWindow) {
                //then stop the transition
                makeTransitionMode = false;
                selectedNode = null;
            }

            e.Use();
        }  //if there is a left click and we are not in a transition mode
        else if (e.button == 0 && e.type == EventType.MouseDown && !makeTransitionMode) {
            bool clickedOnWindow = false;
            int selectedIndex = -1;

            //same as above
            for (int i = 0; i < windows.Count; i++)
                if (windows[i].WindowRect.Contains(mousePos)) {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }


            //if we clicked on a window
            if (clickedOnWindow) {
                //evaluate if an input of the node was clicked
                BaseInputNode nodeToChange = windows[selectedIndex].ClickedOnInput(mousePos);

                //if node to change is not null means that an input was clicked
                if (nodeToChange != null) {
                    //so set the node to the transition mode
                    selectedNode = nodeToChange;
                    makeTransitionMode = true;
                }
            }
        }

        //if we are in a transition mode and there is a selected node
        if (makeTransitionMode && selectedNode != null) {
            //draw the curve from the selected node to the mouse position
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);

            DrawNodeCurve(selectedNode.WindowRect, mouseRect);

            Repaint();
        }

        GUI.BeginGroup(new Rect(PanX, PanY, 100000, 100000));

        //draw each curve for every node
        foreach (BaseNode n in windows) { n.DrawCurves(); }

        //draw the actual windows
        BeginWindows();

        for (int i = 0; i < windows.Count; i++) {
            windows[i].WindowRect = GUI.Window(i
                , new Rect(windows[i].WindowRect.x + PanX, windows[i].WindowRect.y + PanY,
                  windows[i].WindowRect.size.x, windows[i].WindowRect.size.y)
                , DrawNodeWindow
                , windows[i].WindowTitle);
        }

        EndWindows();
        GUI.EndGroup();

        //if (e.keyCode == KeyCode.A && e.type == EventType.KeyDown) {
        //    if (scrollWindow == true) { scrollWindow = false; } else {
        //        scrollStartMousePos = e.mousePosition;
        //        scrollWindow = true;
        //    }
        //}

        //if (e.button == 2) {
        //    if (e.type == EventType.MouseDown) {
        //        scrollStartMousePos = e.mousePosition;
        //        scrollWindow = true;
        //    } else if (e.type == EventType.MouseUp) { scrollWindow = false; }
        //}

        //if (scrollWindow) {
        //    mouseDiff = e.mousePosition - scrollStartMousePos;
        //    PanX += mouseDiff.x / 100;
        //    PanY += mouseDiff.y / 100;
        //}

        bool topButton = GUI.RepeatButton(new Rect(position.width / 2 - (position.width / 10) / 2, 5, position.width / 10, 20), "^");
        bool leftButton = GUI.RepeatButton(new Rect(5, position.height / 2 - (position.height / 10) / 2, 20, position.height / 10), "<");
        bool rightButton = GUI.RepeatButton(new Rect(position.width - 25, position.height / 2 - (position.height / 10) / 2, 20, position.height / 10), ">");
        bool bottomButton = GUI.RepeatButton(new Rect(position.width / 2 - (position.width / 10) / 2, position.height - 25, position.width / 10, 20), "v");

        if (topButton) {
            PanY -= PANSPEED;
            Repaint();
        } else if (leftButton) {
            PanX -= PANSPEED;
            Repaint();
        } else if (rightButton) {
            PanX += PANSPEED;
            Repaint();
        } else if (bottomButton) {
            PanY += PANSPEED;
            Repaint();
        } else
            PanX = PanY = 0;

        if ((Event.current.rawType == EventType.MouseUp) && (GUIUtility.hotControl != _mainwindowID)) {
            GUIUtility.hotControl = 0;
        }
    }

    //function that draws the windows
    void DrawNodeWindow(int id) {
        windows[id].DrawWindow();

        float _cornerX = windows[id].WindowRect.width;
        float _cornerY = windows[id].WindowRect.height;

        if (GUIUtility.hotControl == 0) { windows[id].Resizable = false; }

        GUILayout.BeginArea(new Rect(1, _cornerY - 16, _cornerX - 3, 14));
        GUILayout.BeginHorizontal(EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
        GUILayout.FlexibleSpace();

        windows[id].HandleArea = GUILayoutUtility.GetRect(_icon, GUIStyle.none);
        GUI.DrawTexture(new Rect(windows[id].HandleArea.xMin + 3, windows[id].HandleArea.yMin - 3, 20, 20), _resizeHandle);
        if (!windows[id].Resizable && ((Event.current.type == EventType.MouseDown) || (Event.current.type == EventType.MouseDrag))) {
            if (windows[id].HandleArea.Contains(Event.current.mousePosition, true)) {
                windows[id].Resizable = true;
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Native);
            }
        }
        EditorGUIUtility.AddCursorRect(windows[id].HandleArea, MouseCursor.ResizeUpLeft);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (windows[id].Resizable && (Event.current.type == EventType.mouseDrag)) {
            ResizeNode(id, Event.current.delta.x, Event.current.delta.y);
            Repaint();
            Event.current.Use();
        }
        GUI.DragWindow();

    }


    void ResizeNode(int id, float deltaX, float deltaY) {
        float targetWidth = windows[id].WindowRect.width;
        float targetHeight = windows[id].WindowRect.height;

        if ((windows[id].WindowRect.width + deltaX) > _winMinX)
            targetWidth = windows[id].WindowRect.width + deltaX;

        if ((windows[id].WindowRect.height + deltaY) > _winMinY)
            targetHeight = windows[id].WindowRect.height + deltaY;

        windows[id].WindowRect = new Rect(windows[id].WindowRect.position.x, windows[id].WindowRect.position.y,
                                          targetWidth, targetHeight);
    }

    //Is called when a selection from the context menu is made
    void ContextCallback(object obj) {
        //make the passed object to a string
        string clb = obj.ToString();

        //add the node we want
        if (clb.Equals("inputNode")) {
            InputNode inputNode = ScriptableObject.CreateInstance<InputNode>();
            inputNode.WindowRect = new Rect(mousePos.x, mousePos.y, 200, 80);

            windows.Add(inputNode);
        } else if (clb.Equals("outputNode")) {
            OutputNode outputNode = ScriptableObject.CreateInstance<OutputNode>();
            outputNode.WindowRect = new Rect(mousePos.x, mousePos.y, 200, 80);

            windows.Add(outputNode);
        } else if (clb.Equals("calcNode")) {
            CalcNode calcNode = ScriptableObject.CreateInstance<CalcNode>();
            calcNode.WindowRect = new Rect(mousePos.x, mousePos.y, 200, 95);

            windows.Add(calcNode);
        } else if (clb.Equals("compNode")) {
            ComparisonNode compNode = ScriptableObject.CreateInstance<ComparisonNode>();
            compNode.WindowRect = new Rect(mousePos.x, mousePos.y, 200, 95);

            windows.Add(compNode);
        } else if (clb.Equals("goObj")) {
            GameObjectNode goObj = ScriptableObject.CreateInstance<GameObjectNode>();
            goObj.WindowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            windows.Add(goObj);
        } else if (clb.Equals("goActive")) {
            GameObjectActive goNode = ScriptableObject.CreateInstance<GameObjectActive>();
            goNode.WindowRect = new Rect(mousePos.x, mousePos.y, 200, 80);

            windows.Add(goNode);
        } else if (clb.Equals("goDistance")) {
            GameObjectDistance goDistance = ScriptableObject.CreateInstance<GameObjectDistance>();
            goDistance.WindowRect = new Rect(mousePos.x, mousePos.y, 200, 80);

            windows.Add(goDistance);
        } else if (clb.Equals("timerNode")) {
            TimerNode tNode = ScriptableObject.CreateInstance<TimerNode>();
            tNode.WindowRect = new Rect(mousePos.x, mousePos.y, 200, 95);

            windows.Add(tNode);
        } else if (clb.Equals("boolNode")) {
            BoolNode bNode = ScriptableObject.CreateInstance<BoolNode>();
            bNode.WindowRect = new Rect(mousePos.x, mousePos.y, 120, 75);

            windows.Add(bNode);
        } else if (clb.Equals("clearAll")) {
            windows.Clear();

        } else if (clb.Equals("reset")) {
            PanX = PanY = 0;

        } else if (clb.Equals("makeTransition")) { //if it's a transition
            bool clickedOnWindow = false;
            int selectedIndex = -1;
            //find the window that it was clicked
            for (int i = 0; i < windows.Count; i++)
                if (windows[i].WindowRect.Contains(mousePos)) {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }

            //and make it the selected node of the transition
            if (clickedOnWindow) {
                selectedNode = windows[selectedIndex];
                makeTransitionMode = true;
            }

        } else if (clb.Equals("deleteNode")) { //if it's a delete node 
            bool clickedOnWindow = false;
            int selectedIndex = -1;

            //find the selected node
            for (int i = 0; i < windows.Count; i++)
                if (windows[i].WindowRect.Contains(mousePos)) {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }

            if (clickedOnWindow) {
                //delete it from our list
                BaseNode selNode = windows[selectedIndex];
                windows.RemoveAt(selectedIndex);

                //then pass it to all our nodes that is deleted
                foreach (BaseNode n in windows) { n.NodeDeleted(selNode); }
            }
        }

        //we use else if instead of a switch because:
        /*Selecting from a set of multiple cases is faster with if statements than with switch 
		 */
    }

    //draw the node curve from the middle of the start Rect to the middle of the end rect 
    public static void DrawNodeCurve(Rect start, Rect end) {

        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++) {// Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }

}
