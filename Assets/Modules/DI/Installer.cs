#region
using UnityEngine;
using Zenject;
#endregion
public class Installer : MonoInstaller
{
    [SerializeField] private MovementController player;
    public override void InstallBindings()
    {
        Container.Bind<IMovable>().WithId("Player").FromInstance(player);
    }
}