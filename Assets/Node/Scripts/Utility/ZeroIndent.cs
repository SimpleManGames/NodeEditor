using System;
using UnityEditor;

/// <summary>
/// This is a helper class for FixedWidthLabel
/// It clears the indentation
/// </summary>
internal class ZeroIndent : IDisposable {
    private readonly int originalIndent;//the original indentation value before we change the GUI state
    public ZeroIndent() {
        originalIndent = EditorGUI.indentLevel;//save original indentation
        EditorGUI.indentLevel = 0;//clear indentation
    }

    public void Dispose() {
        EditorGUI.indentLevel = originalIndent;//restore original indentation
    }
}