const js = jQuery.noConflict(true);

js.fn.dataTableExt.afnFiltering.push(
    function( settings, data, dataIndex ) {
        const min = js('#min').val();
        const max = js('#max').val();
        const date = data[0].split(" ")[0].replaceAll(".", "-").split("-").reverse().join("-")

        if (date >= min && date <= max || min === "" || max === "") return true;
        return false;
    }
);

js(document).ready(function() {
    const table = js('#tbllist').DataTable();

    js('#min').change( function() { table.draw(); } );
    js('#max').change( function() { table.draw(); } );
});