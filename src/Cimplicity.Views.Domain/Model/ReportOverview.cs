

namespace Cimplicity.Views.Domain.Model
{
    public class ReportOverview
    {
        public string WorkCell { get; set; }
        public string WorkUnit { get; set; }
        public int Actual { get; set; }
        public int Remaining { get; set; }
        public int Set { get; set; }
        public Overflow Overflow { get; set; }
        public Rule Rule { get; set; }
        
    }

    public class Rule
    {
        public RuleType Type { get; set; }
        public string Name { get; set; }
    }

    public enum RuleType
    {
        Counter,
        Timing,
        Event
    }

    public class Overflow
    {
        public int Set { get; set; }
        public int Remaining { get; set; }
    }
}