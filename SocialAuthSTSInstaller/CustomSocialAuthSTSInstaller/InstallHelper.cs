using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Collections.Specialized;
using Microsoft.Web.Administration;

namespace CustomSocialAuthSTSInstaller
{
    [RunInstaller(true)]
    public class InstallHelper : Installer
    {
        #region Installer Methods

        /// <summary>
        /// Overrides the install method
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            
            SelectProviders selProviders = new SelectProviders();
            selProviders.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            selProviders.TopMost = true;
            selProviders.ShowDialog();

            if (InstallationData.InstallationCanceled)
            {
                throw new InstallException("Installation cancelled by user.");
            }

            /************************************************************************************************
                * This code constructs and writes config settings for the <Providers> section of 
                * <SocialAuthConfiguration> based on the input provided by the user.
            *************************************************************************************************/
            StringBuilder sbSettings = new StringBuilder("<SocialAuthConfiguration><Providers>");

            foreach (Provider prov in InstallationData.Providers)
            {
                CreateConfigSection(sbSettings, prov);
            }

            sbSettings.Append("</Providers></SocialAuthConfiguration>");

            
            string filePath = (Directory.GetParent(Context.Parameters["assemblypath"]).FullName) +
                @"\Web.config";
            FileStream stmPhysical = new FileStream(filePath, FileMode.Open);
            StreamReader srConfig = new StreamReader(stmPhysical);

            string strConfig = srConfig.ReadToEnd();
            strConfig = strConfig.Replace("<SocialAuthConfiguration></SocialAuthConfiguration>", sbSettings.ToString());
            stmPhysical.Close();
            srConfig.Close();
            StreamWriter swConfig = new StreamWriter(filePath);
            swConfig.Write(strConfig);
            swConfig.Close();
            /************************************************************************************************
            *************************************************************************************************/

            /************************************************************************************************
             * This code makes an entry to the host file for: 127.0.0.1 opensource.brickred.com  
            *************************************************************************************************/
            try
            {
                if (InstallationData.DefaultProviders)
                {
                    bool writeToHostFile = false;
                    string hostFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts");
                    if (!File.Exists(hostFilePath))
                        MessageBox.Show("The installer could not access host file.");

                    using (StreamReader r = new StreamReader(hostFilePath))
                    {
                        if (!r.ReadToEnd().Contains("opensource.brickred.com"))
                            writeToHostFile = true;
                    }
                    if (writeToHostFile)
                    {
                        string ip = "127.0.0.1";
                        string sitename = Context.Parameters["targetsite"].ToString();
                        int siteId = Int32.Parse(sitename.Substring(sitename.LastIndexOf("/") + 1));
                        using (ServerManager serverManager = new ServerManager())
                        {
                            Site site = serverManager.Sites.FirstOrDefault(s => s.Id == siteId);
                            if (site != null)
                            {
                                Microsoft.Web.Administration.Binding binding = site.Bindings
                                   .Where(b => b.Protocol == "http")
                                   .FirstOrDefault();

                                ip = binding.BindingInformation.Substring(0, binding.BindingInformation.IndexOf(":"));
                            }
                        }

                        using (StreamWriter w = File.AppendText(hostFilePath))
                        {
                            w.WriteLine(w.NewLine + (ip == "*" ? "127.0.0.1" : ip) + " opensource.brickred.com");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not write to the host file. This entry would have to be made manually if required.");
            }
            /************************************************************************************************
            *************************************************************************************************/
        }
        
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            //TODO: Need to check if code for removing host file entry can/should be added here
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method for creating config section for a single provide.
        /// </summary>
        /// <param name="sbSettings">Stringbuilder to append settings to</param>
        /// <param name="prov">Provider for which config section is to be created.</param>
        private static void CreateConfigSection(StringBuilder sbSettings, Provider prov)
        {
            sbSettings.Append("<add  WrapperName=\"");
            sbSettings.Append(prov.WrapperName);
            sbSettings.Append("\" ConsumerKey=\"");
            sbSettings.Append(prov.ConsumerKey);
            sbSettings.Append("\"");
            sbSettings.Append(" ConsumerSecret=\"");
            sbSettings.Append(prov.ConsumerSecret);
            sbSettings.Append("\"");

            if (prov.ScopeTypeCustom)
            {
                sbSettings.Append(" AdditionalScopes=\"");
                sbSettings.Append(prov.ScopeType);
                sbSettings.Append("\"");
            }

            sbSettings.Append(" />");
        }

        #endregion
    }
}
