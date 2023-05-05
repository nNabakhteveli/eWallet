const js = jQuery.noConflict(true);

js(document).ready(() => {
    const table = js('#tbllist').DataTable({
        columns: [
            {data: 'createDate'},
            {data: 'status'},
            {data: 'paymentType'},
            {data: 'currency'},
            {data: 'amount'}
        ]
    });

    $('.filter-btn').on('click', () => {
        $.ajax({
            type: 'POST',
            url: '/TransactionsTable/FilterInRange',
            data: {StartDate: js('#min').val(), EndDate: js('#max').val()},
            success: data => {
                const arr = [];

                for (const i of data.data) {
                    arr.push({
                        createDate: i.createDate || " ",
                        status: i.status || " ",
                        paymentType: i.paymentType || " ",
                        currency: i.currency || " ",
                        amount: i.amount || " "
                    });
                }

                table.clear();
                table.rows.add(arr);
                table.draw();
            }
        });
    });

    js('#min').change(function () {
        table.draw();
    });
    js('#max').change(function () {
        table.draw();
    });
});