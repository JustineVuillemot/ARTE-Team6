using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureState
{
    Idle,
    Walk,
    Panic,
    Smashed
}

public class LivingCreature : MonoBehaviour
{
    public SpriteRenderer _sprite;
    public Animator _animator;
    public PanicTrigger _panicCollider;
    public SmashTrigger _smashCollider;
    public int _speed = 1;
    public float _minScale = 0.5f;
    public float _maxScale = 1.3f;

    private LineRenderer _line;
    private int _currentIndex;

    private CreatureState _state;
    private float _animTimer;

    private int _walkingDirection;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _animTimer -= Time.deltaTime;

        if(_animTimer <= 0)
        {
            switch (_state)
            {
                case CreatureState.Idle:
                    _animator.SetBool("Move", true);
                    _state = CreatureState.Walk;
                    _animTimer = Random.Range(3.0f, 5.0f);
                    break;
                case CreatureState.Walk:
                    _animator.SetBool("Move", false);
                    _state = CreatureState.Idle;
                    _animTimer = Random.Range(1.0f, 4.0f);
                    break;
                default:
                    _animator.SetBool("Move", false);
                    _animTimer = 0;
                    break;
            }
        }

        if(_state == CreatureState.Walk)
        {
            int nextIndex = _currentIndex + (_walkingDirection * _speed);
            
            if(nextIndex < 2)
            {
                nextIndex = 2;
                _walkingDirection = 1;
                _sprite.transform.localScale = new Vector3(_walkingDirection, _walkingDirection, 1);
            }

            if(nextIndex >= _line.positionCount)
            {
                nextIndex = _line.positionCount - 1;
                _walkingDirection = -1;
                _sprite.transform.localScale = new Vector3(_walkingDirection, _walkingDirection, 1);
            }

            _currentIndex = nextIndex;
            UpdatePosAndRotate();
        }
    }

    public void InitCreature(Color c, LineRenderer line, int indexOnLine)
    {
        if (_sprite == null)
        {
            Debug.LogError("pb with sprite renderer on object " + name);
        }
        else
        {
            _sprite.color = c;
        }
        
        _line = line;
        _currentIndex = indexOnLine;
        _animTimer = Random.Range(2.0f, 5.0f);
        _state = CreatureState.Idle;

        _walkingDirection = Random.Range(0, 2) == 0 ? -1 : 1;

        float scale = Random.Range(_minScale, _maxScale);
        transform.localScale = new Vector3(scale, scale);

        UpdatePosAndRotate();
    }

    private void UpdatePosAndRotate()
    {
        if(_currentIndex < 0 || _currentIndex >= _line.positionCount)
        {
            Debug.LogError("pb with current index : " + _currentIndex);
            gameObject.SetActive(false);
            return;
        }

        transform.position = _line.GetPosition(_currentIndex);

        int prevIndex = Mathf.Clamp(_currentIndex - 5, 2, _currentIndex);
        int nextIndex = Mathf.Clamp(_currentIndex + 5, _currentIndex, _line.positionCount - 1);

        Vector2 nextPos = _line.GetPosition(nextIndex);
        Vector2 previousPos = _line.GetPosition(prevIndex);

        Vector2 direction = nextPos - previousPos;
        Vector2 wantedDirection = Vector2.Perpendicular(direction);

        Vector3 wantedDirection3D = new Vector3(wantedDirection.x, wantedDirection.y, 0);
        transform.rotation = Quaternion.FromToRotation(Vector3.up, wantedDirection3D);
    }

    //what to do when a creature panic
    public void FromOkToPanic()
    {
        _state = CreatureState.Panic;
        _animator.SetBool("Panic", true);
    }

    //what to do when a creature stops panicking
    public void FromPanicToOk()
    {
        //it's over so we don't want to exit panic
        if (_state == CreatureState.Smashed)
        {
            return;
        }

        _state = CreatureState.Idle;
        _animator.SetBool("Panic", false);
        _animTimer = Random.Range(1.0f, 4.0f);
    }

    //what to do when a creature is smahed
    public void FromPanicToSmashed()
    {
        _state = CreatureState.Smashed;
        _animator.SetBool("Smash", true);
    }

    public void EnableColliders(bool enabled)
    {
        _panicCollider.enabled = enabled;
        _smashCollider.enabled = enabled;
    }

    public void SetColor(Color c)
    {
        _sprite.color = c;
    }
}
