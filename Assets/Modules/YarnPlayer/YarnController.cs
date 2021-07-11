using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using Events;
using Modules.Resources;
using UnityEngine;
using Yarn;
using Yarn.Markup;
using Yarn.Unity;
using Zenject;

namespace Modules.YarnPlayer
{
    public class YarnController : MonoBehaviour
    {
        public YarnProject yarnProject;
        [SerializeField]
        private List<Program> programs;
        private Dialogue _dialogue;
        private Program _mainProgram;
        private IDictionary<string, string> _stringTable = new Dictionary<string, string>();
        private Dictionary<string, Delegate> commandHandlers = new Dictionary<string, Delegate>();
        private DataProvider _dataProvider;
        private IVariableStorage _variableStorage;

        [Inject]
        void Construct(DataProvider users, IVariableStorage variableStorage)
        {
            _variableStorage = variableStorage;
            _dataProvider = users;
        }
        
        private void Awake()
        {
            LoadPrograms();
            
            this.Subscribe<StartDialogueMessage>(OnStartDialogueMessage);
            this.Subscribe<OptionSelectedMessage>(OnOptionSelectMessage);
            this.Subscribe<ContinueMessage>(OnContinueMessage);
        }

        private void LoadPrograms()
        {
            //foreach (var p in programs) AddStringTable(p);
            _mainProgram = yarnProject.GetProgram(); 
            _dialogue = InitDialogue(_mainProgram);
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

        private Dialogue InitDialogue(Program program)
        {
            //IVariableStorage variableStorage = new MemoryVariableStore(); 
            var dialogue = new Dialogue(_variableStorage) {
                DialogueCompleteHandler = DialogueCompleteHandle,
                NodeCompleteHandler = NodeCompleteHandler,
                NodeStartHandler = NodeStartHandler,
                CommandHandler = CommandHandler,
                OptionsHandler = OptionsHandler,
                LineHandler = LineHandler,
                LogDebugMessage = Debug.Log,
                LogErrorMessage = Debug.LogError
            };

            dialogue.SetProgram(program);
            return dialogue;
        }

        private void InitFunctions(Dialogue dialogue)
        {
            AddFunction("visited", (string s) => false);
        }

        private bool Visited(string name)
        {
            return false;
        }

        public void Run(string node)
        {
            _dialogue.Stop();
            _dialogue.SetNode(node);
            _dialogue.Continue();
        }


        private void NodeStartHandler(string startedNodeName)
        {
        }

        private void DialogueCompleteHandle()
        {
            this.SendEvent(new DialogueComplete());
        }
        
        public void AddCommand(string name, Delegate implementation)
        {
           commandHandlers.Add(name, implementation); 
        }
        
        public void AddFunction(string name, Delegate implementation)
        {
            _dialogue.Library.RegisterFunction(name, implementation);
        }

        public void AddFunction<TResult, T1>(string name, Func<TResult, T1> implementation)
        {
            AddFunction(name, (Delegate)implementation);
        }

        private void NodeCompleteHandler(string completedNodeName)
        {
            this.SendEvent(new NodeComplete());

        }
        
        private static string[] ParseCommandParameters(string command)
        {
            return command.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
        private void CommandHandler(Command command)
        {
            DispatchCommandToRegisteredHandlers(command.Text);
        }
        
        internal bool DispatchCommandToRegisteredHandlers(String command)
        {
            string[] commandTokens = ParseCommandParameters(command);
            if (commandTokens.Length == 0) return false;
            var firstWord = commandTokens[0];
            if (commandHandlers.ContainsKey(firstWord) == false) return false;
            var remainingWords = new string[commandTokens.Length - 1];
            var @delegate = commandHandlers[firstWord];
            var methodInfo = @delegate.Method;
            Array.Copy(commandTokens, 1, remainingWords, 0, remainingWords.Length);
            var rawParameters = new List<string>(remainingWords);
            object[] finalParameters;
            try
            {
                finalParameters = GetPreparedParametersForMethod(rawParameters.ToArray(), methodInfo);
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"Can't run command {firstWord}: {e.Message}");
                return false;
            }


            if (typeof(YieldInstruction).IsAssignableFrom(methodInfo.ReturnType))
            {
                StartCoroutine(WaitForYieldInstruction(@delegate, finalParameters));
            }
            else if (typeof(void) == methodInfo.ReturnType)
            {
                // This method does not return anything. Invoke it and
                // continue immediately.
                @delegate.DynamicInvoke(finalParameters);
                _dialogue.Continue();
                //ContinueDialogue();
            }
            else
            {
                Debug.LogError($"Cannot run command {firstWord}: the provided delegate does not return a valid type (permitted return types are YieldInstruction or void)");
                return false;
            }

            return true;

            IEnumerator WaitForYieldInstruction(Delegate @theDelegate, object[] finalParametersToUse)
            {
                var yieldInstruction = @theDelegate.DynamicInvoke(finalParametersToUse);
                yield return yieldInstruction;
                _dialogue.Continue();
            }
        }


        ResourceCondition[] GetResourceConditions(Tag tag)
        {
            
            return tag.Attributes.Select( x=>
                new ResourceCondition(x.Name,x.Type.FromText(), int.Parse(x.Value))
            ).ToArray();
        }
        
        bool CheckLineRequirement(Tag tag)
        {
            return _dataProvider.Check(GetResourceConditions(tag));
        }



        ResourceCondition ParseResourceConditional(string name, string value)
        {
            var type = name;
            //ResourceType type;
            //if (!ResourceType.TryParse(name, out type))
            //{
            //    Debug.LogError($"could not parse {name} attribute");
            //    return default;
            //}
            switch (value[0])
            {
                case '>':
                    return new ResourceCondition(type, Operator.Gt, int.Parse(value.Substring(1, value.Length - 1)));
                case '<':
                    return new ResourceCondition(type, Operator.Lt, int.Parse(value.Substring(1, value.Length - 1)));
                case '=':
                    return new ResourceCondition(type, Operator.Eq, int.Parse(value.Substring(1, value.Length - 1)));
                default:
                    return new ResourceCondition(type, Operator.Eq, int.Parse(value));
            }
        }


        protected class ResourceAttribute
        {
            public string Type;
            public int Value;
            public Func<int, int, bool> Comparer;
            
            public ResourceAttribute(string name, string value)
            {
                switch (value[0])
                {
                    case '>':
                        Comparer = (a, b) => a > b;
                        Value = int.Parse(value.Substring(1, value.Length-1));
                        break;
                    case '<': 
                        Comparer = (a, b) => a < b;
                        Value = int.Parse(value.Substring(1, value.Length-1));
                        break;
                    case '=': 
                        Comparer = (a, b) => a == b;
                        Value = int.Parse(value.Substring(1, value.Length-1));
                        break;
                    default:
                        Comparer = (a, b) => a == b;
                        Value = int.Parse(value);
                        break;
                }
            }
        }

        private void OptionsHandler(OptionSet options)
        {
            var optionsStrings = new Dictionary<int, OptionLine>();
                //DialogueOption[] optionSet = new DialogueOption[options.Options.Length];
                //for (int i = 0; i < options.Options.Length; i++) {

                //    var localisedLine = lineProvider.GetLocalizedLine(options.Options[i].Line);
                //    var text = Dialogue.ExpandSubstitutions(localisedLine.RawText, options.Options[i].Line.Substitutions);
                //    localisedLine.Text = Dialogue.ParseMarkup(text);

                foreach (var opt in options.Options)
                {
                    var lineData = GetLocalizedLine(opt.Line);
                    var lineText = _dialogue.ParseMarkup(lineData.RawText).Text;
                optionsStrings[opt.ID] = new OptionLine {
                    IsAvailable = opt.IsAvailable,
                    Line = opt.Line,
  //                Text = lineData.Text,
                    Text = Dialogue.ExpandSubstitutions(lineText, lineData.Substitutions)
                };

                var lineRequirements = new LineRequirement[] {
                };// lineData.GetLineRequirements().Select(x => _userDataProvider.ResolveLineRequirement(x));
                var conditions = GetRequirements(lineData);
                lineRequirements = _dataProvider.ResolveLineRequirement(conditions);
                
                optionsStrings[opt.ID].LineRequirements = lineRequirements.ToArray();
                optionsStrings[opt.ID].IsSatisfied = lineRequirements.All(x => x.IsSatisfied);
            }

            this.SendEvent(new OptionsProvidedMessage {
                Options = optionsStrings
            });
        }


        ResourceCondition[] GetRequirements(LocalizedLine line)
        {

            var result = new List<ResourceCondition>();
            MarkupParseResult attributes = _dialogue.ParseMarkup(line.RawText);
            //if (attribures. == null) return result.ToArray();
            foreach (MarkupAttribute attr in attributes.Attributes.Where(x=>x.Name == Tags.R))
            {
                var condition = attr.Properties["Condition"];
                var text = condition.StringValue;
                
                string[] splitResult = null;
                foreach (var op in new[] { '=', '>', '<' })
                    if ((splitResult = text.Split(op)).Length == 2)
                        result.Add( new ResourceCondition(splitResult[0], op.ToString().FromText(), int.Parse(splitResult[1])
                        ));

            }
            return result.ToArray();
        }

        private void OnOptionSelected(int optionId)
        {
            _dialogue.SetSelectedOption(optionId);
            _dialogue.Continue();
        }

        private LocalizedLine GetLocalizedLine(Line line)
        {
            string textLanguageCode = System.Globalization.CultureInfo.CurrentCulture.Name;
            var text = yarnProject.GetLocalization(textLanguageCode).GetLocalizedString(line.ID);
            var x = new LocalizedLine();
            var f = x.Text;
            return new LocalizedLine()
            {
                TextID = line.ID,
                RawText = text,
                Substitutions = line.Substitutions,
            };
            


        }

        private void LineHandler(Line line)
        {
            //_stringTable.TryGetValue(line.ID, out var rawText);
            var localizedLine = GetLocalizedLine(line); 

            var text = Dialogue.ExpandSubstitutions(localizedLine.RawText, line.Substitutions);
            var lineData = LineParser.ParseLine(text);
            this.SendEvent(new NewLineMessage {
                Text = text,
                Line = line
            });
            _dialogue.Continue();
        }

        public void AddStringTable(Program yarnScript)
        {
            //var data = StringTableEntry.ParseFromCSV(yarnScript.);
            //stringTable = data.ToDictionary(x => x.ID, x => x.Text);
        }
        
        private object[] GetPreparedParametersForMethod(string[] parameters, MethodInfo methodInfo)
        {
            ParameterInfo[] methodParameters = methodInfo.GetParameters();
            var requiredParameters = 0;
            var optionalParameters = 0;
            foreach (var parameter in methodParameters)
                if (parameter.IsOptional)
                    optionalParameters += 1;
                else
                    requiredParameters += 1;

            bool anyOptional = optionalParameters > 0;
            if (anyOptional)
                if (parameters.Length < requiredParameters || parameters.Length > (requiredParameters + optionalParameters))
                    throw new ArgumentException($"{methodInfo.Name} requires between {requiredParameters} and {requiredParameters + optionalParameters} parameters, but {parameters.Length} {(parameters.Length == 1 ? "was" : "were")} provided."); 
                else if (parameters.Length != requiredParameters)
                    throw new ArgumentException($"{methodInfo.Name} requires {requiredParameters} parameters, but {parameters.Length} {(parameters.Length == 1 ? "was" : "were")} provided.");          

            var finalParameters = new object[requiredParameters + optionalParameters];

            for (int i = 0; i < finalParameters.Length; i++)
            {
                if (i >= parameters.Length) {
                    finalParameters[i] = System.Type.Missing;
                    continue;
                }

                var expectedType = methodParameters[i].ParameterType;
                try {
                    finalParameters[i] = Convert.ChangeType(parameters[i], expectedType, System.Globalization.CultureInfo.InvariantCulture);
                } catch (Exception e) {
                    var paramName  = methodParameters[i].Name;
                    throw new ArgumentException($"can't convert parameter {i+1} (\"{parameters[i]}\") to parameter {paramName} ({expectedType}): {e}"); 
                }
            }
            return finalParameters;
        }
    }

    
        public class LineRequirement
        {
            public LineRequirement(ResourceCondition condition, ResourceInfo info, int actualValue, bool isSatisfied)
            {
                Condition = condition;
                this.ResourceInfo = info;
                ActualValue = actualValue;
                IsSatisfied = isSatisfied;
            }
            public readonly ResourceCondition Condition;
            public readonly ResourceInfo ResourceInfo;
            public readonly int  ActualValue;
            public readonly bool IsSatisfied;
        }
}
