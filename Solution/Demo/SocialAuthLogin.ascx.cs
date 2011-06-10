using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Brickred.SocialAuth.NET.Core;

using Brickred.SocialAuth.NET.Core.BusinessObjects;


public partial class SocialAuthLogin : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ProviderFactory.Providers.ForEach(p =>
            this.FindControl("providerContainer").Controls.Add(constructControl(p))
            );
    }

    private HtmlGenericControl constructControl(Provider provider)
    {
        string iconPath = Utility.GetConfiguration().IconFolder.Path + provider.ProviderType.ToString() + ".png";

        HtmlGenericControl providerDiv = new HtmlGenericControl("div");
        providerDiv.Attributes.Add("class", "provider");
        ImageButton imgB = new ImageButton()
        {
            ID = "img" + provider.ProviderType,
            CommandArgument = provider.ProviderType.ToString(),
            ImageUrl = iconPath
        };
        imgB.Command += new CommandEventHandler(imgB_Command);
        providerDiv.Controls.Add(imgB);
        return providerDiv;
    }

    private void imgB_Command(object sender, EventArgs e)
    {
        SocialAuthUser oUser = new SocialAuthUser((PROVIDER_TYPE)Enum.Parse(typeof(PROVIDER_TYPE), ((CommandEventArgs)e).CommandArgument.ToString()));
        oUser.Login();
    }
}