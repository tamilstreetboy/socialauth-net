<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SocialAuthLogin.ascx.cs" Inherits="SocialAuthLogin"
    %>
<style type="text/css">
    .provider
    {
        float: left;
        padding-top: 5px;
        padding-left: 20px;
        padding-bottom: 5px;
    }
</style>
<div style="height: 80px; background-color: purple;  border: 10px solid orange;
    font-family: verdana; font-size: small; font-weight: bold;width:300px">
    <span style="color: #ffffff">Select a provider:</span><br />
    <div id='providerContainer' style='float: left;vertical-align:middle' runat="server" >
    </div>
</div>
