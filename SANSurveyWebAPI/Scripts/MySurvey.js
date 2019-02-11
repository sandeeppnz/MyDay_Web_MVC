function LoaderLocal() {
        $.blockUI({
            css: {
                border: 'none',
                backgroundColor: 'transparent',
                opacity: .9,
                baseZ: 10001,
            },
            message: '<img src="/Images/comment.svg" />'
        });
    }