using Events;
using Modules.Resources;
using UnityEngine;
using Zenject;
namespace Modules.Game.Player
{
    public class PlayerDamageController : DamageController
    {
        private DataProvider _userdata;
        [Inject]
        public void Construct(DataProvider dataProvider)
        {
            _userdata = dataProvider;
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
