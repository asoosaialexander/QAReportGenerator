using System;
using System.Collections.Generic;
using System.Data;
using ClosedXML.Excel;

namespace QAReportGenerator
{
    public static class ExcelClient
    {
        public static void GenerateReport(List<Project> data)
        {
            DataTable dt = new DataTable();
            dt.TableName = "QA Report";
            dt.Columns.Add("Team");
            dt.Columns.Add("Repository");
            dt.Columns.Add("Branch");
            dt.Columns.Add("Scanned date");
            dt.Columns.Add("Code coverage");
            dt.Columns.Add("Vulnerabilities");
            dt.Columns.Add("Code smells");

            foreach (var project in data)
            {
                DataRow dr = dt.NewRow();
                dr["Team"] = project.Team;
                dr["Repository"] = project.Repository;
                dr["Branch"] = project.Branch;
                dr["Scanned date"] =
                project.Metrics.Measures.Find(m => m.Metric == "coverage").History[0].Date;
                dr["Code coverage"] =
                    project.Metrics.Measures.Find(m => m.Metric == "coverage").History[0].Value;
                dr["Vulnerabilities"] =
                    project.Metrics.Measures.Find(m => m.Metric == "vulnerabilities").History[0].Value;
                dr["Code smells"] =
                    project.Metrics.Measures.Find(m => m.Metric == "code_smells").History[0].Value;

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();

            string fileName = "QA_Report_"+DateTime.Now.ToShortDateString()+".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Sonarqube Report");
                wb.Worksheet("Sonarqube Report").Row(1).Style.Font.Bold = true;
                wb.SaveAs(fileName);
            }
        }
    }
}