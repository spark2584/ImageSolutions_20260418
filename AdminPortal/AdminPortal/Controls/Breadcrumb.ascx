<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Breadcrumb.ascx.cs" Inherits="AdminPortal.Controls.Breadcrumb" %>
<!-- Container-fluid starts-->
<div class="container-fluid">
    <div class="page-header">
        <div class="row">
            <div class="col-lg-6">
                <div class="page-header-left">
                    <h3>Category<asp:Label ID="lblTitle" runat="server"></asp:Label>
                        <small>Multikart Admin panel</small>
                    </h3>
                </div>
            </div>
            <div class="col-lg-6">
                <ol class="breadcrumb pull-right">
                    <li class="breadcrumb-item">
                        <a href="/index.html">
                            <i data-feather="home"></i>
                        </a>
                    </li>
                    <li class="breadcrumb-item">Physical</li>
                    <li class="breadcrumb-item active">Category</li>
                </ol>
            </div>
        </div>
    </div>
</div>
<!-- Container-fluid Ends-->