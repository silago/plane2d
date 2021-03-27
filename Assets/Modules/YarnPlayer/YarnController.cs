using System;
using System.Collections.Generic;
using Yarn;
using Events;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

namespace Modules.YarnPlayer {
public class YarnController : MonoBehaviour {

    [SerializeField]
    string lang;
    [SerializeField]
    List<YarnProgram> programs;
    private Dialogue _dialogue;
	bool isWaitingForAnswer = false;
	IDictionary<string, string> stringTable = new Dictionary<string, string>();
	bool isReady = false;
	private Program _mainProgram;


	void Awake() {
		LoadCompileAndLoad();
	}

	void LoadCompileAndLoad() 
	{
        foreach(var p in programs) {
            AddStringTable(p);
        }
        _mainProgram =Program.Combine(programs.Select(x => x.GetProgram()).ToArray());
		_dialogue = InitDialogue(_mainProgram);
		InitFunctions(_dialogue);

		this.Subscribe<StartDialogueMessage>(OnStartDialogueMessage);
		this.Subscribe<OptionSelectedMessage>(OnOptionSelectMessage);
		this.Subscribe<ContinueMessage>(OnContinueMessage);
	}

	private void OnContinueMessage(ContinueMessage obj)
	{
		_dialogue.Continue();
	}

	private void OnOptionSelectMessage(OptionSelectedMessage obj)
	{
		OnOptionSelected(obj.ID);
	}
	private void OnStartDialogueMessage(StartDialogueMessage obj)
	{
		Run(obj.NodeName);
	}
	void OnDisable() 
	{
		this.Unsubscribe<StartDialogueMessage>(OnStartDialogueMessage);
	}

	Dialogue InitDialogue(Program program)
	{
		var variableStorage = new MemoryVariableStore();//DictStorage();
		
		var dialogue = new Dialogue(variableStorage);

		dialogue.DialogueCompleteHandler = DialogueCompleteHandle;
		dialogue.NodeCompleteHandler = NodeCompleteHandler;
		dialogue.NodeStartHandler = NodeStartHandler;
		dialogue.CommandHandler = CommandHandler; 
		dialogue.OptionsHandler = OptionsHandler;
		dialogue.LineHandler = LineHandler;
		dialogue.LogDebugMessage = (msg) => Debug.Log(msg); //GD.Print(msg);
		dialogue.LogErrorMessage = (msg) => Debug.LogError(msg); //GD.PrintErr(msg);
        dialogue.SetProgram(program);
		return dialogue;
	}

	void InitFunctions(Dialogue dialogue)
	{
		AddFunction("visited", (string s) => false); 
	}

	private bool Visited(string name)
	{
		return false;
	}

	public void Run(string node)
	{
		_dialogue.SetNode(node);
		_dialogue.Continue();
	}


	private void NodeStartHandler(string startedNodeName)
	{
		//return Dialogue.HandlerExecutionType.ContinueExecution;
	}

	private void DialogueCompleteHandle() {
        this.SendEvent(new DialogueComplete());
    }//GD.Print("dialogue complete");



    // todo:: rewrite
    /*
	public override void _Input(InputEvent ev) {
		if (isWaitingForAnswer == false) {
			return;
		}
		if ( ev is InputEventKey ke && ke.Pressed) {
			var textInput = ke.AsText();
			int num;
			if (int.TryParse(textInput, out num)) {
				isWaitingForAnswer = false;
				OnOptionSelected(num);
			}
		}
	}
    */

    
	private void AddFunction(string name, Delegate implementation)
		=> _dialogue.Library.RegisterFunction(name, implementation);

	private void AddFunction<TResult, T1>(string name, Func<TResult, T1> implementation)
		=> AddFunction(name, (Delegate) implementation); 

	private void NodeCompleteHandler(string completedNodeName)
	{
        this.SendEvent(new NodeComplete());
		//throw new NotImplementedException();
		//return Dialogue.HandlerExecutionType.ContinueExecution;

	}
	void CommandHandler(Command command)
	{
		//throw new NotImplementedException();
	}

	void OptionsHandler(OptionSet options) {

		isWaitingForAnswer = true;
		Dictionary<int,OptionLine> optionsStrings = new Dictionary<int, OptionLine>();
		foreach(OptionSet.Option opt in options.Options) {
			stringTable.TryGetValue(opt.Line.ID, out var info);
			optionsStrings[opt.ID]=new OptionLine(){Line = opt.Line, Text = info};
		}

		this.SendEvent( new OptionsProvidedMessage { Options=optionsStrings } );
	}

	void OnOptionSelected(int optionId) {
		_dialogue.SetSelectedOption(optionId);
		_dialogue.Continue();
	}
	
	void LineHandler(Line line) {
		stringTable.TryGetValue(line.ID, out var info);
		this.SendEvent(new NewLineMessage { Text = info, Line = line });
		//return Dialogue.HandlerExecutionType.PauseExecution;
	}

    public void AddStringTable(YarnProgram yarnScript)
        {
            Debug.Log(" add string table");
            //var textToLoad = new TextAsset();
            var text = yarnScript.defaultStringTable.text;
            var data = StringTableEntry.ParseFromCSV(text);
            stringTable = data.ToDictionary(x => x.ID, x => x.Text);
            /*
            
            if (yarnScript.localizations != null || yarnScript.localizations.Length > 0) {
                textToLoad = Array.Find(yarnScript.localizations, element => element.languageName == lang)?.text;
            }

            if (textToLoad == null || string.IsNullOrEmpty(textToLoad.text)) {
                textToLoad = yarnScript.baseLocalisationStringTable;
            }

            // Use the invariant culture when parsing the CSV
            var configuration = new CsvHelper.Configuration.Configuration(
                System.Globalization.CultureInfo.InvariantCulture
            );

            using (var reader = new System.IO.StringReader(textToLoad.text))
            using (var csv = new CsvReader(reader, configuration)) {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    stringTable.Add(csv.GetField("id"), csv.GetField("text"));
                }
            }
            
            */
        }

}
}
