using UnityEngine;

public abstract partial class Gun : MonoBehaviour
{
    public Vector3 CurrentRecoil { get; protected set; }
    private Vector3 _targetRecoil;

    private float CalculateRecoilMultiplaier()
    {
        float aimMultiplaier = isAimingState ? 0.7f : 1f;
        float jumpMultiplaier = input.Jump ? 1.5f : 1f;
        float runMultiplaier = input.Run ? 2f : 1f;
        float crouchMultiplaier = input.Crouch ? 0.8f : 1f;
        float moveMultiplaier = input.Move != Vector2.zero ? 1.25f : 1f;

        return aimMultiplaier * jumpMultiplaier * runMultiplaier * crouchMultiplaier * moveMultiplaier;
    }

    public void ApplyTargetRecoil()
    {
        float recoilMultiplaier = CalculateRecoilMultiplaier();

        float recoilX = UnityEngine.Random.Range(-1f, 1f) * GunData.RecoilAmount * recoilMultiplaier;
        float recoilY = UnityEngine.Random.Range(0.5f, 1f) * GunData.RecoilAmount * recoilMultiplaier;

        _targetRecoil += new Vector3(recoilX, recoilY, 0);
        CurrentRecoil = _targetRecoil;
    }

    public void ResetTargetRecoil()
    {
        float recoilMultiplaier = CalculateRecoilMultiplaier();

        CurrentRecoil = Vector3.Lerp(CurrentRecoil, Vector3.zero, Time.deltaTime * GunData.RecoilResetSpeed * recoilMultiplaier);
        _targetRecoil = Vector3.Lerp(_targetRecoil, Vector3.zero, Time.deltaTime * GunData.RecoilResetSpeed * recoilMultiplaier);
    }
}
