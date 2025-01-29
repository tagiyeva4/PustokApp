$(document).ready(function () {
    $("#Photo").change(function (ev) {
        let file = ev.target.files[0];
        var uploadimg = new FileReader();
        uploadimg.onload = function (displayimg) {
            $("#ImgPreview").attr('src', displayimg.target.result);
            $("#ImgPreview").show();
        }
        uploadimg.readAsDataURL(file);
    })
})
