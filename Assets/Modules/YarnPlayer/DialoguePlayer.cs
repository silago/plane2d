#region
using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;
#endregion

// process options
namespace Modules.YarnPlayer
{
    public class DialoguePlayer : MonoBehaviour
    {
        [SerializeField] private GameObject mainContainer;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private YarnDialogueOptionNode optionPrefab;

        [SerializeField] private Transform place;

        [SerializeField] private DialogueLineHandler linePrefab;
        private readonly List<YarnDialogueOptionNode> _buttons = new List<YarnDialogueOptionNode>();
        // Called when the node enters the scene tree for the first time.
        private void Awake()
        {
            mainContainer.SetActive(false);
            this.Subscribe<OptionsProvidedMessage>(OnOptionsProvided);
            this.Subscribe<OptionSelectedMessage>(OnOptionSelected);
            this.Subscribe<NewLineMessage>(OnNewLine);
            this.Subscribe<DialogueComplete>(OnDialogueComplete);
            this.Subscribe<StartDialogueMessage>(OnStartDialogueMessage);

            continueButton.onClick.AddListener(() =>
            {
                this.SendEvent<ContinueMessage>();
            });

            closeButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                mainContainer.SetActive(false);
            });

        }

        private void OnStartDialogueMessage(StartDialogueMessage obj)
        {
            Time.timeScale = 0f;
            mainContainer.SetActive(true);
            closeButton.gameObject.SetActive(false);
        }

        private void OnDialogueComplete(DialogueComplete _)
        {
            continueButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);
        }

        private void OnNewLine(NewLineMessage obj)
        {
            var item = Instantiate(linePrefab, place);
            item.SetLine(obj.Line);
            item.SetText(obj.Text);
            continueButton.gameObject.SetActive(true);
            continueButton.transform.SetAsLastSibling();
            OnNextFrame(() =>
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            });
        }

        private void OnOptionSelected(OptionSelectedMessage msg)
        {
            //DialogueLineHandler item = Instantiate(linePrefab, place);
            //item.SetText(msg.Text);
            for (var index = 0; index < _buttons.Count; index++)
            {
                var btn = _buttons[index];
                if (msg.ID == btn.OptionId)
                {
                    var copy = Instantiate(btn, place);
                    copy.OptionId = btn.OptionId;
                    copy.optionLine = btn.optionLine;
                    _buttons[index].DisableButton();
                    _buttons[index] = copy;
                    copy.Hide();
                }
                else
                {
                    btn.Hide();
                }
            }

            OnNextFrame(() =>
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            });
        }

        private void OnNextFrame(Action a)
        {
            StartCoroutine(OnNextFrameCo(a));
        }

        private IEnumerator OnNextFrameCo(Action a)
        {
            yield return new WaitForEndOfFrame(); 
            a();
        }

        private void OnOptionsProvided(OptionsProvidedMessage obj)
        {
            continueButton.gameObject.SetActive(false);
            while (_buttons.Count < obj.Options.Count)
            {
                var btn = Instantiate(optionPrefab, place);
                _buttons.Add(btn);
            }

            var itOptions = obj.Options.GetEnumerator();
            var itButtons = _buttons.GetEnumerator();
            var i = 0;
            using (itOptions)
            using (itButtons)
            {
                while (itButtons.MoveNext() && itOptions.MoveNext())
                {
                    //itButtons.Current.OptionId = itOptions.Current.Key;
                    itButtons.Current.optionLine = itOptions.Current.Value;
                    itButtons.Current.OptionId = i++;
                    itButtons.Current.gameObject.SetActive(true);
                    itButtons.Current.gameObject.transform.SetAsLastSibling();
                }
            }
            
            OnNextFrame(() =>
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            });
        }


        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
