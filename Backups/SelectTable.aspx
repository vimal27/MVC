<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SelectTable.aspx.cs" Inherits="FL_SelectTable" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="txt_Query" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox>
            </br><asp:Button ID="btn_Search" OnClick="btn_Search_Click" runat="server" Text="Select"/>
            </br><asp:Button ID="btn_Insert" OnClick="btn_Insert_Click" runat="server"  Text="InsertOrUpdate"/>
            </br><asp:Button ID="btn_Download" OnClick="btn_Download_Click" runat="server" Text="Download" />
        </div>
        <div>
            <asp:GridView ID="grd_Data" runat="server">
            </asp:GridView>
        </div>
    </form>
</body>
</html>
