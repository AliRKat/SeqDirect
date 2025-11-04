using UnityEngine;

namespace SeqDirect.Core {
    /// <summary>
    /// Central logging utility for all SeqDirect systems.
    /// Handles warnings, missing dependencies, and editor/runtime safe logs.
    /// </summary>
    public static class SeqLogger {
        private const string Prefix = "<color=#7CE7FF>[SeqDirect]</color>";

        public static bool Verbose = true;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Info(string message, Object context = null) {
            if (!Verbose) return;
            Debug.Log($"{Prefix} {message}", context);
        }

        public static void Warn(string message, Object context = null) {
            Debug.LogWarning($"{Prefix} ⚠️ {message}", context);
        }

        public static void Error(string message, Object context = null) {
            Debug.LogError($"{Prefix} ❌ {message}", context);
        }

        public static void DependencyMissing(string dependencyName, string nodeId) {
            Debug.LogError($"{Prefix} Missing dependency <b>{dependencyName}</b>. Node <b>{nodeId}</b> cannot execute.");
        }

        public static void NodeExecution(string nodeId, float duration, Object context = null) {
            if (!Verbose) return;
            Debug.Log($"{Prefix} ▶ Executing node <b>{nodeId}</b> ({duration:0.00}s)", context);
        }
    }
}
