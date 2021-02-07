using Events;
using System.Collections.Generic;
using Yarn.Compiler;
using Yarn;

namespace Modules.YarnPlayer {
public class NewLineMessage : Message {
    public string Text { get; set;}
    public Line Line { get; set;}
}

public class ContinueMessage : Message {}
public class StartDialogueMessage : Message {
    public string NodeName { get; set;}
}

public class OptionSelectedMessage : Message {
    public int ID { get; set; }
    public string Text { get; set; }
}

public class NodeComplete : Message {}
public class CloseEvent : Message {}

public class OptionsProvidedMessage : Message {
    public Dictionary<int,OptionLine> Options { get; set; } 
}
}
