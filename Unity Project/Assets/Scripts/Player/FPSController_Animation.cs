//FPSontroller_Animation.cs
using UnityEngine;

public partial class FPSController : MonoBehaviour
{
    [Header("Animation Blend")]
    [SerializeField] private float _animationBlendSpeed = 8.9f;

    private int _xVelHash, _yVelHash, _zVelHash, _jumpHash, _groundHash, _fallingHash, _crouchHash;
    private bool _hasAnimator;

    private void SetAnimationGrounding()
    {
        _animator.SetBool(_fallingHash, !_grounded);
        _animator.SetBool(_groundHash, _grounded);
    }
}
