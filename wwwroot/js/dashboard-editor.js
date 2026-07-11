(function () {
    if (!window.tinymce) {
        return;
    }

    tinymce.init({
        selector: ".js-rte",
        height: 360,
        menubar: false,
        branding: false,
        promotion: false,
        plugins: "lists link image table code autoresize wordcount",
        toolbar: "undo redo | bold italic underline strikethrough | forecolor | bullist numlist | link image table | removeformat code",
        content_style: "body{font-family:'Open Sans','Segoe UI',Arial,sans-serif;font-size:16px;line-height:1.8;color:#e5edf7;background:#172234;} img{max-width:100%;height:auto;} p{margin:0 0 1rem;}",
        skin: "oxide-dark",
        content_css: "dark",
        automatic_uploads: false,
        convert_urls: false,
        valid_elements: "p,br,strong/b,em/i,u,s,span[style],a[href|target|rel],ul,ol,li,blockquote,code,pre,img[src|alt|width|height],table,thead,tbody,tr,td,th",
        extended_valid_elements: "span[class|style]",
        setup: editor => {
            editor.on("change keyup", () => editor.save());
        }
    });
})();
