using Events;
using UnityEngine;
namespace Modules.SFX
{
    public class MusicTrigger : MonoBehaviour
    {
        [SerializeField]
        private string trackName;
    
        private void OnTriggerEnter(Collider other)
        {
            this.PlayMusic(trackName);
        }
    }
}
