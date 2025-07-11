using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }
    
    [Header("Weapon and ammo")]
    public TextMeshProUGUI currentAmmoUI;
    public TextMeshProUGUI magazineAmmoUI;
    public Image ammoTypeUI;
    public Image activeWeaponUI;
    public Sprite emptySlot;

    [Header("Health")]
    public TextMeshProUGUI currentHealth;
    public Slider healthSlider;
    public Gradient healthGradient;
    public Image healthFill;

    [Header("Crosshair")]
    public Image crosshair;
    public Sprite pistolCrosshair;
    public Sprite shotgunCrosshair;
    public Sprite arCrosshair;
    public Sprite smgCrosshair;

    [SerializeField]private FPSController player;
    public Image gameOverImage;
    public TextMeshProUGUI survivedWaves;
    public Image winImage;
    private Gun activeWeapon;
    private Health playerHealth;

    private string ammoSpritesPath = "Weapons/UI/Ammo";
    private string weaponIconsPath = "Weapons/UI/Icons";

    private Dictionary<string, Sprite> _ammoSprites;
    private Dictionary<string, Sprite> _weaponIcons;

    public GameManager gameManager;

    private void Awake()
    {
        if (Instance!=null && Instance !=this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _ammoSprites = new Dictionary<string, Sprite>();
        foreach (var sp in Resources.LoadAll<Sprite>(ammoSpritesPath))
            _ammoSprites[sp.name] = sp;

        _weaponIcons = new Dictionary<string, Sprite>();
        foreach (var sp in Resources.LoadAll<Sprite>(weaponIconsPath))
            _weaponIcons[sp.name] = sp;
    }

    private void Start()
    {
        if (player == null) return;
        player.OnWeaponChanged += OnWeaponChanged;
        OnWeaponChanged(player.CurrentWeapon);

        playerHealth = player.GetComponentInChildren<Health>();
        if (playerHealth != null)
        {
            playerHealth.OnMaxHealthSet.AddListener(SetMaxHealth);
            playerHealth.OnHealthChanged.AddListener(SetHealth);
            playerHealth.OnDeath.AddListener(OnPlayerDeath);
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnWeaponChanged -= OnWeaponChanged;
            if (activeWeapon != null)
                activeWeapon.OnAimStateChanged -= OnAimChanged;
        }
        UnsubscribeAmmo();

        if (playerHealth != null)
        {
            playerHealth.OnMaxHealthSet.RemoveListener(SetMaxHealth);
            playerHealth.OnHealthChanged.RemoveListener(SetHealth);
            playerHealth.OnDeath.RemoveListener(OnPlayerDeath);
        }
    }

    private void OnWeaponChanged(Gun newWeapon)
    {
        UnsubscribeAmmo();

        activeWeapon = newWeapon;
        if (activeWeapon != null)
        {
            UpdateAmmoUI(activeWeapon.GetCurrentAmmo, activeWeapon.GunData.MagazineSize);
            ammoTypeUI.sprite = GetAmmoSprite(activeWeapon.GunData.WeaponType);
            activeWeaponUI.sprite = GetWeaponIcon(activeWeapon.GunData.GunName);
            activeWeapon.OnAmmoChanged += UpdateAmmoUI;
            activeWeapon.OnAimStateChanged += OnAimChanged;
            UpdateCrosshair(activeWeapon.GunData.WeaponType);
        }
        else
        {
            currentAmmoUI.text = "";
            magazineAmmoUI.text = "";
            ammoTypeUI.sprite = emptySlot;
            activeWeaponUI.sprite = emptySlot;
            crosshair.sprite = emptySlot;
        }

    }

    private void UnsubscribeAmmo()
    {
        if (activeWeapon != null)
            activeWeapon.OnAmmoChanged -= UpdateAmmoUI;
    }

    private void UpdateAmmoUI(int current, int magazine)
    {
        currentAmmoUI.text = current.ToString();
        magazineAmmoUI.text = $"/{magazine}";
    }

    private Sprite GetAmmoSprite(WeaponType type)
    {
        string ammoName = type.ToString() + "_Ammo";
        if (!_ammoSprites.TryGetValue(ammoName, out var sprite))
            return emptySlot;
        return sprite;
    }

    private Sprite GetWeaponIcon(string iconName)
    {
        if (string.IsNullOrEmpty(iconName) || !_weaponIcons.TryGetValue(iconName, out var sprite))
            return emptySlot;
        return sprite;
    }

    private void SetMaxHealth(float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = max;
        healthFill.color = healthGradient.Evaluate(1f);
        currentHealth.text = $"{max}|{max}";
    }

    private void SetHealth(float current)
    {
        healthSlider.value = current;
        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
        currentHealth.text = $"{current}|{healthSlider.maxValue}";
    }

    private void UpdateCrosshair(WeaponType type)
    {
        switch (type)   
        {
            case WeaponType.Pistol: crosshair.sprite = pistolCrosshair; break;
            case WeaponType.AR: crosshair.sprite = arCrosshair; break;
            case WeaponType.SMG: crosshair.sprite = smgCrosshair; break;
            case WeaponType.Shotgun: crosshair.sprite = shotgunCrosshair; break;
            default: crosshair.sprite = pistolCrosshair; break;
        }
    }

    private void OnPlayerDeath()
    {
        int waves = gameManager.totalWavesCompleted;
        int currentHighScore = SaveLoadManager.Instance.LoadHighScoreW();

        if (waves > currentHighScore)
        {
            SaveLoadManager.Instance.SaveHighScore(waves,0);
            Debug.Log($"New high score: {waves} waves!");
        }
        else
        {
            Debug.Log($"Waves survived: {waves}. Current high score: {currentHighScore}");
        }

        survivedWaves.text = $"Survived Waves: {waves}";

        player.GetComponent<InputManager>()?.ShowCursor();

        StartCoroutine(HandleGameOverSequence());
    }

    private IEnumerator HandleGameOverSequence()
    {
        yield return StartCoroutine(FadeInGameOver(1.5f));

        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene("MainMenu");
    }

    private void OnAimChanged(bool isAiming)
    {
        crosshair.gameObject.SetActive(!isAiming);
    }

    private IEnumerator FadeInGameOver(float duration)
    {
        gameOverImage.gameObject.SetActive(true);
        Color color = gameOverImage.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / duration);
            gameOverImage.color = color;
            yield return null;
        }

        color.a = 1f;
        gameOverImage.color = color;
    }
}
