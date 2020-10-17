using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float STUN_DURATION = 0.3f;

    public enum PlayerState { Idle, Running, Falling, GettingHit, Dying }

    [SerializeField] float movementSpeed = 1f;

    SpriteRenderer _spriteRenderer;
    Animator _animator;
    CharacterController2D _characterController;
    Health _health;

    PlayerState _lastState;
    PlayerState _currentState;
    float _moveSpeed;
    bool _hasJumped, _isJumping, _isSecondJumping;
    bool _isDead, _isStunned;
    bool _canPlay = true;

    Coroutine _gettingHitCoroutine;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController2D>();
        _characterController.OnFallEvent.AddListener(() => SetPlayerState(PlayerState.Falling));
        _characterController.OnLandEvent.AddListener(() => SetPlayerState(PlayerState.Idle));

        _health = GetComponent<Health>();
        _health.OnHit += (float f) => SetPlayerState(PlayerState.GettingHit);
        _health.OnDeath += (float f) => { _isStunned = false; SetPlayerState(PlayerState.Dying); };

        Events.OnLevelStart += OnLevelStart;
        Events.OnLevelEnd += OnLevelEnd;
    }

    private void OnDestroy()
    {
        Events.OnLevelStart -= OnLevelStart;
        Events.OnLevelEnd -= OnLevelEnd;
    }

    void Update()
    {
        if (!_canPlay)
            return;

        if (_isDead)
            return;

        switch (_currentState)
        {
            case PlayerState.Idle:
                IdleState();
            break;
            case PlayerState.Running:
                RunningState();
            break;
            case PlayerState.Falling:
                FallingState();
            break;
            case PlayerState.GettingHit:
                GettingHitState();
                break;
            case PlayerState.Dying:
                DyingState();
                break;
        }

        _lastState = _currentState;
    }

    private void FixedUpdate()
    {
        _characterController.Move(_moveSpeed * Time.fixedDeltaTime, false, _hasJumped);
        _hasJumped = false;
    }

    float GetMovementSpeed()
    {
        return Input.GetAxisRaw("Horizontal") * movementSpeed;
    }

    void DoJump()
    {
        if (_isSecondJumping)
            return;

        if (Input.GetButtonDown("Jump"))
        {
            if (!_isJumping)
            {
                _isJumping = true;
                _hasJumped = true;
            }
            else
            {
                _isSecondJumping = true;
                _hasJumped = true;
            }
        }
    }

    void SetPlayerState(PlayerState state)
    {
        if (_lastState == state || _lastState == PlayerState.Dying)
            return;

        _currentState = state;
        _animator.Play(state.ToString());

        Debug.Log(state.ToString());
    }

    #region States
    void IdleState()
    {
        _isJumping = false;
        _isSecondJumping = false;

        _moveSpeed = GetMovementSpeed();
        if (Mathf.Abs(_moveSpeed) > 0f)
            SetPlayerState(PlayerState.Running);

        DoJump();
    }

    void RunningState()
    {
        _isJumping = false;
        _isSecondJumping = false;

        _moveSpeed = GetMovementSpeed();
        if (Mathf.Abs(_moveSpeed) == 0f)
            SetPlayerState(PlayerState.Idle);

        DoJump();
    }

    void FallingState()
    {
        _isJumping = true;
        _moveSpeed = GetMovementSpeed();

        DoJump();
    }

    void GettingHitState()
    {
        _isStunned = true;
        _moveSpeed = 0f;

        if(_gettingHitCoroutine == null)
            _gettingHitCoroutine = StartCoroutine(GettingHitRoutine());
    }
    IEnumerator GettingHitRoutine()
    {
        yield return new WaitForSeconds(STUN_DURATION);

        _isStunned = false;
        _gettingHitCoroutine = null;
        SetPlayerState(PlayerState.Idle);
    }

    void DyingState()
    {
        _isDead = true;
        _moveSpeed = 0f;

        StopAllCoroutines();
    }
    #endregion
    #region Events
    void OnLevelStart()
    {
        _canPlay = true;
    }

    void OnLevelEnd()
    {
        _canPlay = false;
    }

    #endregion
}