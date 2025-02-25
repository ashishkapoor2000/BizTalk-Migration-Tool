namespace BizTalk_Migration_Tool
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MigrationForm : Form
    {
        private Button analyzeButton;
        private Button reportButton;
        private Button convertButton;
        private TextBox inputPathTextBox;
        private TextBox outputPathTextBox;
        private Label inputLabel;
        private Label outputLabel;
        private ProgressBar progressBar;
        public MigrationForm()
        {
            this.Text = "BizTalk Migration Tool";
            this.Width = 450;
            this.Height = 300;

            inputLabel = new Label() { Text = "Input Path:", Left = 10, Top = 20, Width = 80 };
            inputPathTextBox = new TextBox() { Left = 100, Top = 20, Width = 300 };
            outputLabel = new Label() { Text = "Output Path:", Left = 10, Top = 60, Width = 80 };
            outputPathTextBox = new TextBox() { Left = 100, Top = 60, Width = 300 };

            analyzeButton = new Button() { Text = "Analyze", Left = 10, Top = 100, Width = 100 };
            analyzeButton.Click += (sender, e) => RunAnalyze();

            reportButton = new Button() { Text = "Generate Report", Left = 120, Top = 100, Width = 120 };
            reportButton.Click += (sender, e) => RunReport();

            convertButton = new Button() { Text = "Convert", Left = 250, Top = 100, Width = 100 };
            convertButton.Click += (sender, e) => RunConvert();

            progressBar = new ProgressBar() { Left = 10, Top = 140, Width = 380, Height = 20, Visible = false };

            this.Controls.Add(inputLabel);
            this.Controls.Add(inputPathTextBox);
            this.Controls.Add(outputLabel);
            this.Controls.Add(outputPathTextBox);
            this.Controls.Add(analyzeButton);
            this.Controls.Add(reportButton);
            this.Controls.Add(convertButton);
            this.Controls.Add(progressBar);
            InitializeComponent();
        }

        private void RunAnalyze()
        {
            string inputPath = inputPathTextBox.Text;
            if (!Directory.Exists(inputPath))
            {
                MessageBox.Show("Invalid input path.");
                return;
            }

            progressBar.Visible = true;
            progressBar.Value = 10;

            var files = Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories)
                                 .Where(f => f.EndsWith(".odx") || f.EndsWith(".xsd") || f.EndsWith(".btm") || f.EndsWith(".dll"))
                                 .ToList();

            progressBar.Value = 50;
            MessageBox.Show($"Found {files.Count} BizTalk components.");
            progressBar.Value = 100;
            progressBar.Visible = false;
        }

        private void RunReport()
        {
            string inputPath = inputPathTextBox.Text;
            string outputPath = outputPathTextBox.Text;
            if (!Directory.Exists(inputPath) || string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show("Invalid input or output path.");
                return;
            }

            progressBar.Visible = true;
            progressBar.Value = 20;

            var files = Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories)
                                 .Where(f => f.EndsWith(".odx") || f.EndsWith(".xsd") || f.EndsWith(".btm") || f.EndsWith(".dll"))
                                 .ToList();

            StringBuilder reportContent = new StringBuilder();
            reportContent.AppendLine("Component Type,File Name,Migration Feasibility");

            foreach (var file in files)
            {
                string type=string.Empty;
                switch(Path.GetExtension(file))
                {
                    case ".odx":
                        type = "Orchestration";
                        break;
                    case ".xsd":
                        type = "Schema";
                        break;
                    case ".btm":
                        type = "Map";
                        break;
                    case ".dll":
                        type = "Pipeline/Other";
                        break;
                    default:
                        type = "Unknown";
                        break;

                }
                //string type = Path.GetExtension(file) switch
                //{
                //    ".odx" => "Orchestration",
                //    ".xsd" => "Schema",
                //    ".btm" => "Map",
                //    ".dll" => "Pipeline/Other",
                //    _ => "Unknown"
                //};
                string feasibility = (type == "Orchestration" || type == "Pipeline/Other") ? "Manual" : "Automated";
                reportContent.AppendLine($"{type},{Path.GetFileName(file)},{feasibility}");
            }

            File.WriteAllText(outputPath, reportContent.ToString());
            progressBar.Value = 100;
            MessageBox.Show("Migration report generated successfully.");
            progressBar.Visible = false;
        }

        private void RunConvert()
        {
            MessageBox.Show("Converting BizTalk bindings...");
            // TODO: Implement BTDF package conversion logic
        }
    }
}
