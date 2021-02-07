using System;
using System.Collections.Generic;
using System.Collections;
using Events;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// process options
namespace Modules.YarnPlayer {
public class DialoguePlayer : MonoBehaviour
{
	private readonly List<YarnDialogueOptionNode> _buttons = new List<YarnDialogueOptionNode>();
	[SerializeField] private Button continueButton;
	[SerializeField] private Button    closeButton;
	[SerializeField] private ScrollRect scrollRect;

    [SerializeField] private YarnDialogueOptionNode optionPrefab;

    [SerializeField] private Transform place;

    [SerializeField] private DialogueLineHandler linePrefab;
	// Called when the node enters the scene tree for the first time.
	void Awake()
	{
		this.Subscribe<OptionsProvidedMessage>(OnOptionsProvided);
		this.Subscribe<OptionSelectedMessage>(OnOptionSelected);
		this.Subscribe<NewLineMessage>(OnNewLine);
		this.Subscribe<NodeComplete>(OnNodeComplete);
		this.Subscribe<StartDialogueMessage>(OnStartDialogueMessage);
		
		continueButton.onClick.AddListener(() =>
		{
			this.SendEvent<ContinueMessage>();	
		});
		
		closeButton.onClick.AddListener(() =>
		{
			this.gameObject.SetActive(false);
		});
		
	}

	private void OnStartDialogueMessage(StartDialogueMessage obj)
	{
		this.gameObject.SetActive(true);
		closeButton.gameObject.SetActive(false);
	}

	private void OnNodeComplete(NodeComplete obj)
	{
		continueButton.gameObject.SetActive(false);
		closeButton.gameObject.SetActive(true);
	}

	private void OnNewLine(NewLineMessage obj)
    {
        DialogueLineHandler item = Instantiate(linePrefab, place);
        item.SetLine(obj.Line);
        item.SetText(obj.Text);
		continueButton.gameObject.SetActive(true);
        OnNextFrame(() =>
        {
	        scrollRect.verticalNormalizedPosition = 0f;
	        Canvas.ForceUpdateCanvases();
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
	        scrollRect.verticalNormalizedPosition = 0f;
	        Canvas.ForceUpdateCanvases();
        });
    }

    private void OnNextFrame(Action a)
    {
	    StartCoroutine(OnNextFrameCo(a));
    }

    private IEnumerator OnNextFrameCo(Action a)
    {
	    yield return new WaitForSeconds(1);
	    a();
    } 

	private void OnOptionsProvided(OptionsProvidedMessage obj)
	{
		continueButton.gameObject.SetActive(false);
		while (_buttons.Count < obj.Options.Count) {
			var btn = Instantiate(optionPrefab, place);
            _buttons.Add(btn);
		}

		var itOptions = obj.Options.GetEnumerator();
		var itButtons = _buttons.GetEnumerator();
		var i = 0;
		using (itOptions)
		using (itButtons)
			while(itButtons.MoveNext() && itOptions.MoveNext()) {
			   //itButtons.Current.OptionId = itOptions.Current.Key;
			   itButtons.Current.optionLine = itOptions.Current.Value;
			   itButtons.Current.OptionId = i++; 
			   itButtons.Current.gameObject.SetActive(true);
			   itButtons.Current.gameObject.transform.SetAsLastSibling();
			}
	}


	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
}
