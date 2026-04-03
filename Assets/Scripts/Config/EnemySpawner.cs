using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class EnemySpawner : NetworkBehaviour
{
    public NetworkObject enemyPrefab;
    public float respawnTime = 8f;
    
    private NetworkObject spawnedEnemy;
    private Vector3 spawnPosition;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        
        spawnPosition = transform.position;
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (!IsServer) return;

        spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemy.Spawn();
        
        // Theo dõi việc Object bị hủy thông qua một Coroutine kiểm tra
        StartCoroutine(MonitorEnemyStatus());
    }

    private IEnumerator MonitorEnemyStatus()
    {
        // Chờ cho đến khi con quái không còn tồn tại nữa
        // (Khi Despawn() được gọi, object sẽ bị Destroy)
        yield return new WaitUntil(() => spawnedEnemy == null);

        // Bắt đầu đếm ngược hồi sinh
        yield return new WaitForSeconds(respawnTime);
        
        SpawnEnemy();
    }
}