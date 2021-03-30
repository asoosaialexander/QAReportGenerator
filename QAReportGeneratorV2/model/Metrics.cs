using System.Collections.Generic;

namespace QAReportGenerator
{
    public class Metrics
    {
        public Paging paging { get; set; }
        public List<Measure> Measures { get; set; }
    }

    public class Paging
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }

    }

    public class Measure
    {
        public string Metric { get; set; }
        public List<History> History { get; set; }
    }

    public class History
    {
        public string Date { get; set; }
        public string Value { get; set; }
    }
}