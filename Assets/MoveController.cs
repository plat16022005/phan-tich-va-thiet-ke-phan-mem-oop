using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MoveController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Vector2 vt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        InputControl();
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     NetworkManager.Singleton.SceneManager.LoadScene("SceneNew", UnityEngine.SceneManagement.LoadSceneMode.Single);
        //     // hoặc SceneManager.LoadScene(1);
        // }
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     NetworkManager.Singleton.SceneManager.LoadScene("Test", UnityEngine.SceneManagement.LoadSceneMode.Single);
        // }
    }
    void FixedUpdate()
    {
        Move();
    }
    private void InputControl()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        vt = new Vector2(moveX, moveY).normalized;
    }
    private void Move()
    {
        rb.velocity = vt * moveSpeed;
    }
}
