using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    string newGameScene = "Map_v1";

    void Start()
    {
        int highScoreW = SaveLoadManager.Instance.LoadHighScoreW();
        float highScoreT = SaveLoadManager.Instance.LoadHighScoreT(); /// TO DO
        highScoreUI.text = $"Best Wave Survived:\n{highScoreW}";
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
