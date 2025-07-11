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
    public TextMeshProUGUI CurrentAmmoUI;
    public TextMeshProUGUI MagazineAmmoUI;
    public Image AmmoTypeUI;
    public Image ActiveWeaponUI;
    public Sprite EmptySlot;

    [Header("Health")]
    public TextMeshProUGUI CurrentHealth;
    public Slider HealthSlider;
    public Gradient HealthGradient;
    public Image HealthFill;

    [Header("Crosshair")]
    public Image Crosshair;
    public Sprite PistolCrosshair;
    public Sprite ShotgunCrosshair;
    public Sprite ArCrosshair;
    public Sprite SmgCrosshair;

    [SerializeField]
    private FPSController _player;
    public Image GameOverImage;
    public TextMeshProUGUI SurvivedWaves;
    public Image WinImage;
    
    private Gun _activeWeapon;
    private Health _playerHealth;

    private string _ammoSpritesPath = "Weapons/UI/Ammo";
    private string _weaponIconsPath = "Weapons/UI/Icons";

    private Dictionary<string, Sprite> _ammoSprites;
    private Dictionary<string, Sprite> _weaponIcons;

    public GameManager GameManager;

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
        foreach (var sp in Resources.LoadAll<Sprite>(_ammoSpritesPath))
            _ammoSprites[sp.name] = sp;

        _weaponIcons = new Dictionary<string, Sprite>();
        foreach (var sp in Resources.LoadAll<Sprite>(_weaponIconsPath))
            _weaponIcons[sp.name] = sp;
    }

    private void Start()
    {
        if (_player == null) return;
        _player.OnWeaponChanged += OnWeaponChanged;
        OnWeaponChanged(_player.CurrentWeapon);

        _playerHealth = _player.GetComponentInChildren<Health>();
        if (_playerHealth != null)
        {
            _playerHealth.OnMaxHealthSet.AddListener(SetMaxHealth);
            _playerHealth.OnHealthChanged.AddListener(SetHealth);
            _playerHealth.OnDeath.AddListener(OnPlayerDeath);
        }
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnWeaponChanged -= OnWeaponChanged;
            if (_activeWeapon != null)
                _activeWeapon.OnAimStateChanged -= OnAimChanged;
        }
        UnsubscribeAmmo();

        if (_playerHealth != null)
        {
            _playerHealth.OnMaxHealthSet.RemoveListener(SetMaxHealth);
            _playerHealth.OnHealthChanged.RemoveListener(SetHealth);
            _playerHealth.OnDeath.RemoveListener(OnPlayerDeath);
        }
    }

    private void OnWeaponChanged(Gun newWeapon)
    {
        UnsubscribeAmmo();

        _activeWeapon = newWeapon;
        if (_activeWeapon != null)
        {
            UpdateAmmoUI(_activeWeapon.GetCurrentAmmo, _activeWeapon.GunData.MagazineSize);
            AmmoTypeUI.sprite = GetAmmoSprite(_activeWeapon.GunData.WeaponType);
            ActiveWeaponUI.sprite = GetWeaponIcon(_activeWeapon.GunData.GunName);
            _activeWeapon.OnAmmoChanged += UpdateAmmoUI;
            _activeWeapon.OnAimStateChanged += OnAimChanged;
            UpdateCrosshair(_activeWeapon.GunData.WeaponType);
        }
        else
        {
            CurrentAmmoUI.text = "";
            MagazineAmmoUI.text = "";
            AmmoTypeUI.sprite = EmptySlot;
            ActiveWeaponUI.sprite = EmptySlot;
            Crosshair.sprite = EmptySlot;
        }

    }

    private void UnsubscribeAmmo()
    {
        if (_activeWeapon != null)
            _activeWeapon.OnAmmoChanged -= UpdateAmmoUI;
    }

    private void UpdateAmmoUI(int current, int magazine)
    {
        CurrentAmmoUI.text = current.ToString();
        MagazineAmmoUI.text = $"/{magazine}";
    }

    private Sprite GetAmmoSprite(WeaponType type)
    {
        string ammoName = type.ToString() + "_Ammo";
        if (!_ammoSprites.TryGetValue(ammoName, out var sprite))
            return EmptySlot;
        return sprite;
    }

    private Sprite GetWeaponIcon(string iconName)
    {
        if (string.IsNullOrEmpty(iconName) || !_weaponIcons.TryGetValue(iconName, out var sprite))
            return EmptySlot;
        return sprite;
    }

    private void SetMaxHealth(float max)
    {
        HealthSlider.maxValue = max;
        HealthSlider.value = max;
        HealthFill.color = HealthGradient.Evaluate(1f);
        CurrentHealth.text = $"{max}|{max}";
    }

    private void SetHealth(float current)
    {
        HealthSlider.value = current;
        HealthFill.color = HealthGradient.Evaluate(HealthSlider.normalizedValue);
        CurrentHealth.text = $"{current}|{HealthSlider.maxValue}";
    }

    private void UpdateCrosshair(WeaponType type)
    {
        switch (type)   
        {
            case WeaponType.Pistol: Crosshair.sprite = PistolCrosshair; break;
            case WeaponType.AR: Crosshair.sprite = ArCrosshair; break;
            case WeaponType.SMG: Crosshair.sprite = SmgCrosshair; break;
            case WeaponType.Shotgun: Crosshair.sprite = ShotgunCrosshair; break;
            default: Crosshair.sprite = PistolCrosshair; break;
        }
    }

    private void OnPlayerDeath()
    {
        int waves = GameManager.totalWavesCompleted;
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

        SurvivedWaves.text = $"Survived Waves: {waves}";

        _player.GetComponent<InputManager>()?.ShowCursor();

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
        Crosshair.gameObject.SetActive(!isAiming);
    }

    private IEnumerator FadeInGameOver(float duration)
    {
        GameOverImage.gameObject.SetActive(true);
        Color color = GameOverImage.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / duration);
            GameOverImage.color = color;
            yield return null;
        }

        color.a = 1f;
        GameOverImage.color = color;
    }
}
