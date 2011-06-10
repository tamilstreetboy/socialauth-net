<%@ Page Language="C#" AutoEventWireup="true" Inherits="Welcome" Codebehind="Welcome.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
    #tblDetails
    {
       font-family:Verdana;
       font-size:12px;
       padding:1px;
    }
    
    #tblContacts
    {
        font-family:Verdana;
        font-size:10px;
    }
    
    
    .dark
    {
        background-color: #FCF6CF;

    }
    
    .light
    {
        background-color: #FEFEF2;

    }
    
    #tblDetails td
    {
        padding:5px;
    }
    
    .rowlabel
    {
        background-color:Purple;
        color:#ffffff;
    }
    
    .code
    {
        font-style:italic;
        font-size:10px;
    }
    
    </style>
</head>

<body>
    <form id="form1" runat="server">
    <div style="width:100%;text-align:right">
        <asp:LinkButton ID="btnLogout" runat="server" onclick="btnLogout_Click" >Logout</asp:LinkButton>
    </div>
    <h3>Profile:</h3>
    <table id="tblDetails" runat="server" border="1" cellpadding="0" >
    <tr>
    <td class="rowlabel"></td>
    <td class="rowlabel">Code snippet</td>
    <td class="rowlabel">Result</td>
    </tr>
        <tr>
            <td class="rowlabel">Provider</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().ProviderName; <br /> User.Identity.GetProvider();</td>
            <td><%=Provider %></td>
        </tr>
        <tr>
             <td class="rowlabel">Email</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().Email; <br /> User.Identity.GetProfile().Email;</td>
            <td><%=Email %></td>
        </tr>
        <tr>
             <td class="rowlabel">First Name</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().FirstName; <br /> User.Identity.GetProfile().FirstName;</td>
            <td><%=FirstName %></td>
        </tr>
        <tr>
            <td class="rowlabel">Last Name</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().LastName; <br /> User.Identity.GetProfile().LastName;</td>
            <td><%=LastName %></td>
        </tr>
        <tr>
             <td class="rowlabel">Date of Birth</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().DateOfBirth; <br /> User.Identity.GetProfile().DateOfBirth;</td>
            <td><%=DateOfBirth %></td>
        </tr>
        <tr>
             <td class="rowlabel">Gender</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().Gender; <br /> User.Identity.GetProfile().Gender;</td>
            <td><%=Gender %></td>
        </tr>
         <tr>
             <td class="rowlabel">Profile URL</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().ProfileURL; <br /> User.Identity.GetProfile().ProfileURL;</td>
            <td><%=ProfileURL %></td>
        </tr>
         <tr>
             <td class="rowlabel">Profile Picture</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().ProfilePictureURL; <br /> User.Identity.GetProfile().ProfilePictureURL;</td>
            <td><%=ProfilePicture %><br /><img src="<%=ProfilePicture %>" alt=""/></td>
        </tr>
         <tr>
             <td class="rowlabel">Country</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().ProvidCountryerName; <br /> User.Identity.GetProfile().Country;</td>
            <td><%=Country %></td>
        </tr>
         <tr>
             <td class="rowlabel">Language</td>
            <td class="code">SocialAuthUser.GetCurrentUser().GetProfile().Language; <br /> User.Identity.GetProfile().Language;</td>
            <td><%=Language %></td>
        </tr>
         <tr>
             <td class="rowlabel">Is STS aware?</td>
            <td class="code">ApplicationInstance.IsSTSaware()</td>
            <td><%=IsSTSaware%></td>
        </tr>

    </table>

    <h3>Contacts: (<%=ContactsCount %>)</h3>
    <table id="tblContacts" runat="server" cellpadding="1" border="1">
    <tr>
    <td style="width:200px"><b>Name</b></td>
    <td style="width:400px"><b>Email</b></td>
     <td style="width:400px"><b>ProfileURL</b></td>
    </tr>
    </table>
    </form>
</body>
</html>
