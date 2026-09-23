using Core.AddressablesLoadSystem;
using Core.Consts.Enums;
using R3;
using SO;
using UnityEngine;

namespace Core.Entry.Local.WheelOfLuck
{
    public class WheelOfLuckEntryPoint : MonoBehaviour
    {
        [SerializeField] private GameResourcePathsConfig _resourceConfig;

        private readonly IUniversalResourceLoader _loader = new WheelOfLuckResourceLoader();

        public Observable<WheelOfLuckEvents> Run()
        {
            throw new System.NotImplementedException();
        }
    }
}