using Modules.Resources;
using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField]
    private ResourceSettings resourceSettings;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<ResourceSettings>().FromInstance(resourceSettings).AsSingle().NonLazy();
        Container.Bind<UserDataProvider>().AsSingle().NonLazy();
    }
}