using UnityEngine;
using UnityEngine.Rendering;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    public string highScoreKeyW = "BestWave";
    public string highScoreKeyT = "BestTime"; // TO DO

    private void Awake()
    {
        if (Instance != null && Instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
            DontDestroyOnLoad(gameObject);
    }

    public void SaveHighScore(int waves,float time)
    {
        PlayerPrefs.SetInt(highScoreKeyW, waves);
        PlayerPrefs.SetFloat(highScoreKeyT, time);
    }

    public int LoadHighScoreW()
    {
        if (PlayerPrefs.HasKey(highScoreKeyW))
        {
            return PlayerPrefs.GetInt(highScoreKeyW);
        }
        else 
        { 
            return 0; 
        }
    }
    public float LoadHighScoreT()
    {
        if (PlayerPrefs.HasKey(highScoreKeyT))
        {
            return PlayerPrefs.GetInt(highScoreKeyT);
        }
        else 
        { 
            return 0; 
        }
    }
}
