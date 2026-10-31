using System;
using UnityEngine;

public enum ColliderType
{
    Player,
    Ground,
    Obstacle,
    FallZone,
    Void
}

public class AABBCollider : MonoBehaviour
{
    [SerializeField] private float _length;
    [SerializeField] private float _width;
    [SerializeField] private ColliderType _type;

    public ColliderType Type => _type;

    public Vector2 max => new Vector2(
        transform.position.x + _width * .5f,
        transform.position.y + _length * .5f);

    public Vector2 min => new Vector2(
        transform.position.x - _width * .5f,
        transform.position.y - _length * .5f);

    public event Action<AABBCollider> Collided;

    private void Start()
    {
        if (CollisionManager.Instance != null)
        {
            CollisionManager.Instance.Register(this);
        }
    }

    private void OnDestroy()
    {
        if (CollisionManager.Instance != null)
        {
            CollisionManager.Instance.Unregister(this);
        }
    }

    private void Update()
    {
        var lo = min;
        var hi = max;

        Debug.DrawLine(new Vector3(lo.x, hi.y), new Vector3(hi.x, hi.y), Color.blueViolet);
        Debug.DrawLine(new Vector3(hi.x, hi.y), new Vector3(hi.x, lo.y), Color.blueViolet);
        Debug.DrawLine(new Vector3(hi.x, lo.y), new Vector3(lo.x, lo.y), Color.blueViolet);
        Debug.DrawLine(new Vector3(lo.x, lo.y), new Vector3(lo.x, hi.y), Color.blueViolet);
    }

    public void OnCollision(AABBCollider other)
    {
        Collided?.Invoke(other);
    }
}