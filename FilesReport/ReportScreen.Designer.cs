namespace FilesReport
{
    partial class ReportScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRootFolder = new System.Windows.Forms.Label();
            this.btnProcessReport = new System.Windows.Forms.Button();
            this.txtSummaryReport = new System.Windows.Forms.TextBox();
            this.lblTotalFiles = new System.Windows.Forms.Label();
            this.lblLastReport = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblRootFolder
            // 
            this.lblRootFolder.AutoSize = true;
            this.lblRootFolder.Location = new System.Drawing.Point(12, 9);
            this.lblRootFolder.Name = "lblRootFolder";
            this.lblRootFolder.Size = new System.Drawing.Size(82, 17);
            this.lblRootFolder.TabIndex = 0;
            this.lblRootFolder.Text = "RootFolder:";
            // 
            // btnProcessReport
            // 
            this.btnProcessReport.Location = new System.Drawing.Point(15, 75);
            this.btnProcessReport.Name = "btnProcessReport";
            this.btnProcessReport.Size = new System.Drawing.Size(238, 23);
            this.btnProcessReport.TabIndex = 1;
            this.btnProcessReport.Text = "ProcessReport";
            this.btnProcessReport.UseVisualStyleBackColor = true;
            this.btnProcessReport.Click += new System.EventHandler(this.btnProcessReport_Click);
            // 
            // txtSummaryReport
            // 
            this.txtSummaryReport.Enabled = false;
            this.txtSummaryReport.Location = new System.Drawing.Point(15, 127);
            this.txtSummaryReport.Multiline = true;
            this.txtSummaryReport.Name = "txtSummaryReport";
            this.txtSummaryReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSummaryReport.Size = new System.Drawing.Size(1240, 516);
            this.txtSummaryReport.TabIndex = 2;
            // 
            // lblTotalFiles
            // 
            this.lblTotalFiles.AutoSize = true;
            this.lblTotalFiles.Location = new System.Drawing.Point(12, 35);
            this.lblTotalFiles.Name = "lblTotalFiles";
            this.lblTotalFiles.Size = new System.Drawing.Size(77, 17);
            this.lblTotalFiles.TabIndex = 3;
            this.lblTotalFiles.Text = "Total Files:";
            // 
            // lblLastReport
            // 
            this.lblLastReport.AutoSize = true;
            this.lblLastReport.Location = new System.Drawing.Point(15, 660);
            this.lblLastReport.Name = "lblLastReport";
            this.lblLastReport.Size = new System.Drawing.Size(112, 17);
            this.lblLastReport.TabIndex = 4;
            this.lblLastReport.Text = "Last Report File:";
            // 
            // ReportScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1267, 703);
            this.Controls.Add(this.lblLastReport);
            this.Controls.Add(this.lblTotalFiles);
            this.Controls.Add(this.txtSummaryReport);
            this.Controls.Add(this.btnProcessReport);
            this.Controls.Add(this.lblRootFolder);
            this.Name = "ReportScreen";
            this.Text = "ReportScreen";
            this.Load += new System.EventHandler(this.ReportScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRootFolder;
        private System.Windows.Forms.Button btnProcessReport;
        private System.Windows.Forms.TextBox txtSummaryReport;
        private System.Windows.Forms.Label lblTotalFiles;
        private System.Windows.Forms.Label lblLastReport;
    }
}