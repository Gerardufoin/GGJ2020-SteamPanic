using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    private const float MIN_GROUND_NORMAL_Y = 0.65f;
    private const float MIN_MAGNITUDE = 0.001f;
    private const float SHELL_RADIUS = 0.01f;

    public float GravityModifier = 3f;

    protected Rigidbody2D m_body;
    protected Vector2 m_velocity;
    protected Vector2 m_targetVelocity;
    protected Vector2 m_groundNormal;

    protected ContactFilter2D m_contactFilter;
    protected RaycastHit2D[] m_hits = new RaycastHit2D[16];

    private bool _isGrounded;
    private float _ungroundedTimer;
    protected float m_loseJumpDelay = 0.1f;
    public bool IsGrounded
    {
        get
        {
            return _isGrounded || (m_loseJumpDelay >= _ungroundedTimer);
        }
    }

    private void OnEnable()
    {
        m_body = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        m_contactFilter.useTriggers = false;
        m_contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        m_contactFilter.useLayerMask = true;
    }

    protected virtual void ComputeVelocity() { }

    protected void Update()
    {
        if (!_isGrounded)
        {
            _ungroundedTimer += Time.deltaTime;
        }
        m_targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    private void FixedUpdate()
    {
        m_velocity += GravityModifier * Physics2D.gravity * Time.deltaTime;
        m_velocity.x = m_targetVelocity.x;

        _isGrounded = false;

        Vector2 deltaPosition = m_velocity * Time.deltaTime;

        // To move following slopes
        Vector2 moveAlongGround = new Vector2(m_groundNormal.y, (m_groundNormal.y > 0 ? -m_groundNormal.x : 0));

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);

        if (_isGrounded)
        {
            _ungroundedTimer = 0f;
        }
    }

    void Movement(Vector2 move, bool yMovement = false)
    {
        float distance = move.magnitude;

        if (distance > MIN_MAGNITUDE)
        {
            int count = m_body.Cast(move, m_contactFilter, m_hits, distance + SHELL_RADIUS);
            for (int i = 0; i < count; ++i)
            {
                Vector2 currentNormal = m_hits[i].normal;
                if (currentNormal.y > MIN_GROUND_NORMAL_Y)
                {
                    _isGrounded = true;
                    if (yMovement)
                    {
                        m_groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(m_velocity, currentNormal);
                if (projection < 0)
                {
                    m_velocity = m_velocity - projection * currentNormal;
                }

                float modifiedDistance = m_hits[i].distance - SHELL_RADIUS;
                distance = Mathf.Min(modifiedDistance, distance);
            }

        }
        m_body.position += move.normalized * distance;
    }
}
