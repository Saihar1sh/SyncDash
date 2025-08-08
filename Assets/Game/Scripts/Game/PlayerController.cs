using Arixen.ScriptSmith;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jumping")]
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody rb;
    private bool isGrounded;
    
    [Header("Effects")]
    public ParticleSystem collectEffect;

    

    
    
    void OnEnable()
    {
        EventBusService.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    void OnDisable()
    {
        EventBusService.UnSubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState == GameState.Playing)
        {
            ResetPlayer();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);

        if (GameManager.Instance.currentState == GameState.Playing && isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            Jump();
        }
    }

    void Jump()
    {
        var velocity = rb.velocity;
        velocity = new Vector3(velocity.x, 0, velocity.z);
        rb.velocity = velocity;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        float jumpDuration = 2 * (jumpForce / rb.mass) / Physics.gravity.magnitude;

        EventBusService.InvokeEvent(new PlayerJumpEvent() { Position = transform.localPosition, JumpDuration = jumpDuration });
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            GameManager.Instance.AddScore(10);
            Collectible collectible = other.gameObject.GetComponent<Collectible>();
            collectible.Collect();
            
            if (collectEffect != null)
            {
                collectEffect.transform.position = other.transform.position;
                collectEffect.Play();
            }

            EventBusService.InvokeEvent(new PlayerCollectEvent { Position = other.transform.localPosition, CollectibleID = collectible.collectibleID });
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Collision with obstacle");
            EventBusService.InvokeEvent(new PlayerCollideEvent { Position = other.transform.localPosition });

            var dissolve = other.gameObject.GetComponent<DissolveEffectController>();
            if (dissolve != null)
            {
                dissolve.StartDissolve();
            }
            else
            {
                other.gameObject.SetActive(false);
            }
        }
    }

    public void ResetPlayer()
    {
        transform.position = new Vector3(0, 1, 0);
        rb.velocity = Vector3.zero;
    }
}