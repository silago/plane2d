using System;
using Events;
using UnityEngine;
using UnityEngine.UI;
using Yarn;


namespace Modules.YarnPlayer {
    public class DialogueLineHandler : MonoBehaviour {
        [SerializeField]
        Text text;
        [SerializeField]
        Image image;
        
        
        public void SetLine(Line line)
        {
        }
        public void SetText(string text) {
            this.text.text = text;
        }
    }
}
