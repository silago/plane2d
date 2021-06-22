#region
using Modules.Player;
using UnityEngine;
using Zenject;
#endregion
public class EyeActivator : MonoBehaviour
{
    [SerializeField]
    private EyeFollow[] eyes;
    [Inject(Id = "Player")]
    private IMovable _player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == _player.Transform)
            foreach (var eye in eyes)
                eye.Activate(other.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == _player.Transform)
            foreach (var eye in eyes)
                eye.Deactivate();
    }
}
