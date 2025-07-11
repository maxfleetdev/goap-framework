using System;
using UnityEngine;

/// <summary>
/// Static wrapper class for centralised and customised logging without UnityEngine dependancy
/// </summary>
public static class Logger
{
    #region Logging Methods

    /// <summary>
    /// Logs the class which has been initialised
    /// </summary>
    /// <param name="name"></param>
    public static void Initialised(string name)
    {
        Debug.Log($"{ColourString(LogColour.Green, $"[{CleanName(name)}]")} Initialised");
    }

    /// <summary>
    /// Logs data has been loaded to console
    /// </summary>
    /// <param name="name"></param>
    /// <param name="msg"></param>
    public static void Loaded(string name, int count, string data)
    {
        Debug.Log($"{ColourString(LogColour.Lightblue, $"[{CleanName(name)}]")} Loaded {count} entries for {data}.");
    }

    /// <summary>
    /// Logs the object which has been terminated
    /// </summary>
    /// <param name="name"></param>
    public static void Terminated(string name)
    {
        Debug.Log($"{ColourString(LogColour.Orange, $"[{CleanName(name)}]")} Terminated successfully");
    }

    /// <summary>
    /// Logs the name of the source terminator and how many it removed
    /// </summary>
    /// <param name="name"></param>
    /// <param name="count"></param>
    public static void Terminated(string name, int count)
    {
        Debug.Log($"{ColourString(LogColour.Orange, $"[{CleanName(name)}]")} Terminated {count} objects successfully");
    }

    /// <summary>
    /// Logs a warning to console
    /// </summary>
    /// <param name="name"></param>
    /// <param name="msg"></param>
    public static void Warn(string name, string msg)
    {
        Debug.LogWarning($"{ColourString(LogColour.Orange, $"[{CleanName(name)}]")} {msg}");
    }

    /// <summary>
    /// Logs an error to console
    /// </summary>
    /// <param name="name"></param>
    /// <param name="msg"></param>
    public static void Error(string name, string msg)
    {
        Debug.LogError($"{ColourString(LogColour.Red, $"[{CleanName(name)}]")} {msg}");
    }

    /// <summary>
    /// Logs information to console
    /// </summary>
    /// <param name="name"></param>
    /// <param name="msg"></param>
    public static void Info(string msg)
    {
        Debug.Log(msg);
    }
    /// <summary>
    /// Logs information to console with provided name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="msg"></param>
    public static void Info(string name, string msg)
    {
        Debug.Log($"[{CleanName(name)}] {msg}");
    }
    /// <summary>
    /// Logs information in colour to console with provided name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="msg"></param>
    public static void Info(string name, string msg, LogColour colour)
    {
        Debug.Log($"{ColourString(colour, $"[{CleanName(name)}]")} {msg}");
    }

    #endregion

    #region String Builer

    // returns string with RichText in a given colour
    private static string ColourString(LogColour colour, string msg)
    {
        return $"<color={LogColourToString(colour)}>" + msg + "</color>";
    }

    // returns enum value to raw text
    private static string LogColourToString(LogColour colour)
    {
        return Enum.IsDefined(typeof(LogColour), colour) ? colour.ToString().ToLower(): "white";
    }

    // returns cleaned up string using Split and Generic `
    private static string CleanName(string name)
    {
        return name.Split('`')[0];
    }

    #endregion
}

/// <summary>
/// Colour of the logged message
/// </summary>
public enum LogColour
{
    Aqua,
    Blue,
    Darkblue,
    Green,
    Lightblue,
    Lime,
    Orange,
    Red,
    Teal,
    White
}