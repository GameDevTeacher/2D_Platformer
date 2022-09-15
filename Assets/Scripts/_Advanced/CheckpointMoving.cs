using System;
using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;

[DisallowMultipleComponent]
public class CheckpointMoving : MonoBehaviour
{
    [Header("Checkpoints")]
    [Tooltip("First checkpoint")]
    [SerializeField] private MoveCheckpoint _firstCheckpoint = new MoveCheckpoint();
    [Tooltip("All checkpoints after first checkpoint")]
    [SerializeField] private MoveCheckpoint[] _otherCheckpoints = Array.Empty<MoveCheckpoint>();

    [Header("Gizmos")]
    [Tooltip("Doesn't draw gizmos when true (Double click to update unvalidated gizmos)")]
    [SerializeField] private bool g_disable = false;
    [Tooltip("Automatically set size to collider2D bounds")]
    [SerializeField] private bool g_useCollider2DSize = true;
    [Tooltip("Size of gizmo box")]
    [SerializeField] private Vector2 g_size = Vector2.one;
    [Tooltip("Color of gizmo")]
    [SerializeField] private Color g_color = new Color(0f, 1f, 1f, 0.5f);

    public MoveCheckpoint CurrentMoveCheckpoint { get; private set; }
    public MoveCheckpoint NextMoveCheckpoint { get; private set; }
    public Vector3 StartPosition { get; private set; }
    public bool IsWaiting { get; private set; }
    public bool IsMoving { get; private set; }

    public int CheckpointCount => _otherCheckpoints.Length + 1;
    public MoveCheckpoint FirstCheckpoint => _firstCheckpoint;
    public ReadOnlyCollection<MoveCheckpoint> OtherCheckpoints => Array.AsReadOnly(_otherCheckpoints);

