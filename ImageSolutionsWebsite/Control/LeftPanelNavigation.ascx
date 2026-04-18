<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeftPanelNavigation.ascx.cs" Inherits="ImageSolutionsWebsite.Control.LeftPanelNavigation" %>

    <!-- side-bar colleps block stat -->
    <div class="collection-filter-block">
        <div class="collection-collapse-block open">
            <div class="collection-collapse-block-content">
                <div class="collection-brand-filter">
                    <br />
                    <button class="btn btn-solid" style="width:100%" onclick="window.history.go(-1); return false;">« Back</button>
                </div>
            </div>
        </div>
        <div class="collection-collapse-block open">

            <h3 class="collapse-block-title">Shop By Category</h3>
            <div class="collection-collapse-block-content">
                <div class="collection-brand-filter">
                    <asp:Literal ID="litWebsiteTab" runat="server"></asp:Literal>
                </div>
            </div>

             <%--<h3 class="collapse-block-title">brand</h3>
             <div class="collection-collapse-block-content">
                 <div class="collection-brand-filter">
                     <div class="form-check collection-filter-checkbox">
                         <label class="form-check-label" for="zara">&#183; zara</label>
                     </div>
                     <div class="form-check collection-filter-checkbox" style="margin-left:20px;">
                         <label class="form-check-label" for="vera-moda">&#183; vera-moda</label>
                     </div>
                     <div class="form-check collection-filter-checkbox" style="margin-left:40px;">
                         <label class="form-check-label" for="forever-21" style="color: blue;">&#183; forever-21</label>
                     </div>
                     <div class="form-check collection-filter-checkbox">
                         <label class="form-check-label" for="roadster">&#183; roadster</label>
                     </div>
                     <div class="form-check collection-filter-checkbox">
                         <label class="form-check-label" for="only">&#183; only</label>
                     </div>
                 </div>
             </div>--%>
         </div>
    </div>
    <!-- silde-bar colleps block end here -->

