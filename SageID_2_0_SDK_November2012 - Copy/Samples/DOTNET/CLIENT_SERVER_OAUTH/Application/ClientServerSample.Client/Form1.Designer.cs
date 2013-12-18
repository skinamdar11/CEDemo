namespace ClientServerSample.Client
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
            this.btnStartAuthorisation = new System.Windows.Forms.Button();
            this.btnCleanup = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartAuthorisation
            // 
            this.btnStartAuthorisation.Location = new System.Drawing.Point(12, 12);
            this.btnStartAuthorisation.Name = "btnStartAuthorisation";
            this.btnStartAuthorisation.Size = new System.Drawing.Size(260, 53);
            this.btnStartAuthorisation.TabIndex = 0;
            this.btnStartAuthorisation.Text = "Start Authorisation";
            this.btnStartAuthorisation.UseVisualStyleBackColor = true;
            this.btnStartAuthorisation.Click += new System.EventHandler(this.btnStartAuthorisation_Click);
            // 
            // btnCleanup
            // 
            this.btnCleanup.Location = new System.Drawing.Point(12, 71);
            this.btnCleanup.Name = "btnCleanup";
            this.btnCleanup.Size = new System.Drawing.Size(260, 55);
            this.btnCleanup.TabIndex = 2;
            this.btnCleanup.Text = "Cleanup";
            this.btnCleanup.UseVisualStyleBackColor = true;
            this.btnCleanup.Click += new System.EventHandler(this.btnCleanup_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 198);
            this.Controls.Add(this.btnCleanup);
            this.Controls.Add(this.btnStartAuthorisation);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartAuthorisation;
        private System.Windows.Forms.Button btnCleanup;
    }
}

