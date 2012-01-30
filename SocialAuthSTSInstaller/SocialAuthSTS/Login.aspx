<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<%@ Register Src="SocialAuthLogin.ascx" TagName="SocialAuthLogin" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Login to the security token service (STS)</title>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 125px;
        }
        .style3
        {
            font-family: verdana;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <center>
        <div class="style3">
            <img src="images/logo.png" alt="BrickRed Technologies Pvt. Ltd." />
            <br>
            SECURED TOKEN SERVICE
            <br />
            <br />
            <br />
            <div style="width: 500px; background-color: #FFaa20">
                <asp:Label ID="Label3" runat="server" Text="Select any provider to login"></asp:Label>
                &nbsp;<br />
                <uc1:SocialAuthLogin ID="SocialAuthLogin1" runat="server" />
            </div>
            <p>
                <br />
                <br />
                <p>
                    <b>Powered by:</b></p>
            <img src="images/WIFlogo.png" />
            &nbsp; &nbsp;
            <img src="images/socialauth.png" />
    </center>
    </form>
</body>
</html>