    private IEnumerator Start()
    {
        StartPosition = transform.position;

        for (int i = 0; true; i = ++i, i %= CheckpointCount)
        {
            yield return new WaitUntil(() => enabled);

            IsWaiting = true;
            IsMoving = false;

            CurrentMoveCheckpoint = GetMoveCheckpointAtIndex(i);
            NextMoveCheckpoint = GetMoveCheckpointAfterIndex(i);

            yield return new WaitForSeconds(CurrentMoveCheckpoint.WaitTime);

            IsWaiting = false;
            IsMoving = true;

            float lerpValue = 0f;
            while (lerpValue < 1f)
            {
                yield return new WaitUntil(() => enabled);

                switch (CurrentMoveCheckpoint.MoveMode)
                {
                    case MoveCheckpoint.MovementMode.Seconds:
                        lerpValue += Time.deltaTime / CurrentMoveCheckpoint.MoveValue;
                        break;

                    case MoveCheckpoint.MovementMode.Speed:
                        lerpValue += Time.deltaTime * CurrentMoveCheckpoint.MoveValue / Vector2.Distance(CurrentMoveCheckpoint.Pos, NextMoveCheckpoint.Pos);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                transform.position = Vector3.Lerp(StartPosition + CurrentMoveCheckpoint.Pos, StartPosition + NextMoveCheckpoint.Pos, lerpValue);

                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void OnValidate()
    {
        ValidateFirstCheckpoint();
        ValidateCheckpoints();
        ValidateGizmoSize();
    }

    private void Reset()
    {
        _firstCheckpoint = new MoveCheckpoint();
        _otherCheckpoints = Array.Empty<MoveCheckpoint>();
        g_useCollider2DSize = GetComponent<Collider2D>() != null;
        g_size = Vector2.one;
        g_color = new Color(0f, 1f, 1f, 0.5f);

        OnValidate();
    }

    private void OnDrawGizmos()
    {
        if (g_disable) return;

        if (!Application.isPlaying)
        {
            StartPosition = transform.position;
        }
        
        Gizmos.color = g_color;
        for (int i = 0; i < CheckpointCount; ++i)
        {
            MoveCheckpoint fromCheckpoint = GetMoveCheckpointAtIndex(i);
            MoveCheckpoint toCheckpoint = GetMoveCheckpointAfterIndex(i);

            Gizmos.DrawWireCube(StartPosition + fromCheckpoint.Pos, g_size);
            Gizmos.DrawLine(StartPosition + fromCheckpoint.Pos, StartPosition + toCheckpoint.Pos);
        }
    }

    private MoveCheckpoint ValidateCheckpoint(MoveCheckpoint checkpoint)
    {
        if (checkpoint.MoveValue <= 0f)
        {
            checkpoint = new MoveCheckpoint(checkpoint.Pos, checkpoint.MoveMode, 0.0001f, checkpoint.WaitTime);
        }
        if (checkpoint.WaitTime < 0f)
        {
            checkpoint = new MoveCheckpoint(checkpoint.Pos, checkpoint.MoveMode, checkpoint.MoveValue, 0f);
        }

        return checkpoint;
    }

    private void ValidateFirstCheckpoint()
    {
        _firstCheckpoint = new MoveCheckpoint(Vector3.zero, _firstCheckpoint.MoveMode, _firstCheckpoint.MoveValue, _firstCheckpoint.WaitTime);
        _firstCheckpoint = ValidateCheckpoint(_firstCheckpoint);
    }

    private void ValidateCheckpoints()
    {
        for (int i = 0; i < _otherCheckpoints.Length; ++i)
        {
            _otherCheckpoints[i] = ValidateCheckpoint(_otherCheckpoints[i]);
        }
    }

    private void ValidateGizmoSize()
    {
        Collider2D collider2D = GetComponent<Collider2D>();

        if (g_useCollider2DSize && collider2D == null)
        {
            g_useCollider2DSize = false;
            UnityEngine.Debug.LogWarning("g_useCollider2DSize cannot be true without a Collider2D attached to gameobject. Automatically set to false.");
        }

        if (!g_useCollider2DSize) return;

        g_size = collider2D.bounds.size;
    }

    public MoveCheckpoint GetMoveCheckpointAtIndex(int index)
    {
        if (index == 0) return _firstCheckpoint;

        return _otherCheckpoints[index - 1];
    }

    public MoveCheckpoint GetMoveCheckpointAfterIndex(int index)
    {
        if (index == CheckpointCount - 1) return _firstCheckpoint;

        return _otherCheckpoints[index];
    }

    public ReadOnlyCollection<MoveCheckpoint> GetAllCheckpointsRelative()
    {
        MoveCheckpoint[] allCheckpointsRelative = new MoveCheckpoint[_otherCheckpoints.Length + 1];
        allCheckpointsRelative[0] = new MoveCheckpoint(Vector2.zero, _firstCheckpoint.MoveMode, _firstCheckpoint.MoveValue, _firstCheckpoint.WaitTime);
        for (int i = 0; i < _otherCheckpoints.Length; ++i)
        {
            allCheckpointsRelative[i + 1] = _otherCheckpoints[i];
        }

        return Array.AsReadOnly(allCheckpointsRelative);
    }
}

[Serializable]
public class MoveCheckpoint
{
    [Tooltip("Placement relative to platform's start position")]
    [SerializeField] private Vector3 _pos;
    [Tooltip("Used on way to next checkpoint. (Speed: moves x speed until checkpoint reached, Seconds: will reach next checkpoint after x seconds)")]
    [SerializeField] private MovementMode _moveMode;
    [Tooltip("Used on way to next checkpoint. X value used with move mode")]
    [SerializeField] private float _moveValue;
    [Tooltip("Seconds to wait bofore moving to next checkpoint")]
    [SerializeField] private float _waitTime;

    public Vector3 Pos => _pos;
    public MovementMode MoveMode => _moveMode;
    public float MoveValue => _moveValue;
    public float WaitTime => _waitTime;

    public MoveCheckpoint()
    {
        _pos = Vector3.zero;
        _moveMode = MovementMode.Seconds;
        _moveValue = 1f;
        _waitTime = 1f;
    }

    public MoveCheckpoint(Vector3 Pos, MovementMode moveMode, float moveValue, float waitTime)
    {
        _pos = Pos;
        _moveMode = moveMode;
        _moveValue = moveValue;
        _waitTime = waitTime;
    }

    public enum MovementMode
    {
        Seconds, Speed
    }
}
