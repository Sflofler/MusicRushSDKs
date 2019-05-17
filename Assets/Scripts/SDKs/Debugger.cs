using UnityEngine;

public static class Debugger
{
    public static void Log(string color, string sdk, string message)
    {
        Debug.Log(string.Format("<color={0}>{1}: {2}</color>", color, sdk, message));
    }
}
