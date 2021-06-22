using UnityEngine;
using Zenject;
namespace Modules.Player
{
    public class PlayerMovementController : MovementController
    {
        
        protected override Vector3 TargetVelocity
        {
            get => _model.TargetVelocity;
            set => _model.TargetVelocity = value;
        }
        private PlayerModel _model;
        
        [Inject]
        public void Construct(PlayerModel model)
        {
            _model = model;
        }
    }
}
