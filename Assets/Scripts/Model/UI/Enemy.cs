using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Enemy : NetworkBehaviour
{
    public string NameEnemy = "Enemy";
    public int maxHealth = 100;
    public GameObject enemyPrefab;
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


    // ===== Combat =====
    [ServerRpc]
    public void TakeDamageServerRpc(int damage, ulong attackerId)
    {
        if (isDead) return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            Die(attackerId); // 🔥 truyền killer
        }
    }

    void Die(ulong killerId)
    {
        if (QuestManager.instance.CurrentQuest == 1 
            && QuestManager.instance.State == "Đang thực hiện" 
            && NameEnemy == "Slime")
        {
            QuestManager.instance.AddProcess(killerId, 1);
        }

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
        GameObject obj = Instantiate(enemyPrefab, startPos, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();

        // Despawn enemy cũ
        NetworkObject.Despawn();
    }
    public int GetCurrentHP()
    {
        return currentHealth.Value;
    }
    // [ClientRpc]
    // void UpdateQuestClientRpc(ulong clientId)
    // {
    //     // 🎯 chỉ client đúng mới cộng
    //     if (NetworkManager.Singleton.LocalClientId == clientId)
    //     {
    //         QuestManager.instance.process += 1;
    //         Debug.Log("Client " + clientId + " được cộng quest");
    //     }
    // }
}