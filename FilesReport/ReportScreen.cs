using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DocCore;
using System.Threading;
using System.Configuration;


namespace FilesReport
{
    public partial class ReportScreen : Form
    {
        public ReportScreen()
        {
            InitializeComponent();
        }

        ReportGenerator reportGenerator;

        private void ReportScreen_Load(object sender, EventArgs e)
        {
            EngineConfiguration engConf = new EngineConfiguration();

            string rootFolder = engConf.PathFolderRepository;
            this.lblRootFolder.Text += " " + rootFolder;

            FilesReport.ReportGenerator.SignalReportProgress signalProgress = new ReportGenerator.SignalReportProgress(SignalStatus);
            FilesReport.ReportGenerator.SignalReportDone signalWorkDone = new ReportGenerator.SignalReportDone(OpenNewReport);

            this.reportGenerator = new ReportGenerator(rootFolder, signalProgress, signalWorkDone);
        }

        private void btnProcessReport_Click(object sender, EventArgs e)
        {
            this.btnProcessReport.Enabled = false;

            Thread tr = new Thread(reportGenerator.ProcessFilesReport);
            tr.Start();
        }

        private delegate void DelegateSignalStatus(long quant);

        private void SignalStatus(long quant)
        {
            if (this.lblTotalFiles.InvokeRequired)
            {

                this.lblTotalFiles.BeginInvoke(new DelegateSignalStatus(SignalStatus), quant);
            }
            else
            {
                this.lblTotalFiles.Text = "Total Files: " + quant.ToString();
            }
        }

        private delegate void DelegateOpenNewReport(List<SummaryDocType> list, IRepositorySummaryDocType report);

        private void CallReport(List<SummaryDocType> list, IRepositorySummaryDocType report)
        {
            string file = ((RepositorySummaryDocTypeTXT)report).ReportFilePath;
            long fullSizeBytes = 0;

            foreach (SummaryDocType item in list)
            {
               this.txtSummaryReport.Text += item.ToString();
               fullSizeBytes += item.TotalSize;
            }

            this.lblLastReport.Text = "Last Report file in: " + file;
            this.lblTotalFiles.Text += " - " + ReportGenerator.GetFormatedSizeString(fullSizeBytes);
        }

        private void OpenNewReport(IRepositorySummaryDocType report)
        {
            List<SummaryDocType> listEntry = report.List();

            if (this.txtSummaryReport.InvokeRequired)
            {
                this.txtSummaryReport.BeginInvoke(new DelegateOpenNewReport(CallReport), listEntry, report);
            }
            else
                CallReport(listEntry, report);
        }
    }
}
