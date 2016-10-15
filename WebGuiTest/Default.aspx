<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebGuiTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <script type="text/javascript">
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', 'https://www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-83697549-1', 'auto');
        ga('send', 'pageview');

    </script>
    <form id="form1" runat="server">

    <asp:TextBox ID="txtQuerySearch" runat="server" Width="574px" Height="89px"></asp:TextBox>
    <asp:Button ID="btnSearch" runat="server" onclick="btnSearch_Click" 
        Text="Search" Height="89px" Width="138px" />
        <div>
            <asp:HyperLink ID="lnkAbout" NavigateUrl="~/About.aspx" runat="server">About the instance</asp:HyperLink> 
        </div>
        <div id='divFeedback' runat="server">
            <asp:RadioButtonList ID="RadioButtonFeedback" runat="server" RepeatDirection="Vertical">
                <asp:ListItem Text="Os primeiros resultados eram exatamente o que eu estava procurando" Value="1"></asp:ListItem>
                <asp:ListItem Text="Os primeiros resultados eram sobre o que eu estava procurando, mas, a ordem pode melhorar." Value="2"></asp:ListItem>
                <asp:ListItem Text="Os primeiros resultados citavam o que eu estava procurando" Value="3"></asp:ListItem>
                <asp:ListItem Text="Os resultados não eram sobre o que eu procurava" Value="4"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:Label Text="Descreva melhor seu feedback:" runat="server" /><br />
            <asp:TextBox ID="txtFeedback" runat="server" Width="574px" Height="89px"></asp:TextBox>
            <asp:Button ID="btnGravarFeedback" runat="server" Text="Send Feedback" 
                Height="89px" Width="138px" onclick="btnGravarFeedback_Click" />

        </div>
    </form>
</body>
</html>
