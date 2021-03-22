namespace QAReportGenerator
{
    public class Project
    {
        public string Team { get; set; }
        public string Repository { get; set; }
        public string Branch { get; set; }

        public Metrics Metrics { get; set; }
    }
}