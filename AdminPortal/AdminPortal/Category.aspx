<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Category.aspx.cs" Inherits="AdminPortal.Category" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">

<!-- Container-fluid starts-->
<div class="container-fluid">
    <div class="row">
        <div class="col-sm-12">
            <div class="card">
                <div class="card-header">
                    <div class="form-inline search-form search-box">
                        <div class="form-group">
                            <asp:TextBox id="txtSearch" runat="server" placeholder="Search.." CssClass="form-control-plaintext"></asp:TextBox>
                        </div>
                    </div>

                    <asp:Button ID="btnAddCategory" runat="server" CssClass="btn btn-primary add-row mt-md-0 mt-2" Text="Add Category" />
                </div>

                <div class="card-body">
                    <div class="table-responsive table-desi">
                        <table class="table all-package table-category " id="editableTable">
                            <thead>
                                <tr>
                                    <th>Image</th>
                                    <th>Name</th>
                                    <th>Price</th>
                                    <th>Status</th>
                                    <th>Category</th>
                                    <th>Option</th>
                                </tr>
                            </thead>

                            <tbody>
                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/1.jpg"
                                            data-field="image" alt="">
                                    </td>

                                    <td data-field="name">Headphones</td>

                                    <td data-field="price">$20.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/2.jpg" alt="">
                                    </td>

                                    <td data-field="name">Honor Mobile</td>

                                    <td data-field="price">$462.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/3.jpg" alt="">
                                    </td>

                                    <td data-field="name">Samsung LED TV</td>

                                    <td data-field="price">$652.00</td>

                                    <td class="order-warning" data-field="status">
                                        <span>Padding</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/4.jpg" alt="">
                                    </td>

                                    <td data-field="name">Motorola Bluetooth</td>

                                    <td data-field="price">$25.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/5.jpg" alt="">
                                    </td>

                                    <td data-field="name">Apple 6s</td>

                                    <td data-field="price">$782.00</td>

                                    <td class="order-cancle" data-field="status">
                                        <span>Cancle</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/6.jpg" alt="">
                                    </td>

                                    <td data-field="name">Printer</td>

                                    <td data-field="price">$2461.00</td>

                                    <td class="order-warning" data-field="status">
                                        <span>Padding</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/7.jpg" alt="">
                                    </td>

                                    <td data-field="name">High Quality Headphones</td>

                                    <td data-field="price">$761.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/8.jpg" alt="">
                                    </td>

                                    <td data-field="name">Home Theater Speakers</td>

                                    <td data-field="price">$672.00</td>

                                    <td class="order-cancle" data-field="status">
                                        <span>Cancle</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/9.jpg" alt="">
                                    </td>

                                    <td data-field="name">Diamond Ring</td>

                                    <td data-field="price">$237.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/11.jpg" alt="">
                                    </td>

                                    <td data-field="name">Diamond Nacklace</td>

                                    <td data-field="price">$3579.00</td>

                                    <td class="order-warning" data-field="status">
                                        <span>Padding</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/12.jpg" alt="">
                                    </td>

                                    <td data-field="name">Diamond Earrings</td>

                                    <td data-field="price">$3145.00</td>

                                    <td class="order-warning" data-field="status">
                                        <span>Padding</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/13.jpg" alt="">
                                    </td>

                                    <td data-field="name">Night lamp</td>

                                    <td data-field="price">$84.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/14.jpg" alt="">
                                    </td>

                                    <td data-field="name">Men's Shoes</td>

                                    <td data-field="price">$67.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/15.jpg" alt="">
                                    </td>

                                    <td data-field="name">Ledi's red top</td>

                                    <td data-field="price">$234.00</td>

                                    <td class="order-warning" data-field="status">
                                        <span>Padding</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/16.jpg" alt="">
                                    </td>

                                    <td data-field="name">latest ledis shoes</td>

                                    <td data-field="price">$357.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/17.jpg" alt="">
                                    </td>

                                    <td data-field="name">Woman one pis</td>

                                    <td data-field="price">$682.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/18.jpg" alt="">
                                    </td>

                                    <td data-field="name">Mouse</td>

                                    <td data-field="price">$24.00</td>

                                    <td class="order-cancle" data-field="status">
                                        <span>Cancle</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/19.jpg" alt="">
                                    </td>

                                    <td data-field="name">Coffee maker</td>

                                    <td data-field="price">$9721.00</td>

                                    <td class="order-success" data-field="status">
                                        <span>Success</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <img src="assets/images/dashboard/product/20.jpg" alt="">
                                    </td>

                                    <td data-field="name">Diamond Nacklace</td>

                                    <td data-field="price">$3579.00</td>

                                    <td class="order-cancle" data-field="status">
                                        <span>Cancle</span>
                                    </td>

                                    <td data-field="name">Electronics</td>

                                    <td>
                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-edit" title="Edit"></i>
                                        </a>

                                        <a href="/javascript:void(0)">
                                            <i class="fa fa-trash" title="Delete"></i>
                                        </a>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- Container-fluid Ends-->

</asp:Content>
