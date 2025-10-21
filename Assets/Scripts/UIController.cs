using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text cristalText;
    public TMP_Text hpText;
    
    private void OnEnable()
    {
        PlayerController.OnHpChanged += OnHPChanged;
        PlayerController.OnPlayerDied += OnPlayerDied;
        PlayerController.OnCristalCollected += OnCristalCollected;
    }

    private void OnDisable()
    {
        PlayerController.OnHpChanged -= OnHPChanged;
        PlayerController.OnPlayerDied -= OnPlayerDied;
        PlayerController.OnCristalCollected -= OnCristalCollected;
    }
    
    private void OnHPChanged(float currentHealth)
    {
        hpText.text = "HP: " + currentHealth;
    }
    
    private void OnCristalCollected(int cristalAmount)
    {
        cristalText.text = "Cristal: " + cristalAmount;
    }
    
    private void OnPlayerDied()
    {
        gameOverPanel.SetActive(true);
    }
}
