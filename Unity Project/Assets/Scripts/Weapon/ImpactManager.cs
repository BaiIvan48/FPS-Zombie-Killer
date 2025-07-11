using UnityEngine;

public class ImpactManager : MonoBehaviour
{
    public static ImpactManager Instance { get; private set; }

    [Header("Bullet Hole Decals")]
    public GameObject BulletHolePrefab;

    [Header("Stone Impact prefabs")]
    public GameObject StoneDustPrefab;
    public GameObject StoneDebrisPrefab;
    public AudioClip StoneHitSound;

    [Header("Metal Impact prefabs")]
    public GameObject MetalDustPrefab;
    public GameObject MetalDebrisPrefab;
    public AudioClip MetalHitSound;

    [Header("Wood Impact prefabs")]
    public GameObject WoodDustPrefab;
    public GameObject WoodDebrisPrefab;
    public AudioClip WoodHitSound;

    [Header("Flesh Impact prefabs")]
    public GameObject FleshDebrisPrefab;
    public AudioClip FleshHitSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void HandleImpact(RaycastHit hit)
    {
        string tag = hit.collider.tag;

        GameObject dust = null;
        GameObject debris = null;
        AudioClip sound = null;

        switch (tag)
        {
            case "Stone":
                dust = StoneDustPrefab;
                debris = StoneDebrisPrefab;
                sound = StoneHitSound;
                break;
            case "Metal":
                dust = MetalDustPrefab;
                debris = MetalDebrisPrefab;
                sound = MetalHitSound;
                break;
            case "Wood":
                dust = WoodDustPrefab;
                debris = WoodDebrisPrefab;
                sound = WoodHitSound;
                break;
            case "Flesh":
                dust = null;
                debris = FleshDebrisPrefab;
                sound = FleshHitSound;
                break;
            default:
                dust = StoneDustPrefab;
                debris = StoneDebrisPrefab;
                sound = StoneHitSound;
                break;
        }

        if (dust != null)
        {
            GameObject dustFx = Instantiate(dust, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(dustFx, 2f);
        }

        if (debris != null)
        {
            GameObject debrisFx = Instantiate(debris, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(debrisFx, 1.5f);
        }

        if (BulletHolePrefab != null && tag != "Flesh")
        {
            GameObject hole = Instantiate(BulletHolePrefab, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation(Vector3.up,hit.normal));
            hole.transform.SetParent(hit.collider.transform);
            Destroy(hole, 8f);
        }

        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, hit.point);
        }
    }
}
