using UnityEngine;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    public float cooldownTime = 5f; // Tempo do cooldown em segundos
    private float cooldownTimer;
    public Image cooldownImage;
    public KeyCode activationKey; // Tecla que ativa o cooldown visual

    void Start()
    {
        cooldownImage.fillAmount = 1; // Começa preenchido, indicando que a skill está disponível
    }

    void Update()
    {
        // Checa se a tecla de ativação foi pressionada e o cooldown já terminou
        if (Input.GetKeyDown(activationKey) && cooldownTimer <= 0)
        {
            StartCooldown();
        }

        // Atualiza o preenchimento da imagem de cooldown durante o tempo de cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownImage.fillAmount = 1 - (cooldownTimer / cooldownTime);
        }
    }

    public void StartCooldown()
    {
        cooldownTimer = cooldownTime;
        cooldownImage.fillAmount = 0; // Inicia o cooldown esvaziando a imagem
    }
}
