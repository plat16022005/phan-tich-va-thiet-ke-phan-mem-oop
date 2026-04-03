using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Enemy : NetworkBehaviour
{
    public string NameEnemy = "Enemy";
    public int maxHealth = 100;
    public NetworkObject enemyPrefab;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public Animator animator;
    public Rigidbody2D rb;

    [Header("Di chuyển")]
    public float speed = 2f;
    public float moveDistance = 3f;

    private Vector3 startPos;
    private int direction = 1;
    private bool isDead = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            startPos = transform.position;
        }
    }

    void Update()
    {
        if (!IsServer || isDead) return;

        Patrol();
        UpdateAnimator();
    }

    void Patrol() 
    { // di chuyển 
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime); // kiểm tra khoảng cách so với vị trí ban đầu 
        if (Mathf.Abs(transform.position.x - startPos.x) >= moveDistance) 
        { 
            direction *= -1; // đổi hướng 
            Vector3 scale = transform.localScale; 
            scale.x *= -1; 
            transform.localScale = scale;
        } 
    }

    void UpdateAnimator()
    {
        animator.SetBool("Running", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

    // ===== Combat =====
    public void TakeDamage(int damage)
    {
        if (!IsServer || isDead) return;

        currentHealth.Value -= damage;
        Debug.Log(currentHealth.Value);


        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        EnemyUIManager ui = FindObjectOfType<EnemyUIManager>(true);
        if (ui != null)
        {
            ui.HideEnemyInfo();
        }

        if (IsServer)
        {
            StartCoroutine(RespawnAndDespawn());
        }
    }
    private IEnumerator RespawnAndDespawn()
    {
        yield return new WaitForSeconds(8f);

        // Spawn enemy mới
        NetworkObject newEnemy = Instantiate(enemyPrefab, startPos, Quaternion.identity);
        newEnemy.Spawn();

        // Despawn enemy cũ
        NetworkObject.Despawn();
    }
    public int GetCurrentHP()
    {
        return currentHealth.Value;
    }
}