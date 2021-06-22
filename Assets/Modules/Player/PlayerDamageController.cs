using Events;
using Modules.Resources;
using UnityEngine;
using Zenject;
namespace Modules.Game.Player
{
    public class PlayerDamageController : DamageController
    {
        private UserDataProvider _userdata;
        [Inject]
        public void Construct(UserDataProvider userDataProvider)
        {
            _userdata = userDataProvider;
        }
        protected override int Hull
        {
            get => _userdata[ResourceNames.Hull];
            set => _userdata[ResourceNames.Hull] = value;
        }
        protected override void Awake()
        {
            this.Subscribe<DamageMessage, int>(OnDamage, gameObject.GetInstanceID()).BindTo(this);
            //base.Awake();
        }
        protected override void OnDestroy()
        {
        }
        
    }
}
