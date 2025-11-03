using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SeqDirect.Core {
    /// <summary>
    /// Attribute used to mark node classes for automatic registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SeqNodeAttribute : Attribute {
        public string Id { get; }
        public string Category { get; set; }
        public string Description { get; set; }

        public SeqNodeAttribute(string id) {
            Id = id;
        }
    }

    /// <summary>
    /// Registry that discovers and instantiates node types marked with <see cref="SeqNodeAttribute"/>.
    /// </summary>
    public static class SeqNodeRegistry {
        private static readonly Dictionary<string, Type> _registeredNodes = new();
        private static bool _initialized;

        /// <summary>
        /// Scans all assemblies for types marked with <see cref="SeqNodeAttribute"/>.
        /// </summary>
        public static void Initialize() {
            if (_initialized) return;
            _initialized = true;

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => SafeGetTypes(a))
                .Where(t => t.IsSubclassOf(typeof(SeqNodeBase<>).MakeGenericType(typeof(SeqNodeParams))) == false)
                .Where(t => typeof(ISeqNode).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var t in types) {
                var attr = t.GetCustomAttribute<SeqNodeAttribute>();
                if (attr != null && !_registeredNodes.ContainsKey(attr.Id)) {
                    _registeredNodes[attr.Id] = t;
                }
            }
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly a) {
            try {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException e) {
                return e.Types.Where(t => t != null);
            }
        }

        /// <summary>
        /// Creates a node instance by its registered Id.
        /// </summary>
        public static ISeqNode Create(string id) {
            Initialize();
            if (_registeredNodes.TryGetValue(id, out var type)) {
                return (ISeqNode)Activator.CreateInstance(type);
            }

            Debug.LogWarning($"[SeqDirect] Node id '{id}' not registered.");
            return null;
        }

        /// <summary>
        /// Returns all registered node identifiers.
        /// </summary>
        public static IEnumerable<string> GetRegisteredNodeIds() {
            Initialize();
            return _registeredNodes.Keys;
        }

        /// <summary>
        /// Returns the human-readable categories grouped by node.
        /// </summary>
        public static Dictionary<string, string> GetNodeCategories() {
            Initialize();
            var result = new Dictionary<string, string>();
            foreach (var kv in _registeredNodes) {
                var attr = kv.Value.GetCustomAttribute<SeqNodeAttribute>();
                if (attr != null) result[kv.Key] = attr.Category;
            }
            return result;
        }
    }
}
