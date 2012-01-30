namespace CustomSocialAuthSTSInstaller
{
    partial class ProviderDetails
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSelectProviders = new System.Windows.Forms.Label();
            this.picProvider = new System.Windows.Forms.PictureBox();
            this.txtSecret = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbScope = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlScope = new System.Windows.Forms.Panel();
            this.txtAdditionalScopes = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlRegister = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.lnkRegister = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picProvider)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.pnlScope.SuspendLayout();
            this.pnlRegister.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblSelectProviders);
            this.panel1.Controls.Add(this.picProvider);
            this.panel1.Location = new System.Drawing.Point(-4, -4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 71);
            this.panel1.TabIndex = 1;
            // 
            // lblSelectProviders
            // 
            this.lblSelectProviders.AutoSize = true;
            this.lblSelectProviders.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectProviders.ForeColor = System.Drawing.Color.Black;
            this.lblSelectProviders.Location = new System.Drawing.Point(68, 25);
            this.lblSelectProviders.Name = "lblSelectProviders";
            this.lblSelectProviders.Size = new System.Drawing.Size(118, 18);
            this.lblSelectProviders.TabIndex = 1;
            this.lblSelectProviders.Text = "Configuration";
            // 
            // picProvider
            // 
            this.picProvider.Location = new System.Drawing.Point(15, 9);
            this.picProvider.Name = "picProvider";
            this.picProvider.Size = new System.Drawing.Size(50, 50);
            this.picProvider.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picProvider.TabIndex = 0;
            this.picProvider.TabStop = false;
            // 
            // txtSecret
            // 
            this.txtSecret.Location = new System.Drawing.Point(154, 136);
            this.txtSecret.Name = "txtSecret";
            this.txtSecret.Size = new System.Drawing.Size(233, 20);
            this.txtSecret.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Secret:";
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(154, 98);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(233, 20);
            this.txtKey.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Key:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(78, 177);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Scope Level:";
            // 
            // cmbScope
            // 
            this.cmbScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScope.Items.AddRange(new object[] {
            "Default",
            "Custom"});
            this.cmbScope.Location = new System.Drawing.Point(155, 174);
            this.cmbScope.Name = "cmbScope";
            this.cmbScope.Size = new System.Drawing.Size(232, 21);
            this.cmbScope.TabIndex = 10;
            this.cmbScope.SelectedIndexChanged += new System.EventHandler(this.cmbScope_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBack);
            this.groupBox1.Controls.Add(this.btnNext);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Location = new System.Drawing.Point(-3, 323);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(493, 52);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(306, 15);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(83, 23);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "< Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(395, 15);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(83, 23);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(217, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(83, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlScope
            // 
            this.pnlScope.Controls.Add(this.txtAdditionalScopes);
            this.pnlScope.Controls.Add(this.label4);
            this.pnlScope.Location = new System.Drawing.Point(44, 198);
            this.pnlScope.Name = "pnlScope";
            this.pnlScope.Size = new System.Drawing.Size(352, 50);
            this.pnlScope.TabIndex = 19;
            this.pnlScope.Visible = false;
            // 
            // txtAdditionalScopes
            // 
            this.txtAdditionalScopes.Location = new System.Drawing.Point(111, 15);
            this.txtAdditionalScopes.Name = "txtAdditionalScopes";
            this.txtAdditionalScopes.Size = new System.Drawing.Size(233, 20);
            this.txtAdditionalScopes.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Scopes:";
            // 
            // pnlRegister
            // 
            this.pnlRegister.Controls.Add(this.label5);
            this.pnlRegister.Controls.Add(this.lnkRegister);
            this.pnlRegister.Controls.Add(this.label6);
            this.pnlRegister.Location = new System.Drawing.Point(-1, 261);
            this.pnlRegister.Name = "pnlRegister";
            this.pnlRegister.Size = new System.Drawing.Size(489, 47);
            this.pnlRegister.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Left;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(263, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "If you don’t have registration keys/secrets, please visit";
            // 
            // lnkRegister
            // 
            this.lnkRegister.AutoEllipsis = true;
            this.lnkRegister.AutoSize = true;
            this.lnkRegister.Dock = System.Windows.Forms.DockStyle.Left;
            this.lnkRegister.Location = new System.Drawing.Point(272, 0);
            this.lnkRegister.Name = "lnkRegister";
            this.lnkRegister.Size = new System.Drawing.Size(52, 13);
            this.lnkRegister.TabIndex = 2;
            this.lnkRegister.TabStop = true;
            this.lnkRegister.Text = "MySpace";
            this.lnkRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRegister_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Left;
            this.label6.Location = new System.Drawing.Point(330, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "to register your application.";
            // 
            // ProviderDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 373);
            this.Controls.Add(this.pnlRegister);
            this.Controls.Add(this.pnlScope);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmbScope);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSecret);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProviderDetails";
            this.ShowInTaskbar = false;
            this.Text = "STSInstaller";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProviderDetails_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picProvider)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.pnlScope.ResumeLayout(false);
            this.pnlScope.PerformLayout();
            this.pnlRegister.ResumeLayout(false);
            this.pnlRegister.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picProvider;
        private System.Windows.Forms.TextBox txtSecret;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbScope;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlScope;
        private System.Windows.Forms.TextBox txtAdditionalScopes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSelectProviders;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.FlowLayoutPanel pnlRegister;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel lnkRegister;
        private System.Windows.Forms.Label label6;
    }
}