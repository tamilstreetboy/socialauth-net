using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration.Install;
using System.Diagnostics;

namespace CustomSocialAuthSTSInstaller
{
    public partial class ProviderDetails : Form
    {
        bool _cancelInstallation = true;

        public ProviderDetails()
        {
            InitializeComponent();

            InitializeForm();
        }

        private void InitializeForm()
        {
            pnlRegister.Visible = true;
            switch (InstallationData.Providers[InstallationData.CurrentIndex].WrapperName)
            {
                case Provider.Wrapper.TwitterWrapper:
                    picProvider.Image = CustomSocialAuthSTSInstaller.Properties.Resources.twitter;
                    lblSelectProviders.Text = "Twitter Configuration";
                    lnkRegister.Text = "Twitter";
                    lnkRegister.Tag = "http://twitter.com/apps";
                    break;
                case Provider.Wrapper.FacebookWrapper:
                    picProvider.Image = CustomSocialAuthSTSInstaller.Properties.Resources.facebook;
                    lblSelectProviders.Text = "Facebook Configuration";
                    lnkRegister.Text = "Facebook";
                    lnkRegister.Tag = "http://www.facebook.com/developers/apps.php";
                    break;
                case Provider.Wrapper.GoogleWrapper:
                    picProvider.Image = CustomSocialAuthSTSInstaller.Properties.Resources.google;
                    lblSelectProviders.Text = "Google Configuration";
                    lnkRegister.Text = "Google";
                    lnkRegister.Tag = "http://code.google.com/apis/accounts/docs/RegistrationForWebAppsAuto.html";
                    break;
                case Provider.Wrapper.MSNWrapper:
                    picProvider.Image = CustomSocialAuthSTSInstaller.Properties.Resources.msn;
                    lblSelectProviders.Text = "MSN Configuration";
                    lnkRegister.Text = "MSN";
                    lnkRegister.Tag = "http://msdn.microsoft.com/en-us/library/cc287659.aspx";
                    break;
                case Provider.Wrapper.YahooWrapper:
                    picProvider.Image = CustomSocialAuthSTSInstaller.Properties.Resources.yahoo;
                    lblSelectProviders.Text = "Yahoo Configuration";
                    lnkRegister.Text = "Yahoo";
                    lnkRegister.Tag = "https://developer.apps.yahoo.com/dashboard/createKey.html";
                    break;
                case Provider.Wrapper.MySpaceWrapper:
                    picProvider.Image = CustomSocialAuthSTSInstaller.Properties.Resources.myspace;
                    lblSelectProviders.Text = "MySpace Configuration";
                    lnkRegister.Text = "MySpace";
                    lnkRegister.Tag = "http://developer.myspace.com/Apps.mvc";
                    break;
                case Provider.Wrapper.LinkedInWrapper:
                    picProvider.Image = CustomSocialAuthSTSInstaller.Properties.Resources.linkedin;
                    lblSelectProviders.Text = "LinkedIn Configuration";
                    lnkRegister.Text = "LinkedIn";
                    lnkRegister.Tag = "https://www.linkedin.com/secure/developer";
                    break;
            }
            _cancelInstallation = true;
            Provider currentProvider = InstallationData.Providers[InstallationData.CurrentIndex];
            txtKey.Text = currentProvider.ConsumerKey;
            txtSecret.Text = currentProvider.ConsumerSecret;

            if (currentProvider.ScopeTypeCustom)
            {
                cmbScope.SelectedIndex = 1;
                txtAdditionalScopes.Text = currentProvider.ScopeType;
            }
            else
            {
                cmbScope.SelectedIndex = 0;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Confirm Cancel", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                InstallationData.InstallationCanceled = true;
                this.Close();
            }
        }        

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                int index = InstallationData.CurrentIndex++;

                InstallationData.Providers[index].ConsumerKey = txtKey.Text;
                InstallationData.Providers[index].ConsumerSecret = txtSecret.Text;

                if (cmbScope.SelectedIndex == 0)
                {
                    InstallationData.Providers[index].ScopeTypeCustom = false;
                }
                else
                {
                    InstallationData.Providers[index].ScopeTypeCustom = true;
                    InstallationData.Providers[index].ScopeType = txtAdditionalScopes.Text;
                }

                if (InstallationData.CurrentIndex >= InstallationData.Providers.Count)
                {
                    _cancelInstallation = false;
                    this.Close();
                }
                else
                {
                    this.InitializeForm();
                }
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(txtKey.Text) || string.IsNullOrEmpty(txtSecret.Text))
            {
                MessageBox.Show("Please enter the key and the value");
                return false;
            }
            else if (cmbScope.SelectedIndex == 1
                & string.IsNullOrEmpty(txtAdditionalScopes.Text))
            {
                MessageBox.Show("Please enter additional scopes.");
                return false;
            }
            
            return true;
        }

        private void cmbScope_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbScope.SelectedIndex == 0)
                pnlScope.Visible = false;
            else
                pnlScope.Visible = true;
        }

        private void ProviderDetails_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(_cancelInstallation)
                InstallationData.InstallationCanceled = true;
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string sUrl = lnkRegister.Tag.ToString();
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", sUrl);
                System.Diagnostics.Process.Start(startInfo);
                startInfo = null;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Installer is not able to launch the link " + sUrl);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (InstallationData.CurrentIndex == 0)
            {
                InstallationData.InstallationCanceled = false;
                InstallationData.BrowseBackMode = true;
                this.Close();
            }
            else
            {
                InstallationData.CurrentIndex--;
                this.InitializeForm();
            }
        }

    }
}
