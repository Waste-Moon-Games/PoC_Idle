using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Attributes;

using SO;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _INTERNAL.Scripts.Editor
{
    [CustomEditor(typeof(GameResourcePathsConfig))]
    public class GameResourcePathsConfigEditor : UnityEditor.Editor
    {
        private Type[] _candidateTypes;
        private string[] _candidateDisplayNames;
        private int _selectedIndex = -1;

        void OnEnable() => RefreshCandidateTypes();

        private void RefreshCandidateTypes()
        {
            _candidateTypes = TypeCache.GetTypesWithAttribute<ResourceKeysProviderAttribute>()
                .OrderBy(t => t.Name)
                .ToArray();

            _candidateDisplayNames = _candidateTypes
                .Select(t => t.Name)
                .ToArray();

            var config = (GameResourcePathsConfig)target;
            if (!string.IsNullOrEmpty(config.KeysClassName))
            {
                var savedType = Type.GetType(config.KeysClassName);
                _selectedIndex = savedType != null ? Array.IndexOf(_candidateTypes, savedType) : -1;
            }
        }

        public override void OnInspectorGUI()
        {
            var config = (GameResourcePathsConfig)target;

            EditorGUILayout.LabelField("Game Resource Paths", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if(_candidateTypes == null || _candidateTypes.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "No classes found with [ResourceKeyProvider]" +
                    "Add attribute to static-class with const string fields",
                    MessageType.Warning);
                return;
            }

            int newIndex = EditorGUILayout.Popup("Keys Source", _selectedIndex, _candidateDisplayNames);
            if(newIndex != _selectedIndex)
            {
                _selectedIndex = newIndex;
                config.KeysClassName = _candidateTypes[_selectedIndex].AssemblyQualifiedName;
                EditorUtility.SetDirty(config);
            }

            if(_selectedIndex < 0)
            {
                EditorGUILayout.HelpBox("Select class with resource keys.", MessageType.Info);
                return;
            }

            EditorGUILayout.HelpBox("Drop the resource into field.", MessageType.Info);
            EditorGUILayout.Space();

            DrawKeyFields(config, _candidateTypes[_selectedIndex]);
        }

        private void DrawKeyFields(GameResourcePathsConfig config, Type keysType)
        {
            var fields = keysType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

            bool changed = false;

            foreach (var field in fields)
            {
                string keyName = field.Name;
                string keyValue = (string)field.GetRawConstantValue();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                string displayName = ObjectNames.NicifyVariableName(keyName.Replace("Key", ""));
                EditorGUILayout.LabelField(displayName, EditorStyles.boldLabel, GUILayout.Width(200));

                Object currentObject = null;
                if(config.Paths.TryGetValue(keyValue, out string currentPath) && !string.IsNullOrEmpty(currentPath))
                    currentObject = Resources.Load(currentPath);

                Object newObject = EditorGUILayout.ObjectField(currentObject, typeof(object), false);
                EditorGUILayout.EndHorizontal();

                if(config.Paths.TryGetValue(keyValue, out string rawPath) && !string.IsNullOrEmpty(rawPath))
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField("Saved Path", rawPath);
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);

                if(newObject != currentObject)
                {
                    string newPath = string.Empty;
                    if(newObject != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(newObject);
                        newPath = GetPathInResource(assetPath);

                        if(!assetPath.Contains("Resources/"))
                            Debug.LogWarning(
                                $"[Game Resource Paths Config] Resource '{newObject}' is not located in Resource folder! Resource.Load can't find it in runtime",
                                newObject);
                    }

                    config.SetPath(keyValue, newPath);
                    changed = true;
                }
            }

            if(changed)
                EditorUtility.SetDirty(config);
        }

        private string GetPathInResource(string assetPath)
        {
            const string resourceFolder = "Resources/";
            int index = assetPath.IndexOf(resourceFolder);

            if(index >= 0)
            {
                string path = assetPath.Substring(index + resourceFolder.Length);
                return Path.ChangeExtension(path, null);
            }

            return assetPath;
        }
    }
}