<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebGuiTest.About" %>

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
    <div>
    
        <asp:Label ID="lblFiles" runat="server" Text="Label"></asp:Label> </br>
        <asp:Label ID="lblWords" runat="server" Text="Label"></asp:Label> </br>
        <asp:Label ID="lblMemory" runat="server" Text="Label"></asp:Label> </br>
    
    </div>
    </form>
</body>
</html>
