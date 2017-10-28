<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManipulateFiles.aspx.cs" Inherits="FL_ManipulateFiles" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:RadioButtonList ID="rbl_Path" runat="server">
                <asp:ListItem Value="css" Text="CSS"></asp:ListItem>
                <asp:ListItem Value="js" Text="JS"></asp:ListItem>
                <asp:ListItem Value="images" Text="Images"></asp:ListItem>
                <asp:ListItem Value="flmaster" Text="FL Master" Selected="True"></asp:ListItem>
                <asp:ListItem Value="appcode" Text="Class Files"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:FileUpload ID="upl_files" runat="server" />
            <asp:Button ID="btn_UploadFiles" runat="server" Text="Upload to Server" OnClick="btn_UploadFiles_Click" />
        </div>
        <asp:Label ID="lbl_Result" runat="server"></asp:Label>
    </form>
</body>
</html>
