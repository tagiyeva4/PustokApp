$(document).ready(function () {
    $("#Photo").change(function (ev) {
        let file = ev.target.files[0];
        var uploadimg = new FileReader();
        uploadimg.onload = function (displayimg) {
            $("#ImgPreview").attr('src', displayimg.target.result);
            $("#ImgPreview").show();
        }
        uploadimg.readAsDataURL(file);
    });

    $("#Photos").change(function (ev) {
        let files = ev.target.files;
        for (var i = 0; i < files.length; i++) {
            var uploadimg = new FileReader();
            let img = $("<img>");
            img.css({ width: "120", height: "120" });
            uploadimg.onload = function (displayimg) {
                img.attr('src', displayimg.target.result)
            }
            uploadimg.readAsDataURL(files[i]);
            $(".bookImagesPreview").append(img);
        }
    });

    $(".deleteGenreButton").click(function (ev) {
        ev.preventDefault();
        let url = $(this).attr("href");
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url)
                    .then(response => {
                        if (response.ok) {
                            Swal.fire({
                                title: "Deleted!",
                                text: "Your file has been deleted.",
                                icon: "success"
                            });
                            window.location.reload();
                        }
                        else {
                            Swal.fire({
                                icon: "error",
                                title: "Oops...",
                                text: "Something went wrong!",
                                footer: '<a href="#">Why do I have this issue?</a>'
                            });
                        }
                    })
               
            }
        });


        
    })


})
