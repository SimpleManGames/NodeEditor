using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text;

public class NodeEditorWindow : EditorWindow {
    static NodeEditorWindow window;

    public Rect window1, window2, _handleArea;
    private bool _nodeOption, _options, _handleActive, _action;
    private Texture2D _resizeHandle, _aaLine;
    private GUIContent _icon;
    private float _winMinX, _winMinY;
    private int _mainwindowID;

    [MenuItem("Window/Node Editor Example")]
    static void Init() {
        window = (NodeEditorWindow)EditorWindow.GetWindow(typeof(NodeEditorWindow));
        window.title = "Node Editor";
        window.ShowNodes();
    }

    private void ShowNodes() {
        _winMinX = 100f;
        _winMinY = 100f;
        window1 = new Rect(30, 30, _winMinX, _winMinY);
        window2 = new Rect(210, 210, _winMinX, _winMinY);

        _resizeHandle = AssetDatabase.LoadAssetAtPath("Assets/Node/Textures/PNG/ResizeHandle.png", typeof(Texture2D)) as Texture2D;
        _aaLine = AssetDatabase.LoadAssetAtPath("Assets/NodeEditor/Icons/AA1x5.png", typeof(Texture2D)) as Texture2D;
        _icon = new GUIContent(_resizeHandle);
        _mainwindowID = GUIUtility.GetControlID(FocusType.Native); //grab primary editor window controlID
    }

    void OnGUI() {
        BeginWindows();
        window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1");   // Updates the Rect's when these are dragged
        window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
        EndWindows();

        DrawNodeCurve(window1, window2);

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        _options = GUILayout.Toggle(_options, "Toggle Me", EditorStyles.toolbarButton);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //if drag extends inner window bounds _handleActive remains true as event gets lost to parent window
        if(( Event.current.rawType == EventType.MouseUp ) && ( GUIUtility.hotControl != _mainwindowID )) {
            GUIUtility.hotControl = 0;
        }
    }

    private void DrawNodeWindow(int id) {
        if(GUIUtility.hotControl == 0)  //mouseup event outside parent window?
        {
            _handleActive = false; //make sure handle is deactivated
        }

        float _cornerX = 0f;
        float _cornerY = 0f;
        switch(id) //case which window this is and nab size info
        {
            case 1:
                _cornerX = window1.width;
                _cornerY = window1.height;
                break;
            case 2:
                _cornerX = window2.width;
                _cornerY = window2.height;
                break;
        }

        //begin layout of contents
        GUILayout.BeginArea(new Rect(1, 16, _cornerX - 3, _cornerY - 1));
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        _nodeOption = GUILayout.Toggle(_nodeOption, "Node Toggle", EditorStyles.toolbarButton);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(1, _cornerY - 16, _cornerX - 3, 14));
        GUILayout.BeginHorizontal(EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
        GUILayout.FlexibleSpace();

        //grab corner area based on content reference
        _handleArea = GUILayoutUtility.GetRect(_icon, GUIStyle.none);
        GUI.DrawTexture(new Rect(_handleArea.xMin + 6, _handleArea.yMin - 3, 20, 20), _resizeHandle); //hacky placement
        _action = ( Event.current.type == EventType.MouseDown ) || ( Event.current.type == EventType.MouseDrag );
        if(!_handleActive && _action) {
            if(_handleArea.Contains(Event.current.mousePosition, true)) {
                _handleActive = true; //active when cursor is in contact area
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Native); //set handle hot
            }
        }

        EditorGUIUtility.AddCursorRect(_handleArea, MouseCursor.ResizeUpLeft);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //resize window
        if(_handleActive && ( Event.current.type == EventType.MouseDrag )) {
            ResizeNode(id, Event.current.delta.x, Event.current.delta.y);
            Repaint();
            Event.current.Use();
        }

        //enable drag for node
        if(!_handleActive) {
            GUI.DragWindow();
        }
    }

    private void ResizeNode(int id, float deltaX, float deltaY) {
        switch(id) {
            case 1:
                if(( window1.width + deltaX ) > _winMinX) { window1.xMax += deltaX; }
                if(( window1.height + deltaY ) > _winMinY) { window1.yMax += deltaY; }
                break;
            case 2:
                if(( window2.width + deltaX ) > _winMinX) { window2.xMax += deltaX; }
                if(( window2.height + deltaY ) > _winMinY) { window2.yMax += deltaY; }
                break;
        }
    }

    void DrawNodeCurve(Rect start, Rect end) {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, _aaLine, 1.5f);
    }
}