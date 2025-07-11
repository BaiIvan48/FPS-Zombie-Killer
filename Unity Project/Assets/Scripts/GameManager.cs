using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Weapons")]
    public List<GameObject> weaponPrefabs;
    public Transform[] weaponSpawnPoints;

    [Header("Zombie prefabs")]
    public List<GameObject> normalZombies;
    public List<GameObject> specialZombies;
    public List<GameObject> bossZombies;

    [Header("Zombie Spawners")]
    public Transform[] zombieSpawnPoints;

    [Header("Wave settings")]
    public int wavesPerWeapon = 3;
    public int baseZombieCount = 10;
    public float restTime = 5f;
    public float spawnDelay = 0.5f;
    public float spawnOffsetRange = 1.0f;

    [Header("References")]
    public FPSController player;
    public Health playerHealth;
    public HUDManager hudManager;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI roundOverUI;
    public TextMeshProUGUI cooldownCounterUI;
    public TextMeshProUGUI currentWaveUI;

    private Gun currentWeapon;
    private bool isGameOver;
    public int totalWavesCompleted = 0;
    private float startTime;
    private List<GameObject> activeZombies = new List<GameObject>();

    private void Start()
    {
        if (player == null) player = GameObject.FindFirstObjectByType<FPSController>();
        if (playerHealth == null) playerHealth = player.GetComponentInChildren<Health>();

        if (playerHealth != null)
            playerHealth.OnDeath.AddListener(OnPlayerDeath);

        startTime = Time.time;
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (weaponPrefabs.Count > 0 && !isGameOver)
        {
            SpawnWeaponOptions();
            yield return StartCoroutine(WaitForWeaponPick());
            roundOverUI.gameObject.SetActive(false);

            for (int wave = 0; wave < wavesPerWeapon && !isGameOver; wave++)
            {
                currentWaveUI.text = "Wave: " + (totalWavesCompleted + 1);
                waveOverUI.gameObject.SetActive(false);

                yield return StartCoroutine(RunWave(wave));
                if (isGameOver) break;

                totalWavesCompleted++;

                if (wave < wavesPerWeapon - 1)
                {
                    yield return StartCoroutine(RestCountdown());
                }
                else
                {
                    roundOverUI.gameObject.SetActive(true);
                }
            }

            if (currentWeapon != null)
            {
                player.DropCurrentWeapon();
                Destroy(currentWeapon);
            } 
            currentWeapon = null;
        }

        if (!isGameOver)
            ShowVictory();
    }

    private void SpawnWeaponOptions()
    {
        if (weaponPrefabs.Count>1)
        {
            int a = Random.Range(0, weaponPrefabs.Count);
            int b=0;
            do { b = Random.Range(0, weaponPrefabs.Count); }
            while (b == a);

            Instantiate(weaponPrefabs[a], weaponSpawnPoints[0].position, weaponSpawnPoints[0].rotation);
            Instantiate(weaponPrefabs[b], weaponSpawnPoints[1].position, weaponSpawnPoints[1].rotation);
        }
        else
        {
            int spawner = Random.Range(0, 2);
            Instantiate(weaponPrefabs[0], weaponSpawnPoints[spawner].position, weaponSpawnPoints[spawner].rotation);
        }

    }

    private IEnumerator WaitForWeaponPick()
    {
        while (player.CurrentWeapon == null && !isGameOver)
            yield return null;

        currentWeapon = player.CurrentWeapon;

        foreach (var obj in GameObject.FindGameObjectsWithTag("Weapon"))
            if (obj.GetComponent<Gun>() != currentWeapon)
                Destroy(obj);

        weaponPrefabs.Remove(currentWeapon.gameObject);
    }

    private IEnumerator RunWave(int waveIndex)
    {
        activeZombies.Clear();

        int count = baseZombieCount * (waveIndex + 1);

        if (waveIndex < 2)
        {
            float normalRatio = waveIndex == 0 ? 0.7f : 0.3f;
            int normalCount = Mathf.RoundToInt(count * normalRatio);
            int specialCount = count - normalCount;

            yield return StartCoroutine(SpawnZombies(normalZombies, normalCount));
            yield return StartCoroutine(SpawnZombies(specialZombies, specialCount));
        }
        else
        {
            yield return StartCoroutine(SpawnZombies(bossZombies, 1));
            int half = Mathf.RoundToInt(count * 0.5f);
            yield return StartCoroutine(SpawnZombies(normalZombies, half));
            yield return StartCoroutine(SpawnZombies(specialZombies, half));
        }

        while (activeZombies.Count > 0 && !isGameOver)
            yield return null;
    }

    private IEnumerator SpawnZombies(List<GameObject> prefabs, int total)
    {
        for (int i = 0; i < total; i++)
        {
            var prefab = prefabs[Random.Range(0, prefabs.Count)];
            var sp = zombieSpawnPoints[Random.Range(0, zombieSpawnPoints.Length)];

            Vector3 offset = new Vector3( Random.Range(-spawnOffsetRange, spawnOffsetRange),0f,Random.Range(-spawnOffsetRange, spawnOffsetRange));

            GameObject z = Instantiate(prefab, sp.position + offset, sp.rotation);

            activeZombies.Add(z);

            var health = z.GetComponent<Health>();
            if (health != null)
                health.OnDeath.AddListener(() => activeZombies.Remove(z));

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private IEnumerator RestCountdown()
    {
        waveOverUI.gameObject.SetActive(true);

        float remaining = restTime;
        while (remaining > 0f)
        {
            cooldownCounterUI.text = remaining.ToString("F0");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        waveOverUI.gameObject.SetActive(false);
    }

    private void OnPlayerDeath()
    {
        isGameOver = true;
        float elapsed = Time.time - startTime;
        Debug.Log($"Game Over! Waves: {totalWavesCompleted}, Time: {elapsed:F1}s");
        //hudManager.ShowGameOver(totalWavesCompleted, elapsed);
    }

    private void ShowVictory()
    {
        float elapsed = Time.time - startTime;
        Debug.Log($"Victory! Waves: {totalWavesCompleted}, Time: {elapsed:F1}s");
        //hudManager.ShowVictory(totalWavesCompleted, elapsed);
    }
}
