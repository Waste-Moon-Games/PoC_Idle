using Common.MVVM;
using Core.AddressablesLoadSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Entry.Local.WheelOfLuck
{
    public class WheelOfLuckResourceLoader : IUniversalResourceLoader
    {
        public async UniTask<T> LoadViewEntity<T>(string path) where T : Object, IView
        {
            var entityPrefab = await Resources.LoadAsync<T>(path);
            if(entityPrefab == null)
                throw new System.ArgumentNullException(nameof(entityPrefab), $"Entity not found by path: {path}");

            Object entity = Object.Instantiate(entityPrefab);
            return (T)entity;
        }
    }
}