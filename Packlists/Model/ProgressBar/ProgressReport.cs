namespace Packlists.Model.ProgressBar
{
    public class ProgressReport
    {
        public string CurrentTask { get; set; }
        public int CurrentTaskNumber { get; set; }
        public int MaxTaskNumber { get; set; }
        public bool IsIndeterminate { get; set; }
    }
}
