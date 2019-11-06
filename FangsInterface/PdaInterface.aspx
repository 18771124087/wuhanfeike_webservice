<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PdaInterface.aspx.cs" Inherits="FangsInterface.PdaInterface" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <div align="center">
        <div align="center" style="width: 1000px">
            <h1>PDA密码重置页面</h1>
            <form id="form1" runat="server" style="background: #eee; padding: 100px">
                <p>
                    <asp:Label ID="lbusername" runat="server">用户ID：</asp:Label> 
                    <asp:TextBox ID="tbusername" runat="server" Style="width: 250px;padding:5px"></asp:TextBox>
                </p>

                <p>
                    <asp:Label ID="Label1" runat="server" >新密码：</asp:Label> 
                    <asp:TextBox ID="tbpsw" runat="server" TextMode="Password" Style="width: 250px;padding:5px"></asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnLogin" runat="server" Text="开始重置" OnClick="btnLogin_Click"
                        Style="background: #0094ff; width: 260px;color:#ffffff;padding:5px" />
                </p>
            </form>
        </div>
    </div>
</body>
</html>
