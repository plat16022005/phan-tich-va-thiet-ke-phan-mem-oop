using UnityEngine;
using Unity.Netcode;

public class Weapon : NetworkBehaviour
{
    public int damage = 20;
    public Collider2D hitbox;

    private bool canHit = false;

    public void EnableHitbox()
    {
        canHit = true;
        hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        canHit = false;
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner || !canHit) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            DealDamageServerRpc(enemy.NetworkObject);
        }
    }

    [ServerRpc]
    void DealDamageServerRpc(NetworkObjectReference enemyRef, ServerRpcParams rpcParams = default)
    {
        if (enemyRef.TryGet(out NetworkObject obj))
        {
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // 🔥 gửi về đúng client vừa đánh
                UpdateEnemyUIClientRpc(
                    enemy.NameEnemy,
                    enemy.GetCurrentHP(),
                    enemy.maxHealth,
                    new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[] { rpcParams.Receive.SenderClientId }
                        }
                    });
            }
        }
    }
    [ClientRpc]
    void UpdateEnemyUIClientRpc(string nameEnemy, int hpcurrent, int hpmax, ClientRpcParams rpcParams = default)
    {
        if (!IsOwner) return;
        EnemyUIManager ui = FindObjectOfType<EnemyUIManager>(true);
        if (ui != null)
        {
            ui.ShowEnemyInfo(nameEnemy, hpcurrent, hpmax);
        }
    }
}