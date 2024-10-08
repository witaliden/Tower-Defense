﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private SpriteRenderer towerPlace;
    [SerializeField] private SpriteRenderer towerHead;

    [SerializeField] private int shootPower = 1;
    [SerializeField] private float shootDistance = 1f;
    [SerializeField] private float shootDelay = 5f;
    [SerializeField] private float bulletSpeed = 1f;
    [SerializeField] private float bulletSplashRadius = 0f;

    [SerializeField] private Bullet bulletPrefab;

    private float runningShootDelay;
    private Enemy targetEnemy;
    private Quaternion targetRotation;

    // Służy do zapisywania pozycji przy przemieszczaniu wieży
    public Vector2? PlacePosition { get; private set; }

    // Służy do umieszczenia ikonki wieży
    public Sprite GetTowerHeadIcon()
    {
        return towerHead.sprite;
    }

    public void SetPlacePosition(Vector2? newPosition)
    {
        PlacePosition = newPosition;
    }

    public void LockPlacement()
    {
        transform.position = (Vector2)PlacePosition;
    }

    // Zmienia kolejność warstwy wieży
    public void ToggleOrderInLayer(bool toFront)
    {
        int orderInLayer = toFront ? 2 : 0;
        towerPlace.sortingOrder = orderInLayer;
        towerHead.sortingOrder = orderInLayer;
    }

    // Sprawdza czy wróg w zasięgu
    public void CheckNearestEnemy(List<Enemy> enemies)
    {
        if (targetEnemy != null)
        {
            if (!targetEnemy.gameObject.activeSelf || Vector3.Distance(transform.position, targetEnemy.transform.position) > shootDistance)
            {
                targetEnemy = null;
            }
            else
            {
                return;
            }
        }

        float nearestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > shootDistance)
            {
                continue;
            }
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        targetEnemy = nearestEnemy;
    }

    // Strzelanie do celu
    public void ShootTarget()
    {
        if (targetEnemy == null)
        {
            return;
        }

        runningShootDelay -= Time.unscaledDeltaTime;
        if (runningShootDelay <= 0f)
        {
            bool headHasAimed = Mathf.Abs(towerHead.transform.rotation.eulerAngles.z - targetRotation.eulerAngles.z) < 10f;
            if (!headHasAimed)
            {
                return;
            }
            Bullet bullet = LevelManager.Instance.GetBulletFromPool(bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.SetProperties(shootPower, bulletSpeed, bulletSplashRadius);
            bullet.SetTargetEnemy(targetEnemy);
            bullet.gameObject.SetActive(true);
            runningShootDelay = shootDelay;
        }
    }

    // Sprawdza czy wieża ma cel
    public void SeekTarget()
    {
        if (targetEnemy == null)
        {
            return;
        }
        Vector3 direction = targetEnemy.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));
        towerHead.transform.rotation = Quaternion.RotateTowards(towerHead.transform.rotation, targetRotation, Time.deltaTime * 180f);
    }
}
