#region
using Yarn;
#endregion
namespace Modules.YarnPlayer
{
    public class OptionLine
    {
        public bool IsAvailable = false;
        public string Text { get; set; }
        public Line Line { get; set; }
        public bool IsSatisfied = true;
        public LineRequirement[] LineRequirements;
    }
}
