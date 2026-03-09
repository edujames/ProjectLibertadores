using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Animator animator;
    private SpriteRenderer sr;
    private AudioSource footstepAudio;

    // Guardamos el estado del frame anterior para detectar cambios
    private bool wasMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        footstepAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
        transform.position = new Vector3(transform.position.x, terrainHeight + 1f, transform.position.z);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, v * moveSpeed);

        bool isMoving = h != 0 || v != 0;

        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("moveX", h);
        animator.SetFloat("moveY", v);

        if (h < 0) sr.flipX = true;
        else if (h > 0) sr.flipX = false;

        // Solo actuamos cuando el estado CAMBIA, no cada frame
        if (isMoving && !wasMoving)
        {
            footstepAudio.Play();
        }
        else if (!isMoving && wasMoving)
        {
            footstepAudio.Stop();
        }

        wasMoving = isMoving;
    }
}