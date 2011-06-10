<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManualLogin.aspx.cs" Inherits="Brickred.SocialAuth.NET.Demo.ManualLogin" %>

<%@ Register src="SocialAuthLogin.ascx" tagname="SocialAuthLogin" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function Login(providerName) {
            window.location.href = 'login.sauth?p=' + providerName
        }

    </script>
    <style type="text/css">
    TD
    {
        background-color:Purple;
        color:White;
        width:60px;
        text-align:center;
    }
    
    TD:Hover
    {
        background-color:Yellow;
        color:black;
        cursor:hand;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>

    <!----------------------------------------------------------------
     To use this page for login instead of automatically generated UI,
    change LoginUrl from blank to "ManualLogin.aspx" in Web.Config!
     <Authentication Enabled="true" LoginUrl="ManualLogin.aspx" DefaultUrl="Welcome.aspx" />
    !---------------------------------------------------------------->

    <h3>ASP.NET controls</h3>
        <asp:Button ID="btnFacebook" runat="server" onclick="btn_Click" Text="Facebook" />
        <asp:Button ID="btnGoogle" runat="server" onclick="btn_Click" Text="Google" />
        <asp:Button ID="btnYahoo" runat="server" onclick="btn_Click" Text="Yahoo" />
        <asp:Button ID="btnMSN" runat="server" onclick="btn_Click" Text="MSN" />
        <br /><br />
        <h3>Simple HTML Controls</h3>
        <table border=0 style="background:orange; font-size:12px; font-family:Verdana;" cellpadding=5 cellspacing=2>
            <tr>
                  <td onclick="Login('facebook')">Facebook</td>
                  <td onclick="Login('google')">Google</td>
                  <td onclick="Login('yahoo')">Yahoo</td>
                  <td onclick="Login('msn')">MSN</td>
            </tr>
        </table>
        <br /><br />
        <h3>SocialAuth.NET User Control</h3>
    </div>
    <uc1:SocialAuthLogin ID="SocialAuthLogin1" runat="server" />
    </form>
</body>
</html>
