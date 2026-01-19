using Core.Common.Animations;
using R3;
using SO.AnimationConfigs;
using UnityEngine;
using Utils;

namespace UI.GameplayMenu.Animations
{
    public class ClickAnimationsService : AnimationService
    {
        private readonly RotateAnimation _rotateAnimationService;

        private readonly RectTransform _coinsSpawnArea;
        private readonly Transform _targetObject;
        private readonly Vector3 _targetScale;
        private readonly Vector3 _defaultScale;

        private readonly ObjectPool<CoinsPerClick> _coinsPerClickPool;

        public ClickAnimationsService(Transform target, Vector3 targetScale, Vector3 defaultScale, GameObject prefab, int initCount)
        {
            _defaultScale = defaultScale;
            _targetObject = target;
            _targetScale = targetScale;

            _coinsSpawnArea = target.GetComponent<RectTransform>();

            var poolContainer = new GameObject("CoinsPerClickContainer");
            poolContainer.transform.SetParent(target.parent, false);
            _coinsPerClickPool = new(prefab.GetComponent<CoinsPerClick>(), initCount, poolContainer.transform)
            {
                AutoExpand = true
            };

            var rotationAnimsConfig = Resources.Load<RotationAnimationsConfig>("Configs/Animations/RotationAnimationsConfig");
            _rotateAnimationService = new(target, rotationAnimsConfig.RotationSpeed, rotationAnimsConfig.RotateDirection);
        }

        public override void OnClickDown()
        {
            _targetObject.localScale = _targetScale;
        }

        public override void OnClickUp()
        {
            _targetObject.localScale = _defaultScale;
        }

        public void RotateAnimation() => _rotateAnimationService.Rotate();

        public override void ClickAnimation(string amount = null)
        {
            var coin = _coinsPerClickPool.GetFreeElement();
            coin.AnimationEnded.Take(1).Subscribe(_ => _coinsPerClickPool.ReturnToPool(coin));

            coin.SetupPosition(GetRandomPosition(_coinsSpawnArea));
            coin.SetCoinsPerClickAmount(amount);
        }

        private Vector2 GetRandomPosition(RectTransform area)
        {
            float randX = Random.Range(-area.rect.width / 2f, area.rect.width / 2f);
            float randY = Random.Range(-area.rect.height / 2f, area.rect.height / 2f);

            return new Vector2(randX, randY);
        }
    }
}