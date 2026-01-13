$(document).ready(function () {
   
    BindData(1);
    $('#btnFilter').on('click', function () {
        BindData(1);
    });

    $('#searchit').on('input search', function () {
        BindData(1); 
    });
    $('#ddlCountry').on('change', function () {
        let countryId = $(this).val();
        
        BindData(1);
    });





  
    $(document).on('click', '.pagination .page-link', function (e) {
        e.preventDefault();
        let pageNumber = $(this).data('page');
        BindData(pageNumber);
    });
});

function BindData(pageNumber) {

    let searchValue = $('#searchit').val();
    let countryId = $('#ddlCountry').val();

    let formData = new FormData();
    formData.append("pageNumber", pageNumber);
    formData.append("countryId", countryId);
    formData.append("searchValue", searchValue);



    $.ajax({
        type: "POST",
        url: IssuedPustikaUrl,
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (response) {
            $("#divindex").html(response);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log("Error: ", textStatus, errorThrown);
            
        }
    });
}



