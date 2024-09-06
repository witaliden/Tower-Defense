using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    private Tower placedTower;

    // Służy do wykrywania kolizji z innymi obiektami
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (placedTower != null)
        {
            return;
        }

        Tower tower = collision.GetComponent<Tower>();
        
        if (tower != null)
        {
            tower.SetPlacePosition(transform.position);
            placedTower = tower;
        }
    }

    // Służy do wykrywania zakończenia kolizji z innymi obiektami
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (placedTower == null)
        {
            return;
        }
        placedTower.SetPlacePosition(null);
        placedTower = null;
    }
}
