using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    public Button backButton;

    public GameObject settingsPanel;

    private void OnEnable()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
        backButton.onClick.AddListener(OnSettingsButtonClick);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
        quitButton.onClick.RemoveListener(OnQuitButtonClick);
        backButton.onClick.RemoveListener(OnSettingsButtonClick);
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("Bigisland");
    }

    private void OnSettingsButtonClick()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void OnQuitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}