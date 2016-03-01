<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebGuiTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    
    <form id="form1" runat="server">
    <div>
        <asp:HyperLink ID="lnkAbout" NavigateUrl="~/About.aspx" runat="server">About the instance</asp:HyperLink> 
    </div>
    <asp:TextBox ID="txtQuerySearch" runat="server" Width="574px" Height="89px"></asp:TextBox>
    <asp:Button ID="btnSearch" runat="server" onclick="btnSearch_Click" 
        Text="Search" Height="89px" Width="138px" />
    
    </form>
</body>
</html>
