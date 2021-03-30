using System;
using System.Collections.Generic;
using System.Data;
using ClosedXML.Excel;

namespace QAReportGenerator
{
    public static class ExcelClient
    {
        public static string GenerateReport(List<Project> data, List<Author> issueData)
        {
            DataTable dt = new DataTable();
            dt.TableName = "By repository";
            dt.Columns.Add("Repository");
            dt.Columns.Add("Branch");
            dt.Columns.Add("Scanned date");
            dt.Columns.Add("Code coverage", typeof(float));
            dt.Columns.Add("Vulnerabilities", typeof(Int32));
            dt.Columns.Add("Code smells", typeof(Int32));

            foreach (var project in data)
            {
                DataRow dr = dt.NewRow();
                var measures = project.Metrics.Measures;

                dr["Repository"] = project.Repository;
                dr["Branch"] = project.Branch;
                dr["Scanned date"] = measures.Find(m => m.Metric == "coverage").History[0].Date.Substring(0,10);
                dr["Code coverage"] = measures.Find(m => m.Metric == "coverage").History[0].Value;
                dr["Vulnerabilities"] = measures.Find(m => m.Metric == "vulnerabilities").History[0].Value;
                dr["Code smells"] = measures.Find(m => m.Metric == "code_smells").History[0].Value;

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();

            DataTable dt2 = new DataTable();
            dt2.TableName = "By Author";
            dt2.Columns.Add("Team");
            dt2.Columns.Add("Name");
            dt2.Columns.Add("Email address");
            dt2.Columns.Add("Vulnerabilities");
            dt2.Columns.Add("Code Smells");
            dt2.Columns.Add("Bug");

            foreach (var author in issueData)
            {
                DataRow dr = dt2.NewRow();
                var issues = author.Vulnerabilities.Issues;

                dr["Team"] = author.Team;
                dr["Name"] = author.Name;
                dr["Email address"] = author.EmailAddress;
                dr["Vulnerabilities"] = issues.FindAll(x => x.Type.ToLower() == "vulnerabilities").Count;
                dr["Code Smells"] = issues.FindAll(x => x.Type.ToLower() == "code_smell").Count;
                dr["Bug"] = issues.FindAll(x => x.Type.ToLower() == "bug").Count;

                dt2.Rows.Add(dr);
            }

            DataTable dt3 = new DataTable();
            dt3.TableName = "Detailed Information";
            dt3.Columns.Add("Repository");
            dt3.Columns.Add("Component");
            dt3.Columns.Add("Type");
            dt3.Columns.Add("Severity");
            dt3.Columns.Add("Message");
            dt3.Columns.Add("Author");
            dt3.Columns.Add("Assignee");
            dt3.Columns.Add("Creation date");
            dt3.Columns.Add("Update date");

            foreach (var author in issueData)
            {
                foreach (var issue in author.Vulnerabilities.Issues)
                {
                    DataRow dr = dt3.NewRow();

                    dr["Repository"] = issue.Project;
                    dr["Component"] = issue.Component;
                    dr["Type"] = issue.Type;
                    dr["Severity"] = issue.Severity;
                    dr["Message"] = issue.Message;
                    dr["Author"] = issue.Author;
                    dr["Assignee"] = issue.Assignee;
                    dr["Creation date"] = issue.CreationDate.Substring(0,10);
                    dr["Update date"] = issue.UpdateDate.Substring(0,10);

                    dt3.Rows.Add(dr);
                }
            }

            string fileName = "Issues_Report_" + DateTime.Now.ToShortDateString() + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, dt.TableName);
                wb.Worksheet(dt.TableName).Row(1).Style.Font.Bold = true;

                wb.Worksheets.Add(dt2, dt2.TableName);
                wb.Worksheet(dt2.TableName).Row(1).Style.Font.Bold = true;

                wb.Worksheets.Add(dt3, dt3.TableName);
                wb.Worksheet(dt3.TableName).Row(1).Style.Font.Bold = true;

                wb.SaveAs(fileName);
            }

            return fileName;
        }
    }
}