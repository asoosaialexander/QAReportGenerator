using System;
using System.Collections.Generic;

namespace QAReportGenerator
{
    public class Author
    {
        
        public string Team { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public Vulnerabilities Vulnerabilities { get; set; }
    }

    public class Vulnerabilities
    {
        public List<Issue> Issues { get; set; }
    }

    public class Issue
    {
        public string Project { get; set; }
        public string Component { get; set; }
        public string Type { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public string Assignee { get; set; }
        public string Author { get; set; }
        public string CreationDate { get; set; }
        public string UpdateDate { get; set; }
    }
}