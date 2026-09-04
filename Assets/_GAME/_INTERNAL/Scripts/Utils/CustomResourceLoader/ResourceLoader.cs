using UnityEngine;

namespace Utils.CustomResourceLoader
{
    public static class ResourceLoader
    {
        public static T LoadOrThrow<T>(string path) where T : Object
        {
            var asset = Resources.Load<T>(path);

            if (asset == null)
                throw new MissingReferenceException($"[Resource Loader] Resource not found by path '{path}'. Type: {typeof(T).Name}");

            return asset;
        }
    }
}