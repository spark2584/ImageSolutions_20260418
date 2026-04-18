<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="ImageSolutionsWebsite.Template.back_end.assets.Test" %>

<!DOCTYPE html>

<head runat="server">

    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description"
        content="Multikart admin is super flexible, powerful, clean &amp; modern responsive bootstrap 4 admin template with unlimited possibilities.">
    <meta name="keywords"
        content="admin template, Multikart admin template, dashboard template, flat admin template, responsive admin template, web app">
    <meta name="author" content="pixelstrap">
    <link rel="icon" href="assets/images/dashboard/favicon.png" type="image/x-icon">
    <link rel="shortcut icon" href="assets/images/dashboard/favicon.png" type="image/x-icon">
    <title>Multikart - Premium Admin Template</title>

    <!-- Google font-->
    <link rel="stylesheet"
        href="https://fonts.googleapis.com/css2?family=Work+Sans:ital,wght@0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,500;1,600;1,700;1,800;1,900&display=swap">

    <link rel="stylesheet"
        href="https://fonts.googleapis.com/css2?family=Nunito:ital,wght@0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap">


    <!-- Font Awesome-->
    <link rel="stylesheet" type="text/css" href="assets/css/vendors/font-awesome.css">

    <!-- Flag icon-->
    <link rel="stylesheet" type="text/css" href="assets/css/vendors/flag-icon.css">

    <!-- Bootstrap css-->
    <link rel="stylesheet" type="text/css" href="assets/css/vendors/bootstrap.css">

    <!-- App css-->
    <link rel="stylesheet" type="text/css" href="assets/css/style.css">
</head>
<body>
    <form id="form1" runat="server">
        <div class="page-wrapper">
        <div class="page-body-wrapper">
            <div class="page-body">
                <!-- Container-fluid starts-->
                <div class="container-fluid">
                    <div class="page-header">
                        <div class="row">
                            <div class="col-lg-6">
                                <div class="page-header-left">
                                    <h3>User List
                                        <small>Multikart Admin panel</small>
                                    </h3>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <ol class="breadcrumb pull-right">
                                    <li class="breadcrumb-item">
                                        <a href="index.html">
                                            <i data-feather="home"></i>
                                        </a>
                                    </li>
                                    <li class="breadcrumb-item">Users</li>
                                    <li class="breadcrumb-item active">User List</li>
                                </ol>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Container-fluid Ends-->
                <!-- Container-fluid starts-->
                <div class="container-fluid">
                    <div class="card">
                        <div class="card-header">
                            <form class="form-inline search-form search-box">
                                <div class="form-group">
                                    <input class="form-control-plaintext" type="search" placeholder="Search.."><span
                                        class="d-sm-none mobile-search"><i data-feather="search"></i></span>
                                </div>
                            </form>

                            <a href="create-user.html" class="btn btn-primary mt-md-0 mt-2">Create User</a>
                        </div>

                        <div class="card-body">
                            <div class="table-responsive table-desi">
                                <table class="all-package coupon-table table table-striped">
                                    <thead>
                                        <tr>
                                            <th>
                                                <button type="button"
                                                    class="btn btn-primary add-row delete_all">Delete</button>
                                            </th>
                                            <th>Avtar</th>
                                            <th>First Name</th>
                                            <th>Last Name</th>
                                            <th>Email</th>
                                            <th>Last Login</th>
                                            <th>Role</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        <tr data-row-id="1">
                                            <td>
                                                <input class="checkbox_animated check-it" type="checkbox" value=""
                                                    id="flexCheckDefault" data-id="1">
                                            </td>

                                            <td>
                                                <img src="assets/images/dashboard/user.jpg" alt="">
                                            </td>

                                            <td>Rowan</td>

                                            <td>Torres</td>

                                            <td>Rowan.torres@gmail.com</td>

                                            <td>6 Days ago</td>

                                            <td>Customer</td>
                                        </tr>

                                        <tr data-row-id="2">
                                            <td>
                                                <input class="checkbox_animated check-it" type="checkbox" value=""
                                                    id="flexCheckDefault1" data-id="2">
                                            </td>

                                            <td>
                                                <img src="assets/images/dashboard/user1.jpg" alt="">
                                            </td>

                                            <td>Alonzo</td>

                                            <td>Perez</td>

                                            <td>Perez.Alonzo@gmail.com</td>

                                            <td>2 Days ago</td>

                                            <td>Customer</td>
                                        </tr>
                                        </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Container-fluid Ends-->
            </div>
        </div>
    </div>
    </form>

        <!-- latest jquery-->
    <script src="assets/js/jquery-3.3.1.min.js"></script>

    <!-- Bootstrap js-->
    <script src="assets/js/bootstrap.bundle.min.js"></script>

    <!-- feather icon js-->
    <script src="assets/js/icons/feather-icon/feather.min.js"></script>
    <script src="assets/js/icons/feather-icon/feather-icon.js"></script>

    <!-- Sidebar jquery-->
    <script src="assets/js/sidebar-menu.js"></script>

    <!-- Table Row Delete js -->
    <script src="assets/js/table-row-delete.js"></script>

    <!--Customizer admin-->
    <script src="assets/js/admin-customizer.js"></script>

    <!-- lazyload js-->
    <script src="assets/js/lazysizes.min.js"></script>

    <!--right sidebar js-->
    <script src="assets/js/chat-menu.js"></script>

    <!--script admin-->
    <script src="assets/js/admin-script.js"></script>
</body>
</html>
