using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Slope Detection")]
    [SerializeField] private float maxSlopeAngle = 40f;
    [SerializeField] private float slopeCheckDistance = 5f;

    [Header("Footstep Audio")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private List<AudioClip> footstepClips = new();
    [SerializeField] private float maxAudioTime = 0.1f;

    private Rigidbody rb;
    private Transform cameraTransform;

    private Vector2 moveInput = Vector2.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private bool isGrounded = false;
    private bool isFrozen = false;
    private float audioTimer = 0f;

    public void Initialize(Rigidbody rb, Transform cameraTransform)
    {
        this.rb = rb;
        this.cameraTransform = cameraTransform;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void Move()
    {
        if (rb == null || cameraTransform == null || isFrozen) return;

        isGrounded = CheckIfGrounded();
        ApplyGravity();

        // Dirección según la cámara (solo XZ)
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 inputDir = (right * moveInput.x + forward * moveInput.y).normalized;

        if (!CanMove(inputDir))
            return;

        Vector3 moveDirection = inputDir * moveSpeed;

        // Obtener la normal del suelo o rampa
        Vector3 groundNormal = Vector3.up;
        RaycastHit slopeHit;
        if (Physics.Raycast(rb.position, Vector3.down, out slopeHit, groundDistance + 0.6f, groundLayer))
        {
            groundNormal = slopeHit.normal;
        }

        // Proyectar el movimiento sobre la rampa
        Vector3 projected = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized * moveSpeed;

        // --- NUEVO ---
        // Si estamos en el aire y tocamos una pared, cancelamos el movimiento horizontal
        if (!isGrounded && IsTouchingWall())
        {
            projected = Vector3.zero; // solo dejamos la gravedad
        }

        Vector3 desiredVelocity = new Vector3(projected.x, rb.velocity.y, projected.z);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, desiredVelocity, ref currentVelocity, 0.05f);

        PlayFootstepAudio();
    }

    private void PlayFootstepAudio()
    {
        if (rb.velocity.magnitude < 0.1f || !isGrounded)
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();

            audioTimer = 0f;
            return;
        }

        audioTimer += Time.deltaTime;

        if (audioTimer < maxAudioTime || footstepSource.isPlaying)
            return;

        audioTimer = 0f;

        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) 
            return;

        Vector3 relativePos = GetMapPos();
        int mapX = Mathf.FloorToInt(relativePos.x * terrain.terrainData.alphamapWidth);
        int mapZ = Mathf.FloorToInt(relativePos.z * terrain.terrainData.alphamapHeight);

        float[,,] map = terrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        int maxTextures = terrain.terrainData.alphamapLayers;

        int maxIndex = 0;
        float maxValue = 0f;

        for (int i = 0; i < maxTextures; i++)
        {
            if (map[0, 0, i] > maxValue)
            {
                maxValue = map[0, 0, i];
                maxIndex = i;
            }
        }

        if (maxIndex < footstepClips.Count && footstepClips[maxIndex] != null)
        {
            footstepSource.clip = footstepClips[maxIndex];
            footstepSource.Play();
        }
    }


    public void Jump()
    {
        if (!isGrounded || rb == null) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    public void ApplyGravity()
    {
        if (rb == null) return;

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private bool CheckIfGrounded()
    {
        Vector3 origin = groundCheck.position + Vector3.up * 0.1f;
        Vector3 direction = Vector3.down;
        float rayLength = groundDistance + 0.1f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength))
        {
            // Verifica que la superficie no sea demasiado empinada
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle < maxSlopeAngle;
        }

        return false;
    }

    private bool IsTouchingWall()
    {
        float radius = 0.3f;
        float castDistance = 0.6f;
        Vector3 origin = rb.position + Vector3.up * 0.5f;

        return Physics.SphereCast(origin, radius, transform.forward, out _, castDistance, wallLayer)
            || Physics.SphereCast(origin, radius, -transform.forward, out _, castDistance, wallLayer)
            || Physics.SphereCast(origin, radius, transform.right, out _, castDistance, wallLayer)
            || Physics.SphereCast(origin, radius, -transform.right, out _, castDistance, wallLayer);
    }

    private bool CanMove(Vector3 moveDir)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return true;

        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(rb.position);
        float nextHeight = terrain.SampleHeight(rb.position + moveDir * slopeCheckDistance);

        return angle <= maxSlopeAngle || nextHeight <= currentHeight;
    }

    private Vector3 GetMapPos()
    {
        Vector3 pos = rb.position;
        Terrain terrain = Terrain.activeTerrain;

        return new Vector3(
            (pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
            0,
            (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z
        );
    }

    public bool IsGrounded() => isGrounded;

    public void SetRigidbody(Rigidbody newRb)
    {
        rb = newRb;
    }

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
    }
}

