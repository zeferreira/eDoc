namespace FilesReport
{
    partial class Form1
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
            this.txtReport = new System.Windows.Forms.TextBox();
            this.btnRunTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtReport
            // 
            this.txtReport.Location = new System.Drawing.Point(12, 49);
            this.txtReport.Multiline = true;
            this.txtReport.Name = "txtReport";
            this.txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReport.Size = new System.Drawing.Size(1450, 528);
            this.txtReport.TabIndex = 0;
            // 
            // btnRunTest
            // 
            this.btnRunTest.Location = new System.Drawing.Point(12, 13);
            this.btnRunTest.Name = "btnRunTest";
            this.btnRunTest.Size = new System.Drawing.Size(75, 23);
            this.btnRunTest.TabIndex = 1;
            this.btnRunTest.Text = "RunTest";
            this.btnRunTest.UseVisualStyleBackColor = true;
            this.btnRunTest.Click += new System.EventHandler(this.btnRunTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1453, 569);
            this.Controls.Add(this.btnRunTest);
            this.Controls.Add(this.txtReport);
            this.Name = "Form1";
            this.Text = "FilesReport";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtReport;
        private System.Windows.Forms.Button btnRunTest;
    }
}

