using System.IO;
using System.Linq;
using System.Reflection;
using Core.AddressablesLoadSystem;
using SO;
using UnityEditor;
using UnityEngine;

namespace _INTERNAL.Scripts.Editor
{
    [CustomEditor(typeof(GameResourcePathsConfig))]
    public class GameResourcePathsConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var config = (GameResourcePathsConfig)target;

            EditorGUILayout.LabelField("Game Resource Paths", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Drop the resource into field.", MessageType.Info);
            EditorGUILayout.Space();

            var fields = typeof(GameplayResourcePathKeys)
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

                UnityEngine.Object currentObject = null;
                if(config.Paths.TryGetValue(keyValue, out string currentPath) && !string.IsNullOrEmpty(currentPath))
                    currentObject = Resources.Load(currentPath);

                UnityEngine.Object newObject = EditorGUILayout.ObjectField(currentObject, typeof(UnityEngine.Object), false);
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
                            Debug.LogWarning($"[Game Resource Paths Config] Resource '{newObject.name}' is not located in Resources folder! Resource.Load can't find it in runtime", newObject);

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