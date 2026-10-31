using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Tilemaps.Tile;

[RequireComponent(typeof(AABBCollider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _jumpSpeed = 12f;
    [SerializeField] private float _gravity = -30f;

    private AABBCollider _collider;
    private Vector2 _velocity;
    private float _moveInput;
    private bool _jumpPressed;
    private bool _isGrounded;
    private bool _restarting;

    private void Awake()
    {
        _collider = GetComponent<AABBCollider>();
    }

    private void OnEnable()
    {
        _collider.Collided += OnCollided;
    }

    private void OnDisable()
    {
        _collider.Collided -= OnCollided;
    }

    private void Update()
    {
        _moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        bool canJump = _isGrounded;
        _isGrounded = false;

        if (_jumpPressed && canJump)
        {
            _velocity.y = _jumpSpeed;
        }
        _jumpPressed = false;

        _velocity.x = _moveInput * _moveSpeed;
        _velocity.y += _gravity * Time.fixedDeltaTime;

        transform.position += (Vector3)(_velocity * Time.fixedDeltaTime);
    }

    private void OnCollided(AABBCollider other)
    {
        switch (other.Type)
        {
            case ColliderType.FallZone:
                Restart();
                break;

            case ColliderType.Void:
                RestartGame();
                break;

            case ColliderType.Ground:
            case ColliderType.Obstacle:
                ResolveSolid(other);
                break;
        }
    }

    private void ResolveSolid(AABBCollider other)
    {
        Vector2 aMin = _collider.min, aMax = _collider.max;
        Vector2 bMin = other.min, bMax = other.max;

        float overlapX = Mathf.Min(aMax.x, bMax.x) - Mathf.Max(aMin.x, bMin.x);
        float overlapY = Mathf.Min(aMax.y, bMax.y) - Mathf.Max(aMin.y, bMin.y);

        if (overlapX <= 0f || overlapY <= 0f) return;

        Vector3 pos = transform.position;

        if (overlapY < overlapX)
        {
            if (transform.position.y > other.transform.position.y)
            {
                pos.y += overlapY;
                if (_velocity.y < 0f) _velocity.y = 0f;
                _isGrounded = true;
            }
            else
            {
                pos.y -= overlapY;
                if (_velocity.y > 0f) _velocity.y = 0f;
            }
        }
        else
        {
            pos.x += transform.position.x > other.transform.position.x ? overlapX : -overlapX;
        }

        transform.position = pos;
    }

    private void Restart()
    {
        if (_restarting) return;
        _restarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RestartGame()
    {
        if (_restarting) return;
        _restarting = true;

        if (CollisionManager.Instance != null)
        {
            CollisionManager.Instance.Shutdown();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}