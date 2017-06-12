//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace BehaviourMachine {
    /// <summary> 
    /// Class containing methods to print messages on the Unity console.
    /// </summary>
    public class Print {

        public static readonly string bmStringLogo = "{<b><color=#78a413>b</color></b>}";

        /// <summary> 
        /// Logs message to the Unity Console.
        /// <param name="message">The message to be printed in the console.</param> 
        /// </summary>
        static public void Log (string message) {
            if (Debug.isDebugBuild)
                Debug.Log(bmStringLogo + ": " + message);
        }

        /// <summary> 
        /// Logs message to the Unity Console.
        /// <param name="message">The message to be printed in the console.</param> 
        /// <param name="context">An object to be highlighted if you select the message in the console.</param> 
        /// </summary>
        static public void Log (string message, UnityEngine.Object context) {
            if (Debug.isDebugBuild)
                Debug.Log(Print.bmStringLogo + ": " + message, context);
        }

        /// <summary> 
        /// Logs a warning message to the Unity Console.
        /// <param name="message">The message to be printed in the console.</param> 
        /// </summary>
        static public void LogWarning (string message) {
            if (Debug.isDebugBuild)
                Debug.LogWarning(Print.bmStringLogo + ": " + message);
        }

        /// <summary> 
        /// Logs a warning message to the Unity Console.
        /// <param name="message">The message to be printed in the console.</param> 
        /// <param name="context">An object to be highlighted if you select the message in the console.</param> 
        /// </summary>
        static public void LogWarning (string message, UnityEngine.Object context) {
            if (Debug.isDebugBuild)
                Debug.LogWarning(Print.bmStringLogo + ": " + message, context);
        }

        /// <summary> 
        /// Logs an error message to the Unity Console.
        /// <param name="message">The message to be printed in the console.</param> 
        /// </summary>
        static public void LogError (string message) {
            if (Debug.isDebugBuild)
                Debug.LogError(Print.bmStringLogo + ": " + message);
        }

        /// <summary> 
        /// Logs an error message to the Unity Console.
        /// <param name="message">The message to be printed in the console.</param> 
        /// <param name="context">An object to be highlighted if you select the message in the console.</param> 
        /// </summary>
        static public void LogError (string message, UnityEngine.Object context) {
            if (Debug.isDebugBuild)
                Debug.LogError(Print.bmStringLogo + ": " + message, context);
        }
    }
}