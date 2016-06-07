using UnityEngine;
using UnityEditor;

//We use scriptable object because in the future we will need the messages that unity calls on scriptable objects
//such as OnDestroy() OnEnable()
public abstract class BaseNode : ScriptableObject {

    private Rect _windowRect;
    /// <summary> Store the Rect of the window, location and size </summary>
    public Rect WindowRect {
        get { return _windowRect; }
        set { _windowRect = value; }
    }

    private Rect _handleArea;
    /// <summary> Stores the Rect on the resize handle </summary>
    public Rect HandleArea {
        get { return _handleArea; }
        set { _handleArea = value; }
    }

    private bool _hasInputs = false;
    /// <summary> Indicates if the decendant of the baseNode has inputs or can only be used as an input to another node </summary>
    public bool HasInputs {
        get { return _hasInputs; }
        set { _hasInputs = value; }
    }

    private bool _resizable = true;
    /// <summary> Used to see if the node is resizable in the editor </summary>
    public bool Resizable {
        get { return _resizable; }
        set { _resizable = value; }
    }

    private string _windowTitle = "";
    /// <summary> The title of the node </summary>
    public string WindowTitle {
        get { return _windowTitle; }
        set { _windowTitle = value; }
    }

    //Draw the window of the base node, this is virtual and will be implemented by each subclass of baseNode
    public virtual void DrawWindow() {
        //We want each node to have a title which the user can modify
        WindowTitle = EditorGUILayout.TextField("Title", WindowTitle);
    }

    /// <summary> Draws the curves from the inputs of this nodes, this is abstract because it must be implemented by each subclass of this baseNode </summary>
    public abstract void DrawCurves();

    /// <summary> Is used by the subclasses nodes that has inputs and is called when the window is clicked during a transition
    /// <para> we make it virtual because it's optional for nodes that don't have inputs </para>
    /// it is passed in the metho the node to be used as input and the click position of the mouse </summary>
    /// <param name="input"> The input node </param>
    /// <param name="clickPos"> Where you click on the node to see which input spot to put it </param>
    public virtual void SetInput(BaseInputNode input, Vector2 clickPos) { }

    /// <summary> Is called to all nodes when a node is deleted so that it is removed if it used as an input </summary>
    /// <param name="node"> The node that will be deleted </param>
    public virtual void NodeDeleted(BaseNode node) { }

    /// <summary> Is called when a click happens on a window and returns the input that was clicked or null otherwise </summary> 
    /// <param name="pos"> Position on the node where you clicked </param>
    public virtual BaseInputNode ClickedOnInput(Vector2 pos) { return null; }

    /// <summary> Is used to update the nodes when its not in focus </summary>
    /// <param name="deltaTime"> The time per frame </param>
    public abstract void Tick(float deltaTime);
}
