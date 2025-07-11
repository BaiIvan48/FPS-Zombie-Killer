using System.Collections;
using UnityEngine;

public abstract partial class Gun : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] protected ParticleSystem muzzleFlash;
    [SerializeField] protected GameObject bulletTrailPrefab;

    protected AudioSource audioSource;

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    protected IEnumerator BulletFire(Vector3 target, RaycastHit hit, bool hitSomething)
    {
        GameObject bulletTrail = Instantiate(bulletTrailPrefab, GunMuzzle.position, Quaternion.identity);

        while (bulletTrail != null && Vector3.Distance(bulletTrail.transform.position, target) > 0.1f)
        {
            bulletTrail.transform.position = Vector3.MoveTowards(bulletTrail.transform.position, target, Time.deltaTime * GunData.BulletSpeed);
            yield return null;
        }

        Destroy(bulletTrail);

        if (hitSomething)
        {
            ImpactManager.Instance.HandleImpact(hit);

            var health = hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(GunData.Damage);
            }
        }
    }
}
