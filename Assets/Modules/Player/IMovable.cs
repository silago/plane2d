using UnityEngine;
namespace Modules.Player
{
    public interface IMovable
    {
        Vector3 Direction { get; }
        float VelocityMagnitude { get; }
        Transform Transform { get; }
    }
}
