using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jumping")]
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody rb;
    private bool isGrounded;

    [Header("Syncing")]
    public LocalSyncManager syncManager;

    [Header("Effects")]
    public ParticleSystem collectEffect;

    private const string ObstacleLayerName = "Obstacle";
    private const string CollectibleLayerName = "Collectible";
    
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

        var jumpAction = new PlayerAction(ActionType.Jump, Time.time,transform.localPosition, duration: jumpDuration);
        syncManager.QueueAction(jumpAction);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(ObstacleLayerName))
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
            GameManager.Instance.ResetSpeedMultiplier();

            var collideAction = new PlayerAction(ActionType.Collide, Time.time,collision.transform.localPosition);
            syncManager.QueueAction(collideAction);

            var dissolve = collision.gameObject.GetComponent<DissolveEffectController>();
            if (dissolve != null)
            {
                dissolve.StartDissolve();
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(CollectibleLayerName))
        {
            GameManager.Instance.AddScore(10);
            other.gameObject.GetComponent<Collectible>().Collect();
            
            if (collectEffect != null)
            {
                collectEffect.transform.position = other.transform.position;
                collectEffect.Play();
            }

            var collectAction = new PlayerAction(ActionType.Collect, Time.time, other.transform.localPosition, collectibleId: other.gameObject.GetComponent<Collectible>().collectibleID);
            syncManager.QueueAction(collectAction);
        }
    }

    public void ResetPlayer()
    {
        transform.position = new Vector3(0, 1, 0);
        rb.velocity = Vector3.zero;
    }
}