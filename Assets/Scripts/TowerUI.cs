using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image towerIcon;

    private Tower tower, currentSpawnedTower;


    public void SetTowerPrefab(Tower tower)
    {
        this.tower = tower;
        towerIcon.sprite = tower.GetTowerHeadIcon();
    }

    //Służy do tworzenia nowej wieży na planszy
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject newTowerObj = Instantiate(tower.gameObject);
        currentSpawnedTower = newTowerObj.GetComponent<Tower>();
        currentSpawnedTower.ToggleOrderInLayer(true);
    }

    //Służy do przemieszczania wieży na planszy
    public void OnDrag(PointerEventData eventData)
    {
        Camera mainCamera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        currentSpawnedTower.transform.position = targetPosition;
    }

    //Służy do zapisywania wieży na planszy
    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentSpawnedTower.PlacePosition == null)
        {
            Destroy(currentSpawnedTower.gameObject);
        }
        else
        {
            currentSpawnedTower.LockPlacement();
            currentSpawnedTower.ToggleOrderInLayer(false);
            LevelManager.Instance.RegisterSpawnedTower(currentSpawnedTower);
            currentSpawnedTower = null;
        }
    }
}
