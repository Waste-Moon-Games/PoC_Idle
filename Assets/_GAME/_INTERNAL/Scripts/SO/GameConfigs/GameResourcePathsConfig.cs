using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Game Resources/Resource Paths Config")]
    public class GameResourcePathsConfig : ScriptableObject
    {
        [field: SerializeField] public Dictionary<string, string> Paths { get; private set; } = new();

        public void SetPath(string key, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (Paths.ContainsKey(key))
                    Paths.Remove(key);
            }
            else
                Paths[key] = path;
        }

        public string GetPathByKeyWord(string keyWord)
        {
            var path = string.Empty;

            if(Paths.TryGetValue(keyWord, out var resourcePath))
            {
                path = resourcePath;
                path.Trim().Trim('"');
                return path;
            }

            else
            {
                Debug.Log($"[Game Resource Paths Config] Resource Path {keyWord} not found! Return empty path");
                return path;
            }
        }
    }
}