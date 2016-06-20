using System;
using System.Reflection;

using UnityEditor;

public static class ConsoleHelper {

    public static void ClearLog() {
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));

        System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
        System.Reflection.MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
