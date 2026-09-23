using Common.MVVM;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.AddressablesLoadSystem
{
    public interface IUniversalResourceLoader
    {
        UniTask<T> LoadViewEntity<T>(string resourceKey) where T: Object, IView;
    }
}