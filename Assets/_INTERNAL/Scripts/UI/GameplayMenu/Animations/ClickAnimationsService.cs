using Core.Common.Animations;
using DG.Tweening;
using R3;
using SO.AnimationConfigs;
using UnityEngine;
using Utils;
using Utils.CustomResourceLoader;

namespace UI.GameplayMenu.Animations
{
    public class ClickAnimationsService : AnimationService
    {
        private readonly RotateAnimation _rotateAnimationService;
        private readonly CoinFlowAnimation _coinFlowAnimationService;
        private readonly CoinGlowPulseAnimation _coinGlowPulseAnimationService;

        private readonly RectTransform _coinsSpawnArea;
        private readonly Transform _targetObject;
        private readonly Vector3 _targetScale;
        private readonly Vector3 _defaultScale;
        private readonly float _clickAnimDuration;

        private readonly ObjectPool<CoinsPerClick> _coinsPerClickPool;

        public ClickAnimationsService(Transform target, ClickAnimationsConfig config)
        {
            _defaultScale = config.DefaultScale;
            _targetObject = target;
            _targetScale = config.TargetScale;
            _clickAnimDuration = config.ClickAnimationDuration;

            _coinsSpawnArea = target.GetComponent<RectTransform>();

            var poolContainer = new GameObject("CoinsPerClickContainer");
            poolContainer.transform.SetParent(target.parent, false);
            _coinsPerClickPool = new(config.Prefab.GetComponent<CoinsPerClick>(), config.InitPoolCount, poolContainer.transform)
            {
                AutoExpand = true
            };

            var rotationAnimsConfig = ResourceLoader.LoadOrThrow<RotationAnimationsConfig>("Configs/Animations/RotationAnimationsConfig");
            var coinFlowAnimsConfig = ResourceLoader.LoadOrThrow<CoinFlowAnimationConfig>("Configs/Animations/CoinFlowAnimationConfig");
            var coinGlowPulseAnimsConfig = ResourceLoader.LoadOrThrow<CoinGlowPulseAnimationConfig>("Configs/Animations/CoinGlowPulseAnimationConfig");

            _rotateAnimationService = new(target, rotationAnimsConfig.RotationSpeed, rotationAnimsConfig.RotateDirection);

            var targetRectTransform = target.GetComponent<RectTransform>();
            var coinGlowRectTransform = target.GetChild(0).GetComponent<RectTransform>();
            _coinFlowAnimationService = new(targetRectTransform, coinFlowAnimsConfig.Amplitude, coinFlowAnimsConfig.Duration);
            _coinGlowPulseAnimationService = new(coinGlowRectTransform, coinGlowPulseAnimsConfig.Duration, coinGlowPulseAnimsConfig.MinScale, coinGlowPulseAnimsConfig.MaxScale);
        }

        public override void OnClickDown()
        {
            _targetObject.DOKill();

            _targetObject.DOScale(_targetScale, _clickAnimDuration * 2f).SetEase(Ease.OutBounce);
        }

        public override void OnClickUp()
        {
            Sequence clickSequence = DOTween.Sequence();

            clickSequence.Append(
                _targetObject
                .DOScale(_defaultScale * 1.05f, _clickAnimDuration)
                .SetEase(Ease.OutBack));

            clickSequence.Append(_targetObject.DOScale(_defaultScale, _clickAnimDuration * 2f).SetEase(Ease.InOutBack));
        }

        public void StartAnimations()
        {
            _coinFlowAnimationService.StartFlow();
            _coinGlowPulseAnimationService.StartPulse();
        }

        public void StopAnimations()
        {
            _coinFlowAnimationService.StopFlow();
            _coinGlowPulseAnimationService.StopPulse();
            _targetObject.DOKill();
        }

        public void LoopAnimations()
        {
            _rotateAnimationService.Rotate();
        }

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