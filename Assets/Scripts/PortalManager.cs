using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public GameObject portalPrefab; // Prefab do portal (mesmo se for apenas visual)
    public Transform portalSpawnPoint; // Ponto de spawn do portal
    private bool portalSpawned = false;

    public void SpawnPortal()
    {
        if (!portalSpawned)
        {
            GameObject portal = Instantiate(portalPrefab, portalSpawnPoint.position, portalSpawnPoint.rotation);
            
            // Garantir que o portal tenha um Collider para detectar a colisão
            if (portal.GetComponent<Collider>() == null)
            {
                BoxCollider collider = portal.AddComponent<BoxCollider>();
                collider.isTrigger = true; // O Collider precisa ser um trigger para detectar a colisão
            }

            portalSpawned = true;
            Debug.Log("Portal spawned!");
        }
    }
}
