#region
using System.Linq;
using UnityEngine;
#endregion
public class AnimationOffset : MonoBehaviour
{
    private static readonly int Offset = Animator.StringToHash("Offset");
    private static readonly int RandomInt = Animator.StringToHash("RandomInt");
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private bool random;
    [Range(0, 1)]
    [SerializeField]
    private float offset;
    [Range(0, 2)]
    [SerializeField]
    private float speed;
    private StateChanged[] _stateChangedEvents;

    private void OnEnable()
    {
        _stateChangedEvents = _animator.GetBehaviours<StateChanged>().ToArray();
        foreach (var changedEvent in _stateChangedEvents)
            changedEvent.StateExit += OnStateExit;
        if (random)
            offset = Random.Range(0f, 1f);
        _animator.speed = speed;
        _animator.SetFloat(Offset, offset);
    }

    private void OnDisable()
    {
    }
    private void OnStateExit()
    {
        SetRandom();
    }
    private void SetRandom()
    {
        var r = Random.Range(0, 2);
        _animator.SetInteger(RandomInt, r);
    }
}
