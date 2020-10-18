using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _damage;
    [SerializeField] float _speed;
    [SerializeField] float _lifeTime;
    [SerializeField] LayerMask _targetLayer;

    Transform _transform;
    CircleCollider2D _collider;

    public void Init(float direction, SimpleEvent callback = null)
    {
        _collider = GetComponent<CircleCollider2D>();
        _transform = GetComponent<Transform>();
        _transform.localScale = new Vector3(direction, 1f, 1f);

        OnInit(direction);

        StartCoroutine(FlyRoutine(direction, callback));
    }

    IEnumerator FlyRoutine(float direction, SimpleEvent callback = null)
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;
            Move(direction);

            if (CheckCollision(direction, callback))
                break;

            if (time >= _lifeTime)
            {
                OnTimeOut();
                break;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    protected virtual void OnInit(float direction) { }

    protected virtual void Move(float direction)
    {
        _transform.position += Vector3.right * direction * _speed * Time.deltaTime;
    }

    protected virtual bool CheckCollision(float direction, SimpleEvent callback = null)
    {
        RaycastHit2D raycastHit = Physics2D.CircleCast(_transform.position, _collider.radius, Vector3.right * direction, 0.1f, _targetLayer);

        if (raycastHit.collider != null)
        {
            callback?.Invoke();
            raycastHit.collider.GetComponent<Health>().DealDamage(_damage);

            OnHit();

            return true;
        }

        return false;
    }

    protected virtual void OnHit() { }
    protected virtual void OnTimeOut() { }
}
