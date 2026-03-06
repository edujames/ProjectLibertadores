using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Animator animator;
    private SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
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

        // Preservamos la Y que calcula la física (gravedad, caídas)
        // Solo controlamos X y Z manualmente
        rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, v * moveSpeed);

        // Animaciones
        bool isMoving = h != 0 || v != 0;
        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("moveX", h);
        animator.SetFloat("moveY", v);

        // Flip del sprite según dirección horizontal
        if (h < 0) sr.flipX = true;
        else if (h > 0) sr.flipX = false;
    }
}