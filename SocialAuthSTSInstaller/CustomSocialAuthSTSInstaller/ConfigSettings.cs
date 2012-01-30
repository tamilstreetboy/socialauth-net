using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CustomSocialAuthSTSInstaller
{
    public partial class ConfigSettings : Form
    {
        public ConfigSettings()
        {
            InitializeComponent();

            ExpandCollapse(pnlFacebook, false);
            ExpandCollapse(pnlLinkedIn, false);
            ExpandCollapse(pnlTwitter, false);
            ExpandCollapse(pnlGoogle, false);
            ExpandCollapse(pnlMSN, false);
            ExpandCollapse(pnlMySpace, false);
            ExpandCollapse(pnlYahoo, false);
        }

        #region Events
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            ValidateInput();

            UpdateInstaller();
        }
        private void chkFacebook_CheckedChanged(object sender, EventArgs e)
        {
            txtFBKey.Enabled = chkFacebook.Checked;
            txtFBSecret.Enabled = chkFacebook.Checked;
            ExpandCollapse(pnlFacebook, chkFacebook.Checked);
        }

        private void chkLinkedIn_CheckedChanged(object sender, EventArgs e)
        {
            txtLinkedInKey.Enabled = chkLinkedIn.Checked;
            txtLinkedInSecret.Enabled = chkLinkedIn.Checked;
            ExpandCollapse(pnlLinkedIn, chkLinkedIn.Checked);
        }

        private void chkTwitter_CheckedChanged(object sender, EventArgs e)
        {
            txtTwitterKey.Enabled = chkTwitter.Checked;
            txtTwitterSecret.Enabled = chkTwitter.Checked;
            ExpandCollapse(pnlTwitter, chkTwitter.Checked);
        }

        private void chkGoogle_CheckedChanged(object sender, EventArgs e)
        {
            txtGoogleKey.Enabled = chkGoogle.Checked;
            txtGoogleSecret.Enabled = chkGoogle.Checked;
            ExpandCollapse(pnlGoogle, chkGoogle.Checked);
        }

        private void chkMSN_CheckedChanged(object sender, EventArgs e)
        {
            txtMSNKey.Enabled = chkMSN.Checked;
            txtMSNSecret.Enabled = chkMSN.Checked;
            ExpandCollapse(pnlMSN, chkMSN.Checked);
        }

        private void chkMySpace_CheckedChanged(object sender, EventArgs e)
        {
            txtMyspaceKey.Enabled = chkMySpace.Checked;
            txtMyspaceSecret.Enabled = chkMySpace.Checked;
            ExpandCollapse(pnlMySpace, chkMySpace.Checked);
        }

        private void chkYahoo_CheckedChanged(object sender, EventArgs e)
        {
            txtYahooKey.Enabled = chkYahoo.Checked;
            txtYahooSecret.Enabled = chkYahoo.Checked;
            ExpandCollapse(pnlYahoo, chkYahoo.Checked);
        }

        /// <summary>
        /// Method for expanding/collapsing a specific provider panel
        /// </summary>
        /// <param name="currentPanel">Panel being expanded/collapsed</param>
        /// <param name="expand">Bit indicating whether panel is being expanded</param>
        private void ExpandCollapse(Panel currentPanel, bool expand)
        {
            int height = currentPanel.Height;
            currentPanel.Visible = expand;

            foreach (var c in this.Controls.OfType<CheckBox>())
            {
                if (c.TabIndex > currentPanel.TabIndex && expand)
                {
                    c.Top += height;
                }
                else if (c.TabIndex > currentPanel.TabIndex && !expand)
                {
                    c.Top -= height;
                }
            }

            foreach (var c in this.Controls.OfType<Panel>())
            {
                if (c.TabIndex > currentPanel.TabIndex && expand)
                {
                    c.Top += height;
                }
                else if (c.TabIndex > currentPanel.TabIndex && !expand)
                {
                    c.Top -= height;
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Method for validating user input.
        /// </summary>
        private void ValidateInput()
        {
            if (chkFacebook.Checked)
            {
                if (string.IsNullOrEmpty(txtFBKey.Text) ||
                    string.IsNullOrEmpty(txtFBSecret.Text))
                    MessageBox.Show("Please specify the facebook key and secret.");
            }
            else if (chkLinkedIn.Checked)
            {
                if (string.IsNullOrEmpty(txtLinkedInKey.Text) ||
                    string.IsNullOrEmpty(txtLinkedInSecret.Text))
                    MessageBox.Show("Please specify the LinkedIn key and secret.");
            }
            else if (chkTwitter.Checked)
            {
                if (string.IsNullOrEmpty(txtTwitterKey.Text) ||
                    string.IsNullOrEmpty(txtTwitterSecret.Text))
                    MessageBox.Show("Please specify the Twitter key and secret.");
            }
            else if (chkGoogle.Checked)
            {
                if (string.IsNullOrEmpty(txtGoogleKey.Text) ||
                    string.IsNullOrEmpty(txtGoogleSecret.Text))
                    MessageBox.Show("Please specify the Google key and secret.");
            }
            else if (chkMSN.Checked)
            {
                if (string.IsNullOrEmpty(txtMSNKey.Text) ||
                    string.IsNullOrEmpty(txtMSNSecret.Text))
                    MessageBox.Show("Please specify the MSN key and secret.");
            }
            else if (chkMySpace.Checked)
            {
                if (string.IsNullOrEmpty(txtMyspaceKey.Text) ||
                    string.IsNullOrEmpty(txtMyspaceSecret.Text))
                    MessageBox.Show("Please specify the Myspace key and secret.");
            }
            else if (chkYahoo.Checked)
            {
                if (string.IsNullOrEmpty(txtYahooKey.Text) ||
                    string.IsNullOrEmpty(txtYahooSecret.Text))
                    MessageBox.Show("Please specify the Yahoo key and secret.");
            }
        }

        /// <summary>
        /// Method for saving user input to be used later by the installer
        /// </summary>
        private void UpdateInstaller()
        {
            InstallationData.Providers.Clear();

            if (chkFacebook.Checked)
            {
                InstallationData.Providers.Add(new Provider()
                    {WrapperName="Facebook", 
                        ConsumerKey=txtFBKey.Text, 
                        ConsumerSecret = txtFBSecret.Text});
            }

            if (chkLinkedIn.Checked)
            {
                InstallationData.Providers.Add(new Provider()
                {
                    WrapperName = "LinkedIn",
                    ConsumerKey = txtLinkedInKey.Text,
                    ConsumerSecret = txtLinkedInSecret.Text
                });
            }

            if (chkTwitter.Checked)
            {
                InstallationData.Providers.Add(new Provider()
                {
                    WrapperName = "Twitter",
                    ConsumerKey = txtTwitterKey.Text,
                    ConsumerSecret = txtTwitterSecret.Text
                });
            }

            if (chkGoogle.Checked)
            {
                InstallationData.Providers.Add(new Provider()
                {
                    WrapperName = "Google",
                    ConsumerKey = txtGoogleKey.Text,
                    ConsumerSecret = txtGoogleSecret.Text
                });
            }

            if (chkMSN.Checked)
            {
                InstallationData.Providers.Add(new Provider()
                {
                    WrapperName = "MSN",
                    ConsumerKey = txtMSNKey.Text,
                    ConsumerSecret = txtMSNSecret.Text
                });
            }

            if (chkMySpace.Checked)
            {
                InstallationData.Providers.Add(new Provider()
                {
                    WrapperName = "Myspace",
                    ConsumerKey = txtMyspaceKey.Text,
                    ConsumerSecret = txtMyspaceSecret.Text
                });
            }

            if (chkYahoo.Checked)
            {
                InstallationData.Providers.Add(new Provider()
                {
                    WrapperName = "Yahoo",
                    ConsumerKey = txtYahooKey.Text,
                    ConsumerSecret = txtYahooSecret.Text
                });
            }

            this.Close();
        }

        #endregion
    




    }
}
