<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportDataFetch.aspx.cs" Inherits="FL_SourceCodeViewer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="AuthenticationBlock" runat="server" visible="false">
            <asp:Label ID="lbl_Pass" runat="server" Text="Enter Password: "></asp:Label>
            <asp:TextBox ID="txt_Password" TextMode="Password" runat="server"></asp:TextBox>
            <asp:Button ID="btn_UnlockCode" runat="server" Text="Unlock Code" OnClick="UnlockCode"></asp:Button>
        </div>
        <div id="ProtectedBlock" runat="server" visible="false">
            <asp:TextBox ID="txt_FilePath" runat="server" TextMode="MultiLine" Width="300px" Height="200px"></asp:TextBox></br>
            <asp:Button ID="btn_ViewCode" runat="server" Text="View Code" OnClick="ViewCode" />
            <asp:Button ID="btn_CompressAndDownload" runat="server" Text="Compress & Download" OnClick="CompressAndDownload" />
            <asp:Button ID="btn_LockCode" runat="server" Text="Lock Code" OnClick="LockCode" />
        </div>
        <div id="ResultBlock" runat="server">
        </div>
    </form>
</body>
</html>
