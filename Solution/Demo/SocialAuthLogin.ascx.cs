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
    public string DefaultURL
    {
        get;
        set;
    }

    public string Height { get; set; }
    public string Width { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Height))
        {
            providerContainer.Style["Height"] = Height;
        }
        else
        {
            providerContainer.Style["Height"] = "100px";
        }

        if (!string.IsNullOrEmpty(Width))
        {
            providerContainer.Style["Width"] = Width;
        }

        ProviderFactory.Providers.ForEach(p =>
            this.FindControl("providerContainer").Controls.Add(constructControl(p))
            );
    }

    private HtmlGenericControl constructControl(Provider provider)
    {

        string iconPath = Utility.GetSocialAuthConfiguration().IconFolder.Path + provider.ProviderType.ToString() + ".png";
        bool isconnected = SocialAuthUser.IsConnectedWith(provider.ProviderType);
        bool iscurrent = (SocialAuthUser.CurrentConnection) != null ? (SocialAuthUser.CurrentConnection.ProviderType == provider.ProviderType) : false;
        HtmlGenericControl providerDiv = new HtmlGenericControl("div");
        providerDiv.Attributes.Add("class", "provider");
        ImageButton imgB = new ImageButton()
        {
            ID = "img" + provider.ProviderType,
            CommandArgument = provider.ProviderType.ToString(),
            ImageUrl = iconPath
        };
        if (isconnected)
        {
            HtmlGenericControl tickspan = new HtmlGenericControl("span");
            tickspan.InnerHtml = "<img src='images/socialauthicons/" + (iscurrent ? "currentyes" : "yes") + ".png' style='top:0px;left:0px;z-index:100'/>";
            tickspan.Style.Add("position", "absolute");
            
            providerDiv.Controls.Add(tickspan);
        }
        //if (iscurrent)
        //    imgB.Style.Add("border", "1px solid yellow");
        imgB.Command += new CommandEventHandler(imgB_Command);
        providerDiv.Controls.Add(imgB);
        return providerDiv;
    }

    private void imgB_Command(object sender, EventArgs e)
    {
        SocialAuthUser.Connect(
            (PROVIDER_TYPE)Enum.Parse(typeof(PROVIDER_TYPE), ((CommandEventArgs)e).CommandArgument.ToString()), DefaultURL);

    }
}