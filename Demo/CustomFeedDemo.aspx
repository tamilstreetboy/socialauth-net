<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomFeedDemo.aspx.cs"
    Inherits="Brickred.SocialAuth.NET.Demo.CustomFeedDemo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        div
        {
            font-family: Tahoma;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div style="width:100%;text-align:right"> <a href="Welcome.aspx">Back</a></div>
        <asp:Button ID="btnCustomFeed" runat="server" OnClick="btnCustomFeed_Click" Text="Execute Custom Feed" /><br />
        <asp:Label ID="lblJson" runat="server" /><br />
        <asp:Label ID="lblAlbum" runat="server" />
    </div>
    </form>
</body>
</html>
