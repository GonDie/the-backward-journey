using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MELEE_DURATION = 0.3f;
    const float RANGED_DURATION = 0.3f;
    const float STUN_DURATION = 0.3f;
    const float GRAVITY_SCALE = 3f;

    public enum PlayerState { Idle, Running, Falling, GettingHit, Dying, Defending, Dashing, AttackingMelee, AttackingRanged }

    [SerializeField] float movementSpeed = 1f;

    Transform _transform;
    Rigidbody2D _rigidbody2D;
    Animator _animator;
    SpriteRenderer _spriteRenderer;
    CharacterController2D _characterController;
    TrailRenderer _trailRenderer;
    Health _health;

    PlayerState _lastState;
    PlayerState _currentState;
    float _moveSpeed;
    bool _hasJumped, _isJumping, _isSecondJumping;
    bool _isDefending;
    bool _isDashing, _canDash;
    bool _isDead, _isStunned;
    bool _canPlay = true;

    Coroutine _gettingHitCoroutine, _dashCoroutine, _meleeCoroutine, _rangedCoroutine;

    Dictionary<string, BaseSkill> _skills;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _characterController = GetComponent<CharacterController2D>();
        _characterController.OnFallEvent.AddListener(OnFallEvent);
        _characterController.OnLandEvent.AddListener(OnLandEvent);

        _health = GetComponent<Health>();
        _health.OnHit += (float f) => SetPlayerState(PlayerState.GettingHit);
        _health.OnDeath += (float f) => { _isStunned = false; SetPlayerState(PlayerState.Dying); };

        Events.OnLevelStart += OnLevelStart;
        Events.OnLevelEnd += OnLevelEnd;

        _skills = new Dictionary<string, BaseSkill>();
        _skills.Add("DoubleJump", GetComponent<DoubleJumpSkill>());
        _skills.Add("Dash", GetComponent<DashSkill>());
        _skills.Add("Defend", GetComponent<DefendSkill>());
        _skills.Add("Melee", GetComponent<MeleeSkill>());
        _skills.Add("Ranged", GetComponent<RangedSkill>());
        _skills.Add("LifeSteal", GetComponent<LifeStealSkill>());
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
            case PlayerState.Defending:
                DefendingState();
                break;
            case PlayerState.Dashing:
                DashingState();
                break;
            case PlayerState.GettingHit:
                GettingHitState();
                break;
            case PlayerState.Dying:
                DyingState();
                break;
            case PlayerState.AttackingMelee:
                MeleeState();
                break;
            case PlayerState.AttackingRanged:
                RangedState();
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
            else if(_skills["DoubleJump"].Cast())
            {
                _isSecondJumping = true;
                _hasJumped = true;
            }
        }
    }

    void DoDefend()
    {
        if (!_skills["Defend"].Cast())
            return;

        if (Input.GetButtonDown("Defend"))
        {
            _isDefending = true;
            SetPlayerState(PlayerState.Defending);
        }
        else if (Input.GetButtonUp("Defend"))
        {
            _isDefending = false;
            SetPlayerState(PlayerState.Idle);
        }
    }

    void DoDash()
    {
        if (_canDash && _skills["Dash"].Cast())
        {
            if (Input.GetButtonDown("Dash"))
            {
                _canDash = false;
                SetPlayerState(PlayerState.Dashing);
            }
        }
    }

    void DoMelee()
    {
        if (Input.GetButtonDown("Melee"))
            if (_skills["Melee"].Cast(OnSkillConnect))
                SetPlayerState(PlayerState.AttackingMelee);
    }

    void DoRanged()
    {
        if (Input.GetButtonDown("Ranged"))
            if (_skills["Ranged"].Cast(OnSkillConnect))
                SetPlayerState(PlayerState.AttackingRanged);
    }

    void OnSkillConnect()
    {
        _skills["LifeSteal"].Cast();
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
        DoDefend();
        DoDash();
        DoMelee();
        DoRanged();
    }

    void RunningState()
    {
        _isJumping = false;
        _isSecondJumping = false;

        _moveSpeed = GetMovementSpeed();
        if (Mathf.Abs(_moveSpeed) == 0f)
            SetPlayerState(PlayerState.Idle);

        DoJump();
        DoDefend();
        DoDash();
        DoMelee();
        DoRanged();
    }

    void DefendingState()
    {
        _moveSpeed = GetMovementSpeed() / 4f;
        DoDefend();
        DoJump();
        DoDash();
    }

    void FallingState()
    {
        _isDefending = false;
        _isJumping = true;
        _moveSpeed = GetMovementSpeed();

        DoJump();
        DoDash();
        DoMelee();
        DoRanged();
    }

    void MeleeState()
    {
        _moveSpeed = 0f;

        if (_meleeCoroutine == null)
            _meleeCoroutine = StartCoroutine(MeleeCoroutine());
    }
    IEnumerator MeleeCoroutine()
    {
        yield return new WaitForSeconds(MELEE_DURATION);

        _meleeCoroutine = null;
        SetPlayerState(PlayerState.Idle);
    }

    void RangedState()
    {
        _moveSpeed = 0f;

        if (_rangedCoroutine == null)
            _rangedCoroutine = StartCoroutine(RangedCoroutine());
    }
    IEnumerator RangedCoroutine()
    {
        yield return new WaitForSeconds(RANGED_DURATION);

        _rangedCoroutine = null;
        SetPlayerState(PlayerState.Idle);
    }

    void DashingState()
    {
        if (_dashCoroutine == null)
            _dashCoroutine = StartCoroutine(DashCoroutine());

        DoJump();
    }
    IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
        _isDashing = true;
        _trailRenderer.emitting = true;
        _rigidbody2D.gravityScale = 0f;
        float move = GetMovementSpeed();
        _moveSpeed = Mathf.Sign(_transform.localScale.x) * 100f;

        yield return new WaitForSeconds(0.25f);

        _isDashing = false;
        _dashCoroutine = null;
        _trailRenderer.emitting = false;
        _rigidbody2D.gravityScale = GRAVITY_SCALE;

        if (_characterController.IsGrounded)
            _canDash = true;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);
        SetPlayerState(PlayerState.Idle);
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

    void OnFallEvent()
    {
        SetPlayerState(PlayerState.Falling);
    }

    void OnLandEvent()
    {
        _canDash = true; 
        SetPlayerState(PlayerState.Idle);
    }
    #endregion
}