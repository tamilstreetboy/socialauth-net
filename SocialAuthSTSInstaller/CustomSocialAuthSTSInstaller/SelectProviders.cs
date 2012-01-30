using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration.Install;

namespace CustomSocialAuthSTSInstaller
{
    public partial class SelectProviders : Form
    {
        private bool _cancelInstallation = true;

        public SelectProviders()
        {
            InitializeComponent();

            //TODO: Need to change when back button functionality is added.
            rdCustomProviders.Checked = true;
            this.TopMost = true;
            if (InstallationData.Providers.Count != 0)
            {
                ShowSelectedProviders();
            }
        }

        private void ShowSelectedProviders()
        {
            foreach (Provider p in InstallationData.Providers)
            {
                foreach (CheckBox c in pnlProviders.Controls.OfType<CheckBox>())
                {
                    if (Provider.GetWrapperFromString(c.Tag.ToString()) == p.WrapperName)
                        c.Checked = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Confirm Cancel", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                _cancelInstallation = true;
                this.Close();
            }
        }
        
        private void chkDefaultProvider_CheckedChanged(object sender, EventArgs e)
        {
            switch (rdDefaultProvider.Checked)
            {
                case true:
                    foreach (CheckBox chkBox in pnlProviders.Controls.OfType<CheckBox>())
                        chkBox.Enabled = false;
                    break;
                case false:
                    foreach (CheckBox chkBox in pnlProviders.Controls.OfType<CheckBox>())
                        chkBox.Enabled = true;
                    break;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            InstallationData.DefaultProviders = rdDefaultProvider.Checked;
            if (!rdDefaultProvider.Checked)
            {
                bool selected = false;
                foreach (CheckBox chkBox in pnlProviders.Controls.OfType<CheckBox>())
                {
                    bool providerExists = InstallationData.Providers.Exists(o => o.WrapperName == Provider.GetWrapperFromString(chkBox.Tag.ToString()));
                    if (chkBox.Checked)
                    {
                        if (!providerExists)
                            InstallationData.Providers.Add(new Provider() { WrapperName = Provider.GetWrapperFromString(chkBox.Tag.ToString()) });
                        selected = true;
                    }
                    else
                    {
                        if (providerExists)
                            InstallationData.Providers.RemoveAll(o => o.WrapperName == Provider.GetWrapperFromString(chkBox.Tag.ToString()));
                    }
                }

                if (!selected)
                {
                    MessageBox.Show("Please select at least one provider.");
                    return;
                }

                InstallationData.CurrentIndex = 0;
                InstallationData.BrowseBackMode = false;
                ProviderDetails provDetails = new ProviderDetails();
                provDetails.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                this.TopMost = false;
                provDetails.TopMost = true;
                provDetails.ShowDialog();

                if (!InstallationData.BrowseBackMode)
                {
                    _cancelInstallation = false;
                    this.Close();
                }
                else
                {
                    this.TopMost = true;
                }
            }
            else
            {
                //Default Provider selected
                InstallationData.PopulateDefaultProviders();

                _cancelInstallation = false;
                this.Close();
            }
        }

        private void SelectProviders_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_cancelInstallation)
                InstallationData.InstallationCanceled = true;
            else
                InstallationData.InstallationCanceled = false;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Providers will not be configured. Are you sure you want to proceed?",
                "Confirmation", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
            {
                InstallationData.Providers.Clear();
                _cancelInstallation = false;
                this.Close();
            }

        }



    }
}
