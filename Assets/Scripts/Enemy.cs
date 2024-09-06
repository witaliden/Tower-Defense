using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private SpriteRenderer healthBar;
    [SerializeField] private SpriteRenderer healthFill;

    private int currentHealth;

    public Vector3 TargetPosition { get; private set; }
    public int CurrentPathIndex { get; private set; }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        healthFill.size = healthBar.size;
    }

    public void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, moveSpeed * Time.deltaTime);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        TargetPosition = targetPosition;
        healthBar.transform.parent = null;

        Vector3 distance = TargetPosition - transform.position;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Abs(distance.y) > Mathf.Abs(distance.x) ? (distance.y > 0 ? 90f : -90f) : (distance.x > 0 ? 0f : 180f));

        healthBar.transform.parent = transform;
    }

    public void SetCurrentPathIndex(int currentIndex)
    {
        CurrentPathIndex = currentIndex;
    }

    public void ReduceEnemyHealth(int damage)
    {
        currentHealth -= damage;
        AudioPlayer.Instance.PlaySFX("Hit");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameObject.SetActive(false);
            AudioPlayer.Instance.PlaySFX("Collapsed");
        }

        float healthPercentage = (float)currentHealth / maxHealth;
        healthFill.size = new Vector2(healthPercentage * healthBar.size.x, healthBar.size.y);
    }
}
