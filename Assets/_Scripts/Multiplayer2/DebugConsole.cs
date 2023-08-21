using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    private static DebugConsole instance;
    private List<string> debugMessages = new List<string>();
    private int maxMessages = 10;

    // Private constructor to prevent direct instantiation
    private DebugConsole() { }

    public static DebugConsole Instance
    {
        get
        {
            if (instance == null)
            {
                // If the instance is null, find the existing instance in the scene
                instance = FindObjectOfType<DebugConsole>();
                if (instance == null)
                {
                    // If the instance is still null, create a new GameObject and add the DebugConsole component to it
                    GameObject consoleGO = new GameObject("DebugConsole");
                    instance = consoleGO.AddComponent<DebugConsole>();
                }
            }
            return instance;
        }
    }

    public void Log(string message)
    {
        debugMessages.Add(message);
        if (debugMessages.Count > maxMessages)
            debugMessages.RemoveAt(0);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));

        GUILayout.Label("Debug Console:");

        foreach (var message in debugMessages)
        {
            GUILayout.Label(message);
        }

        GUILayout.EndArea();
    }
}
