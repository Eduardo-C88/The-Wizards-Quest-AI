using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public int shieldDurability = 400; // Define a durabilidade do escudo

    public void TakeDamage(int damage)
    {
        shieldDurability -= damage;

        if (shieldDurability <= 0)
        {
            DestroyShield();
        }
    }

    private void DestroyShield()
    {
        // Efeito visual ou som de destruição do escudo (opcional)
        Debug.Log("Shield destroyed!");
        Destroy(gameObject); // Destroi o objeto do escudo
    }
}
