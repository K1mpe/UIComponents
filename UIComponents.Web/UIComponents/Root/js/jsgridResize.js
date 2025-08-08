//https://github.com/tabalinas/jsgrid/issues/9
{
    function preventEvent(e) {
        var ev = e || window.event;
        if (ev.preventDefault) ev.preventDefault();
        else ev.returnValue = false;
        if (ev.stopPropagation)
            ev.stopPropagation();
        return false;
    }

    function getWidth(x) {
        if (x.currentStyle)
            // in IE
            var y = x.clientWidth - parseInt(x.currentStyle["paddingLeft"]) - parseInt(x.currentStyle["paddingRight"]);
        // for IE5: var y = x.offsetWidth;
        else if (window.getComputedStyle)
            // in Gecko
            var y = document.defaultView.getComputedStyle(x, null).getPropertyValue("width");
        return y || 0;
    }

    let ColumnResize = function (aTable, aConnectedTable) {
        var table = aTable;
        var connectedTable = aConnectedTable;

        if (table == null || table == undefined || table.tagName != 'TABLE')
            return;

        this.id = table.id;

        // ============================================================
        // private data
        var self = this;

        var dragColumns = table.rows[0].cells; // first row columns, used for changing of width

        var dragConnectedColumns = dragColumns;
        if (connectedTable)
            dragConnectedColumns = connectedTable.rows[0].cells;

        if (!dragColumns)
            return; // return if no table exists or no one row exists

        var dragColumnNo; // current dragging column
        var dragX; // last event X mouse coordinate

        var saveOnmouseup; // save document onmouseup event handler
        var saveOnmousemove; // save document onmousemove event handler
        var saveBodyCursor; // save body cursor property
        let preventFilter = false;
        // ============================================================
        // methods

        // ============================================================
        // do changes columns widths
        // returns true if success and false otherwise
        this.changeColumnWidth = function (no, w) {
            if (!dragColumns) return false;

            if (no < 0) return false;
            if (dragColumns.length < no) return false;

            if (parseInt(dragColumns[no].style.width) <= -w) return false;
            if (dragColumns[no + 1] && parseInt(dragColumns[no + 1].style.width) <= w) return false;

            dragColumns[no].style.width = parseInt(dragColumns[no].style.width) + w + 'px';
            dragConnectedColumns[no].style.width = parseInt(dragConnectedColumns[no].style.width) + w + 'px';

            //if (dragColumns[no + 1]) {
            //    dragColumns[no + 1].style.width = parseInt(dragColumns[no + 1].style.width) - w + 'px';
            //    dragConnectedColumns[no + 1].style.width = parseInt(dragConnectedColumns[no + 1].style.width) - w + 'px';
            //}

            return true;
        }

        // ============================================================
        // do drag column width
        this.columnDrag = function (e) {
            var e = e || window.event;
            var X = e.clientX || e.pageX;
            if (!self.changeColumnWidth(dragColumnNo, X - dragX)) {
                // stop drag!
                self.stopColumnDrag(e);
            }

            dragX = X;
            // prevent other event handling
            preventEvent(e);
            return false;
        }

        // ============================================================
        // stops column dragging
        this.stopColumnDrag = function (e) {

            var e = e || window.event;
            if (!dragColumns) return;

            // restore handlers & cursor
            document.onmouseup = saveOnmouseup;
            document.onmousemove = saveOnmousemove;
            document.body.style.cursor = saveBodyCursor;

            // remember columns widths in cookies for server side
            var colWidth = '';
            var separator = '';
            for (var i = 0; i < dragColumns.length; i++) {
                colWidth += separator + parseInt(getWidth(dragColumns[i]));
                separator = '+';
            }
            
            preventEvent(e);
            setTimeout(() => preventFilter = false, 100); // prevent the filtering to trigger
        }

        // ============================================================
        // init data and start dragging
        this.startColumnDrag = function (e) {
            e.stopPropagation();
            var e = e || window.event;
            preventFilter = true;
            // if not first button was clicked
            //if (e.button != 0) return;

            // remember dragging object
            dragColumnNo = $(e.target).closest('th').index();
            dragX = e.clientX || e.pageX;

            // set up current columns widths in their particular attributes
            // do it in two steps to avoid jumps on page!
            var colWidth = new Array();
            for (var i = 0; i < dragColumns.length; i++) {
                colWidth[i] = parseInt(getWidth(dragColumns[i]));
            }
            for (var i = 0; i < dragColumns.length; i++) {
                dragColumns[i].width = ""; // for sure
                dragColumns[i].style.width = colWidth[i] + "px";
                dragConnectedColumns[i].width = ""; // for sure
                dragConnectedColumns[i].style.width = colWidth[i] + "px";
            }

            saveOnmouseup = document.onmouseup;
            document.onmouseup = self.stopColumnDrag;

            saveBodyCursor = document.body.style.cursor;
            document.body.style.cursor = 'w-resize';

            // fire!
            saveOnmousemove = document.onmousemove;
            document.onmousemove = self.columnDrag;

            preventEvent(e);
        }

        // prepare table header to be draggable
        // it runs during class creation

        for (var i = 0; i < dragColumns.length; i++) {
            if ($(dragColumns[i]).find('.jsgrid-insert-mode-button').length)
                continue;
            let currentColumnContent = $(dragColumns[i]).html();
            $(dragColumns[i]).html($("<div style='position:relative;height:100%;width:100%'></div>")
                .append(currentColumnContent)
                .append($("<div class='jsgrid-header-resizable' style='position:absolute;height:100%;color:transparent;width:5px;right:0px;top:0;cursor:w-resize;z-index:10;'></div>"))
            );
            $(dragColumns[i]).find('.jsgrid-header-resizable').on('mousedown', this.startColumnDrag);
            $(dragColumns[i]).find('.jsgrid-header-resizable').on('mouseup', (ev) => {
                if (preventEvent) {
                    ev.stopPropagation();
                    this.stopColumnDrag(ev);
                }
                
            });
            $(dragColumns[i]).children().on('click', (ev) => {
                if (preventFilter)
                    ev.stopPropagation();
            })
        }
    }

    uic.jsgrid.resizeColumn = function ($grid) {
        $grid = $grid.closest('.jsgrid');
        let header = $grid.find('.jsgrid-grid-header table');
        let body = $grid.find('.jsgrid-grid-body table');
        ColumnResize(header[0], body[0]);
    };
}