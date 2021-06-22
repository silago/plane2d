using Modules.Inventory;
using Modules.Player;
using Modules.Resources;
using UnityEngine;
using Yarn;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField]
    private ResourceSettings resourceSettings;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<ResourceSettings>().FromInstance(resourceSettings).AsSingle().NonLazy();
        Container.Bind<UserDataProvider>().AsSingle().NonLazy();
        Container.Bind<InventoryDataProvider>().AsSingle().NonLazy();
        Container.Bind<IVariableStorage>().To<DialogueDataStorage>().AsSingle().NonLazy();
        Container.Bind<Updater>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<PlayerModel>().AsSingle().NonLazy();
    }
}