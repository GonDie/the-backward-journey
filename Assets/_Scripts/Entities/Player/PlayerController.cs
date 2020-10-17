using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { Idle, Running, Falling }

    [SerializeField] float movementSpeed = 1f;

    Animator _animator;
    CharacterController2D _characterController;

    PlayerState _lastState;
    PlayerState _currentState;
    float _moveSpeed;
    bool _hasJumped, _isJumping, _isSecondJumping;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController2D>();
        _characterController.OnFallEvent.AddListener(() => SetPlayerState(PlayerState.Falling));
        _characterController.OnLandEvent.AddListener(() => SetPlayerState(PlayerState.Idle));
    }

    void Update()
    {
        switch (_currentState)
        {
            case PlayerState.Idle:
                IdleState();
            break;
            case PlayerState.Running:
                MovingState();
            break;
            case PlayerState.Falling:
                FallingState();
            break;
        }

        _lastState = _currentState;
    }

    private void FixedUpdate()
    {
        _characterController.Move(_moveSpeed * Time.fixedDeltaTime, false, _hasJumped);
        _hasJumped = false;
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

    void MovingState()
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
    #endregion

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
        if (_lastState == state)
            return;

        _currentState = state;
        _animator.Play(state.ToString());
    }
}