#region
using System.Collections.Generic;
using Events;
using Yarn;
#endregion
namespace Modules.YarnPlayer
{
    public class NewLineMessage : IMessage
    {
        public string Text { get; set; }
        public Line Line { get; set; }
    }

    public class ContinueMessage : IMessage
    {
    }
    public class StartDialogueMessage : IMessage
    {
        public string NodeName { get; set; }
    }

    public class OptionSelectedMessage : IMessage
    {
        public int ID { get; set; }
        public string Text { get; set; }
    }

    public class NodeComplete : IMessage
    {
    }
    public class CloseEvent : IMessage
    {
    }

    public class DialogueComplete : IMessage
    {
    }

    public class OptionsProvidedMessage : IMessage
    {
        public Dictionary<int, OptionLine> Options { get; set; }
    }
}
