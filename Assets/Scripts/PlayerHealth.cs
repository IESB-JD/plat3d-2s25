using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Saúde")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("UI (Opcional)")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color criticalColor = Color.red;

    [Header("Efeitos (Opcional)")]
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private AudioClip damageSound;

    // Componentes
    private AudioSource audioSource;

    // Eventos
    public System.Action OnPlayerDeath;
    public System.Action<float, float> OnHealthChanged; // current, max

    // Propriedades
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsAlive => currentHealth > 0;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // Efeitos visuais e sonoros
        ShowDamageEffects();
        PlayDamageSound();

        // Atualizar UI
        UpdateHealthUI();

        // Disparar eventos
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Verificar se morreu
        if (currentHealth <= 0)
        {
            Die();
        }

        Debug.Log($"Jogador recebeu {damage} de dano. Vida atual: {currentHealth}/{maxHealth}");
    }

    public void Heal(float healAmount)
    {
        if (!IsAlive) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"Jogador curou {healAmount} pontos. Vida atual: {currentHealth}/{maxHealth}");
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthUI();
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = HealthPercentage;
        }

        if (healthFillImage != null)
        {
            healthFillImage.color = Color.Lerp(criticalColor, healthyColor, HealthPercentage);
        }
    }

    private void ShowDamageEffects()
    {
        if (damageEffect != null)
        {
            GameObject effect = Instantiate(damageEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    private void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }

    private void Die()
    {
        Debug.Log("Jogador morreu!");
        OnPlayerDeath?.Invoke();
        
        // Aqui você pode adicionar lógica de game over
        // Por exemplo: mostrar tela de game over, reiniciar level, etc.
    }

    // Método para debug no editor
    [ContextMenu("Testar Dano")]
    void TestDamage()
    {
        TakeDamage(10f);
    }

    [ContextMenu("Testar Cura")]
    void TestHeal()
    {
        Heal(20f);
    }
}