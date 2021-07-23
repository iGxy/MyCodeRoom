<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UniEmr_web.aspx.cs" Inherits="UniEmr_CSpic.UniEmr_web" %>

<!DOCTYPE html>

<style type="text/css">
    .Right_color {
        float: right;
        border: 1px solid black;
        background-color:aquamarine;
        font-weight:600;
        font-size:medium
    }
    .Right{
        float:right;
        font-size:20px;
        padding-top:2px
    }
</style>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>
        
    </title>
</head>

<body style="text-align: center;background-color:black">
    <form id="form1" runat="server">
     <div>
        <asp:Label ID="Label1" runat="server" Text="共张" style="text-align: center;color:white;width:80px" class="Right"/>        
        <%--<asp:Button ID="Button1" runat="server" Text="下一张" class="Right_color" OnClick="Button1_Click"/>  
        <asp:Button ID="Button2" runat="server" Text="上一张" class="Right_color" OnClick="Button2_Click"/>      
        <asp:Label ID="Label2" runat="server" Text="第1张" style="text-align: center;color:white;width:80px" class="Right"/> --%>
     </div>
     <br />
    <div ID="div_image" style="text-align: center;margin-top:8px" runat="server">
        <%--<asp:Image ID="Image1" runat="server"/>--%>
    </div>    
        
           
    </form>
</body>
</html>
