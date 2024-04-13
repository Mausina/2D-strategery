using UnityEngine;
using Mirror;

// Обовязково міняти MonoBehaviour на NetworkBehaviour в усіх скриптах
public class Player : NetworkBehaviour
{
    public GameManager gameManager;
    public int coins;

    private Rigidbody2D rb;
    public float speed;
    private Vector2 input;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        gameManager.globalCoinsText.text = "Global Coins: " + gameManager.globalCoins;
        gameManager.coinsText.text = "Coins: " + coins;
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Flip();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + input * speed / 100);
    }

    private void Flip()
    {
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            coins++;

            RpcGlobalCoins();
        }
    }

    // [ClientRpc] Всім клієнтам
    [ClientRpc]
    public void RpcGlobalCoins()
    {
        gameManager.globalCoins++;
    }
}
