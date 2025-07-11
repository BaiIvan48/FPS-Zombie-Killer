using UnityEngine;

public abstract partial class Gun : MonoBehaviour
{
    private Vector3 _currentPositionalRecoil = Vector3.zero;
    private Quaternion _currentRotationalRecoil = Quaternion.identity;

    private float bobTimer = 0f;

    private void ApplyRecoil()
    {
        Vector3 targetPositionalRecoil = Vector3.zero;
        Quaternion targetRotationalRecoil = Quaternion.identity;

        if (isRecoiling)
        {
            targetPositionalRecoil = new Vector3(0, 0, GunData.PositionalRecoilAmount); // -positionalRecoilAmount but it goes the other way
            targetRotationalRecoil = Quaternion.Euler(-GunData.RotationaRecoilAmount, 0, 0); // rotationaRecoilAmount but it goes the other way

            if (Vector3.Distance(_currentPositionalRecoil, targetPositionalRecoil) < 0.1f)
            {
                isRecoiling = false;
            }
        }

        _currentPositionalRecoil = Vector3.Lerp(_currentPositionalRecoil, targetPositionalRecoil, Time.deltaTime * GunData.RecoilSmoothness);
        _currentRotationalRecoil = Quaternion.Lerp(_currentRotationalRecoil, targetRotationalRecoil, Time.deltaTime * GunData.RecoilSmoothness);

        transform.localPosition -= _currentPositionalRecoil;
        transform.localRotation *= _currentRotationalRecoil;
    }
    private void ApplySway()
    {
        Vector3 basePos = isAimingState ? GunData.WeaponAimPosition : GunData.WeaponDefaultPosition;

        Vector3 swipeOffset = new Vector3(Look.x, Look.y, 0) * GunData.PositionalSway;
        transform.localPosition = Vector3.Lerp(transform.localPosition, basePos - swipeOffset, Time.deltaTime * GunData.SwaySmoothness);
    }

    private void ApplyBobbing()
    {
        float moveMag = Move.magnitude*5;
        float bobSpeed = GunData.BobbingSpeed * moveMag;
        float bobAmount = GunData.BobbingAmount * moveMag;

        bobTimer += Time.deltaTime * bobSpeed;
        float yOffset = Mathf.Sin(bobTimer) * bobAmount;

        Vector3 basePos = isAimingState ? GunData.WeaponAimPosition : GunData.WeaponDefaultPosition;

        transform.localPosition += new Vector3(0, yOffset, 0);
    }
}
