using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Google Chart
/// <summary>
namespace Utility.Controls
{
    public class GoogleChart : Control
    {
        private string i_ChartName;
        private string i_FunctionName;

        public GoogleChart()
        {
            this.i_ChartName = "chart_" + DateTime.Now.Ticks.ToString();
            this.i_FunctionName = "draw_" + this.i_ChartName;

            xName = "";
            yType = "number";
            yName = "";
            Title = "";
            Width = 100;
            Height = 100;
        }

        public void Generate()
        {
            if (PackageList == null || Data == null || PackageList.Count == 0 || Data.Count == 0) { return; }

            StringBuilder sb = new StringBuilder();
            // Load Packages
            foreach (string pack in PackageList)
            {
                sb.AppendLine(LoadPackage(pack));
            }

            // Draw Chart Function
            sb.AppendLine(DrawChart());

            // Set Callback
            sb.AppendLine(SetCallBack());

            string APIRef = GetAPIRef();
            string holderHeader = @"<script type=""text/javascript"">";
            string holder = sb.ToString();
            string holderBottom = "</" + "script>";
            string chartRef = String.Format(@"<div id=""{0}"" style=""width: 900px; height: 500px;""></" + "div>", i_ChartName);
            this.Controls.Add(new LiteralControl(APIRef));
            this.Controls.Add(new LiteralControl(holderHeader));
            this.Controls.Add(new LiteralControl(holder));
            this.Controls.Add(new LiteralControl(holderBottom));
            this.Controls.Add(new LiteralControl(chartRef));
        }

        private string GetAPIRef()
        {
            return @"<script type=""text/javascript"" src=""https://www.google.com/jsapi""></" + "script>";
        }

        private string LoadPackage(string pack)
        {
            // Load the Visualization API and the piechart package.
            return String.Format(" google.load('visualization', '1.1', {{ 'packages': ['{0}'] }}); ", pack);
        }

        private string DrawChart()
        {
            StringBuilder tempBuilder = new StringBuilder();
            tempBuilder.AppendLine(String.Format("function {0}() {{ ", i_FunctionName));
            tempBuilder.AppendLine("var data = new google.visualization.DataTable(); ");
            tempBuilder.AppendLine(String.Format("data.addColumn('string', '{0}'); ", xName));
            tempBuilder.AppendLine(String.Format("data.addColumn('{0}', '{1}'); ", yType, yName));

            tempBuilder.AppendLine("data.addRows([ ");

            bool isFirst = true;
            foreach (string key in Data.Keys)
            {
                if (!isFirst) { tempBuilder.AppendLine(", "); }
                tempBuilder.AppendLine(String.Format("['{0}',{1}]", key, Data[key]));
                isFirst = false;
            }

            tempBuilder.AppendLine("]); ");

            tempBuilder.AppendLine("var options = {");
            tempBuilder.AppendLine(String.Format("'title':'{0}', ", Title));
            tempBuilder.AppendLine(String.Format("'width':'{0}', ", Width));
            tempBuilder.AppendLine(String.Format("'height':'{0}'}};", Height));

            tempBuilder.AppendLine(String.Format("var chart = new google.visualization.LineChart(document.getElementById('{0}'));", i_ChartName));
            tempBuilder.AppendLine("chart.draw(data, options); ");
            tempBuilder.AppendLine("} ");
            return tempBuilder.ToString();
        }

        private string SetCallBack()
        {
            // Set a callback to run when the Google Visualization API is loaded.
            return String.Format(" google.setOnLoadCallback({0}); ", i_FunctionName);
        }

        public List<string> PackageList { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string xName { get; set; }
        public string yType { get; set; }
        public string yName { get; set; }
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}