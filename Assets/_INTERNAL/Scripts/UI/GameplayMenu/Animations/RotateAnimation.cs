using UnityEngine;

namespace UI.GameplayMenu.Animations
{
    public class RotateAnimation
    {
        private readonly Transform _target;
        private readonly float _speed;
        private readonly Vector3 _direction;

        public RotateAnimation(Transform target, float speed, Vector3 direction)
        {
            _target = target;
            _speed = speed;
            _direction = direction;
        }

        public void Rotate() => _target.Rotate(_speed * Time.deltaTime * _direction);
    }
}