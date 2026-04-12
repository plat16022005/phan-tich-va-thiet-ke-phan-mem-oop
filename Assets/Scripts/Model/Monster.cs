using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public static Monster instance;
    public Monster(){
    }
    public string NameMonster = "Monster";
    public int maxHealth = 100;
    private int currentHealth;

    public Animator animator;
    public Rigidbody2D rb;

    [Header("Di chuyển")]
    public float speed = 2f;
    public float moveDistance = 4f;

    private Vector3 startPos;
    private int direction = 1;
    private bool isDead = false;

    private void Awake()
    {
        instance = this;
        currentHealth = maxHealth;
        startPos = transform.position; // đúng chuẩn
    }

    void Update()
    {
        if (isDead) return;
        Patrol();
    }

    void Patrol()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= moveDistance)
        {
            direction *= -1;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        DungeonManager.instance.process -= 1;
        Dungeon.instance.FinishDungeon();
        isDead = true;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        Destroy(gameObject, 2f);
    }
    public void DestroyMonsterofDungeon()
    {
        Monster[] monsters = FindObjectsOfType<Monster>();

        foreach (Monster m in monsters)
        {
            Destroy(m.gameObject);
        }
    }
}
