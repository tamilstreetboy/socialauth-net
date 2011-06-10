using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Demo
{
    public partial class ManualLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            PROVIDER_TYPE selectedProvider = (PROVIDER_TYPE)Enum.Parse(typeof(PROVIDER_TYPE), e.ToString().ToUpper());
            SocialAuthUser objUser = new SocialAuthUser(selectedProvider);
        }

    }
}