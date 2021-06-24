    using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.YarnPlayer
{
    public class DialoguePlayer : MonoBehaviour
    {
        [SerializeField] private Text caption;
        [SerializeField] private GameObject mainContainer;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private ResourceChangeInfo _resourceChangeInfoPrefab;
        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private YarnDialogueOptionNode optionPrefab;

        [SerializeField] private Transform place;

        [SerializeField] private DialogueLineHandler linePrefab;
        private readonly List<YarnDialogueOptionNode> _buttons = new List<YarnDialogueOptionNode>();
        private void Awake()
        {
            mainContainer.SetActive(false);
            this.Subscribe<OptionsProvidedMessage>(OnOptionsProvided);
            this.Subscribe<OptionSelectedMessage>(OnOptionSelected);
            this.Subscribe<NewLineMessage>(OnNewLine);
            this.Subscribe<DialogueComplete>(OnDialogueComplete);
            this.Subscribe<StartDialogueMessage>(OnStartDialogueMessage);
            this.Subscribe<DialogueDataStorage.DialogueResourceChanged<int>>(OnDialogueResourceChanged);

            continueButton.onClick.AddListener(() =>
            {
                this.SendEvent<ContinueMessage>(null);
            });

            closeButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                mainContainer.SetActive(false);
            });

        }
        private void OnDialogueResourceChanged(DialogueDataStorage.DialogueResourceChanged<int> obj)
        {
           Instantiate(_resourceChangeInfoPrefab, place).Init(obj); 
        }
        private void OnStartDialogueMessage(StartDialogueMessage obj)
        {
            this.caption.text = obj.Caption;
            Time.timeScale = 0f;
            mainContainer.SetActive(true);
        }

        private void OnDialogueComplete(DialogueComplete _)
        {
            continueButton.gameObject.SetActive(false);
        }

        private void OnNewLine(NewLineMessage obj)
        {
            var item = Instantiate(linePrefab, place);
            item.SetLine(obj.Line);
            var t = obj.Text.Replace("\\n", System.Environment.NewLine);
            item.SetText(t);
            continueButton.gameObject.SetActive(true);
            continueButton.transform.SetAsLastSibling();
            OnNextFrame(() =>
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            });
        }

        private void CopyNode(YarnDialogueOptionNode origin)
        {
            var copy = Instantiate(origin, place);
            //copy.OptionId = origin.OptionId;
            //copy.optionLine = origin.optionLine;
            copy.ClearRequirements();
            copy.DisableButton();
        }

        private void OnOptionSelected(OptionSelectedMessage msg)
        {
            var selectedBtn= _buttons.First(x => x.OptionId == msg.ID);
            CopyNode(selectedBtn);
            _buttons.ForEach(x=>x.Hide());

            OnNextFrame(() =>
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            });
        }

        private void OnNextFrame(Action action)
        {
            StartCoroutine(OnNextFrameCo(action));
        }

        private IEnumerator OnNextFrameCo(Action action)
        {
            yield return new WaitForEndOfFrame(); 
            action();
        }

        private void OnOptionsProvided(OptionsProvidedMessage obj)
        {
            continueButton.gameObject.SetActive(false);
            while (_buttons.Count < obj.Options.Count)
            {
                var btn = Instantiate(optionPrefab, place);
                _buttons.Add(btn);
            }

            _buttons.ForEach(x => {x.gameObject.SetActive(false);});
            foreach (var kv in obj.Options)
            {
                YarnDialogueOptionNode btn = _buttons[kv.Key];
                if (kv.Value.IsAvailable == false)
                {
                    continue;
                }
                btn.OptionId = kv.Key;
                btn.optionLine = kv.Value;
                btn.gameObject.SetActive(true);
                btn.transform.SetAsLastSibling();
                btn.gameObject.SetActive(true);
            }

            OnNextFrame(() =>
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            });
        }
    }
}
