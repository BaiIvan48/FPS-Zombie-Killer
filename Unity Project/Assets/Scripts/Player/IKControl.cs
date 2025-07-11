using UnityEngine;

public class IKControl : MonoBehaviour
{
    public bool IkActive = true;
    public Transform RightHandTarget;
    public Transform LeftHandTarget;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {

            float v = IkActive ? 1 : 0;

            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, v);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, v);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, v);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, v);

            if (IkActive)
            {
                _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
            }
        }
    }

}
