using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MoveController : NetworkBehaviour
{
    public Animator animator;
    public float speed = 5f;
    public float jumpForce = 2f;

    private Vector3 velocity;
    private bool isGrounded;
    [SerializeField] private Rigidbody2D rb;

    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;
    // [SerializeField] private GameObject spawnedObjectPrefab;
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData
    {
        _int = 56, _bool = true, message = ""
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        }; 
    }
    public struct MyCustomData: INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public string message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        UpdateControll();
        UpdateAnimator();
    }
    void UpdateAnimator()
    {
        animator.SetFloat("velocityY", rb.velocity.y);
        animator.SetBool("grounded", isGrounded);
        if (rb.velocity.x != 0)
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);
        animator.SetInteger("class", 0);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("attack");
                lastAttackTime = Time.time;
            }
        }
    }
    void UpdateControll()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");

        Vector2 velocity = rb.velocity;
        velocity.x = x * speed;
        rb.velocity = velocity;

        Vector3 scale = transform.localScale;

        if (x > 0)
            scale.x = Mathf.Abs(scale.x);
        else if (x < 0)
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log($"Test ServerRPC: {OwnerClientId}; {serverRpcParams.Receive.SenderClientId}");
    }
    [ClientRpc]
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log($"Test ClientRPC: {clientRpcParams.Send.TargetClientIds}");
    }
    [ServerRpc]
    void ChangeCustomDataServerRpc()
    {
        CustomData data = new CustomData();

        data.hair = Random.Range(0, 5);
        data.eyes = Random.Range(0, 5);
        data.nose = Random.Range(0, 5);
        data.mouth = Random.Range(0, 5);

        GetComponentInChildren<CustomNetworkManager>().customData.Value = data;
    }
}
