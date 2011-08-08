using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using Newtonsoft.Json.Linq;

namespace Brickred.SocialAuth.NET.Demo
{
    public partial class CustomFeedDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCustomFeed_Click(object sender, EventArgs e)
        {
            if (SocialAuthUser.IsLoggedIn())
            {
           
                string albumJson = SocialAuthUser.GetCurrentUser().ExecuteFeed("https://graph.facebook.com/me/albums");
           JObject jsonObject =  JObject.Parse(albumJson);   
                List<Album> albums = new List<Album>();

                jsonObject["data"].Children().ToList().ForEach(x =>
                {
                    albums.Add(new Album()
                    {
                        ID = (string)x["id"],
                        PhotoCount = (int)x["count"],
                        Name = (string)x["name"],
                        Location = (string)x["location"],
                        CoverPhoto = (string)x["cover_photo"]

                    });

                });

                foreach (var item in albums)
                {
                    Label lbl = new Label();
                    lbl.Text = "<h3>" + item.Name + "</h3>(" + item.PhotoCount + ") : " + "<img src='https://graph.facebook.com/" + item.CoverPhoto + "/picture?type=album&access_token=" + SocialAuthUser.GetCurrentUser().AccessToken + "'>";
                    lbl.CssClass = "album";
                    lblAlbum.Controls.Add(lbl);
                   
                }
               
                lblJson.Text = albumJson;
            }
        }
    }
}