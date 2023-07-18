$(function() {
    $.fn.dataTable.moment("DD-MMM-YYYY");
    $.fn.dataTable.moment("DD-MMM-YYYY HH:mm");
    $.fn.dataTable.moment("DD-MMM-YYYY HH:mm:ss");

    /* Set default values for the datatables framework. */
    $.extend(true,
        $.fn.dataTable.defaults,
        {
            stateSave: true,
            dom: "lBtpi",
            autoWidth: true,
            lengthMenu: [10, 25, 50, 100],
            pageLength: 10,
            fixedHeader: {
                header: false,
                headerOffset: $("#header-bar").outerHeight()
            },
            buttons: [
                {
                    extend: "collection",
                    text: '<i class="fa fa-list-ul" >',
                    titleAttr: "Grid Features",
                    attr: {
                        id: 'gridMenuButton'
                    },
                    buttons: [
                        {
                            extend: "excelHtml5",
                            text: '<i class="fa fa-file-excel-o" >',
                            titleAttr: "Excel",
                            attr: {
                                id: 'gridExcelButton'
                            },
                            exportOptions: {
                                columns: ":visible:not(.not-export-col)"
                            }
                        },
                        {
                            extend: "csvHtml5",
                            text: '<i class="fa fa-file-text-o"></i>',
                            titleAttr: "CSV",
                            attr: {
                                id: 'gridCsvButton'
                            },
                            exportOptions: {
                                columns: ":visible:not(.not-export-col)"
                            }
                        },
                        {
                            extend: "pdfHtml5",
                            orientation: "landscape",
                            pageSize: "LEGAL",
                            text: '<i class="fa fa-file-pdf-o"></i>',
                            titleAttr: "PDF",
                            attr: {
                                id: 'gridPdfButton'
                            },
                            exportOptions: {
                                columns: ":visible:not(.not-export-col)"
                            }
                        },
                        {
                            extend: "print",
                            text: '<i class="fa fa-print"></i>',
                            titleAttr: "Print",
                            attr: {
                                id: 'gridPrintButton'
                            },
                            exportOptions: {
                                columns: ":visible:not(.not-export-col)"
                            }
                        },
                        {
                            extend: "columnsToggle",
                            text: "<div></div>"
                        }
                    ]
                }
            ],
            scrollX: true,
            initComplete: function() {
                var that = this;
                var gridName = $(that).attr("id");
                var removeExport = $(that).data("remove-export") === true;

                /* Add 'Export' and 'Visibility' labels in the drop down menu upon first click. */
                $("#" + gridName + "_wrapper .dt-buttons").one("click",
                    function() {
                        if (!removeExport) {
                            $(".dt-button-collection").prepend($('<li class="dt-label" id="gridExportHeader">Export</li>'));
                        }
                        $(".buttons-columnVisibility:first").before($('<li class="dt-label" id="gridVisibilityHeader">Visibility</li>'));
                    }).on("click",
                    function(e) {
                        e.stopPropagation();
                    });

                //position search footer under header
                $("#" + gridName + "_wrapper .dataTables_scrollFoot")
                    .insertAfter("#" + gridName + "_wrapper .dataTables_scrollHead");

                /* Remove the export buttons if remove-export is specified. */
                if (removeExport) {
                    $(that).DataTable().buttons(".buttons-csv, .buttons-excel, .buttons-pdf, .buttons-print").remove();
                }

                /* Scroll fixed footer bar as table content scrolls. */
                var id = that[0].id;
                var $scrollBody = $("#" + id + "_wrapper .dataTables_scrollBody");
                var timer = null;

                $scrollBody.scroll(function(e) {

                    /* Ensure we are only updating the fixed header every few MS, 
                     * otherwise it will fire continuously as you scroll. */
                    if (timer !== null) {
                        clearTimeout(timer);
                    }
                    timer = setTimeout(function() {
                            $("#" + id).DataTable().fixedHeader.adjust();
                        },
                        50);
                });
            }
        });

    $("body").on("preInit.dt",
        "table",
        function() {
            var $dt = $(this).dataTable();

            $dt.api().columns().every(function() {

                var column = this;
                this.search("");

                if (column.footer()) {
                    var $cell = $(this.footer());
                    var title = $cell[0].textContent;

                    $cell.html('<input type="text" id="Search_' + title.replace(" ", "") + '" placeholder="' + title + '" value="" /><i class="fa fa-search"/>');

                    function debounced(delay) {
                        var timerId;
                        return function() {

                            var searchText = this.value;

                            if (timerId) {
                                clearTimeout(timerId);
                            }

                            timerId = setTimeout(function() {
                                    if (column.search() !== searchText) {
                                        column.search(searchText).draw();
                                    }

                                    timerId = null;
                                },
                                delay);
                        };
                    };

                    $("input", this.footer()).on("keyup change", debounced(500));
                }
            });
        });
});