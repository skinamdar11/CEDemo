namespace DesktopApplication
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
            this.btnStartAuth = new System.Windows.Forms.Button();
            this.btnUseToken = new System.Windows.Forms.Button();
            this.btnCleanup = new System.Windows.Forms.Button();
            this.btnStartAuthAttemptAsync = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnStartAuth
            // 
            this.btnStartAuth.Location = new System.Drawing.Point(12, 12);
            this.btnStartAuth.Name = "btnStartAuth";
            this.btnStartAuth.Size = new System.Drawing.Size(260, 49);
            this.btnStartAuth.TabIndex = 0;
            this.btnStartAuth.Text = "Start Authorisation Attempt";
            this.btnStartAuth.UseVisualStyleBackColor = true;
            this.btnStartAuth.Click += new System.EventHandler(this.btnStartAuth_Click);
            // 
            // btnUseToken
            // 
            this.btnUseToken.Enabled = false;
            this.btnUseToken.Location = new System.Drawing.Point(12, 134);
            this.btnUseToken.Name = "btnUseToken";
            this.btnUseToken.Size = new System.Drawing.Size(260, 50);
            this.btnUseToken.TabIndex = 1;
            this.btnUseToken.Text = "Use Token";
            this.btnUseToken.UseVisualStyleBackColor = true;
            this.btnUseToken.Click += new System.EventHandler(this.btnUseToken_Click);
            // 
            // btnCleanup
            // 
            this.btnCleanup.Location = new System.Drawing.Point(12, 190);
            this.btnCleanup.Name = "btnCleanup";
            this.btnCleanup.Size = new System.Drawing.Size(260, 53);
            this.btnCleanup.TabIndex = 2;
            this.btnCleanup.Text = "Cleanup";
            this.btnCleanup.UseVisualStyleBackColor = true;
            this.btnCleanup.Click += new System.EventHandler(this.btnCleanup_Click);
            // 
            // btnStartAuthAttemptAsync
            // 
            this.btnStartAuthAttemptAsync.Location = new System.Drawing.Point(12, 67);
            this.btnStartAuthAttemptAsync.Name = "btnStartAuthAttemptAsync";
            this.btnStartAuthAttemptAsync.Size = new System.Drawing.Size(260, 49);
            this.btnStartAuthAttemptAsync.TabIndex = 3;
            this.btnStartAuthAttemptAsync.Text = "Start Authorisation Attempt Async";
            this.btnStartAuthAttemptAsync.UseVisualStyleBackColor = true;
            this.btnStartAuthAttemptAsync.Click += new System.EventHandler(this.btnStartAuthAttemptAsync_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(14, 124);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(257, 1);
            this.panel1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 254);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnStartAuthAttemptAsync);
            this.Controls.Add(this.btnCleanup);
            this.Controls.Add(this.btnUseToken);
            this.Controls.Add(this.btnStartAuth);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartAuth;
        private System.Windows.Forms.Button btnUseToken;
        private System.Windows.Forms.Button btnCleanup;
        private System.Windows.Forms.Button btnStartAuthAttemptAsync;
        private System.Windows.Forms.Panel panel1;
    }
}

