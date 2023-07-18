/*********************************
 * Dashboard.js
 * -------------------------------
 * Date:    12Sept2017
 * Author:  BS
 * Desc:    Javascript to help functionality on the Dashboard page
 * Directly controls the Views/Dashboard/Index.cshtml page
 ********************************/

function Dashboard(saveDashboardApi, renderWidgetApi) {
    var self = this;
    this.isDirty = false;
    this.isEditMode = false;
    this.originalPosition = 0;
    this.renderWidgetApi = renderWidgetApi;
    this.saveDashboardApi = saveDashboardApi;
    this.init = function(tabPage) {
        self.bindAddEditDashboard();
        self.bindAddWidget();
        self.bindRemoveWidget();
        self.bindDragDrop();
        self.initWidgetList();
        $(".container-fluid").removeClass("container_white");
    };

    this.bindAddEditDashboard = function() {
        //Bind edit mode
        $("#dashboard-toggle").click(function(e) {

            //Toggle icons
            $("#dashboard-toggle-icon").toggleClass("fa-pencil");
            $("#dashboard-toggle-icon").toggleClass("fa-check");
            $("#dashboard-grid-container").toggleClass("edit");

            //Set draggable
            if (!self.isEditMode) {
                $(".widget").draggable({ disabled: false });
                self.isEditMode = true;
            } else {
                //Save if we changed something and disable draggable
                if (self.isDirty) {
                    self.saveDashboard();
                }
                $(".widget").draggable({ disabled: true });
                self.isEditMode = false;
            }
        });

        //Bind Add on hover
        $("#dashboard-edit-container").hover(function(e) {
            if (!self.IsEditMode) {
                $("#dashboard-add").toggleClass("open");
            }
        });
    };

    this.bindAddWidget = function() {
        //Bind open add widget panel
        $("#dashboard-add").click(function() {
            $("#dashboard-add-container").addClass("open");

            if (!self.isEditMode) {
                $("#dashboard-toggle").click();
            }
        });

        //Bind close add widget panel
        $("#dashboard-add-close").click(function() {
            $("#dashboard-add-container").removeClass("open");
            $("#dashboard-add-save").addClass("disabled");
            $(".widget-list-item").removeClass("selected");
        });

        //Bind add widget panel
        $("#dashboard-add-save").click(function() {
            if (!$(this).hasClass("disabled")) {
                var widgetId = $(".widget-list-item.selected").attr("data-widget-id");
                var json = { widgetId: widgetId };
                ajaxPost(self.renderWidgetApi, json, self.addWidgetSuccess, self.addWidgetFailure);
            }
        });

        //Bind close add widget panel
        $(".widget-list-item").click(function() {
            $(".widget-list-item").removeClass("selected");
            $("#dashboard-add-save").removeClass("disabled");
            $(this).addClass("selected");
        });
    };

    this.bindRemoveWidget = function(widget) {
        $widget = widget ? $(widget) : $(".widget-delete");
        $widget.click(function(e) {
            var widget = this.parentElement;
            self.removeWidget(widget);
        });
    };

    this.bindDragDrop = function() {
        //Bind draggable
        $(".widget").draggable(self.draggableOptions(true));

        //Bind droppable 
        $(".droppable").droppable(self.droppableOptions());

        //Bind droppable (disabled)
        $(".droppable.disabled").droppable(self.droppableOptions(true));
    };

    this.initWidgetList = function() {
        //Hide widget list items that are rendered
        var renderedWidgetIds = $(".row.widget-panel [data-widget-position]:has(>div) .widget")
            .map(function(e) { return $(this).attr("data-widget-id"); });
        for (var i = 0; i < renderedWidgetIds.length; i++) {
            var widgetListItem = $(".widget-list-item[data-widget-id='" + renderedWidgetIds[i] + "']");
            $(widgetListItem).addClass("hidden");
        }
    };

    this.addWidget = function(widgetBloc, targetWidgetBloc) {
        //Find first open bloc
        var $widgetBloc = $(widgetBloc);
        var width = $widgetBloc.data("widget-width");
        var height = $widgetBloc.data("widget-height");
        var $targetBloc = $(self.setWidgetBloc(width, height, targetWidgetBloc));

        //Set new widget position
        var widgetPosition = $targetBloc.data("widget-position");
        $widgetBloc.attr("data-widget-position", widgetPosition);
        $targetBloc.replaceWith($widgetBloc);

        //Remove disabled on previous
        for (j = 0; j < height; j++) {
            var rowPosition = widgetPosition + (4 * j);
            for (var i = 0; i < width; i++) {
                var posToCheck = rowPosition + i;
                var gridElement = $(".row.grid-panel [data-widget-position='" + posToCheck + "'] div");
                gridElement.droppable().droppable("disable");
            }
        }

        //Rebind delete & draggable to the new widget
        var widget = $widgetBloc.find(".widget");
        var widgetDelete = $widgetBloc.find(".widget-delete");
        $(widget).draggable(self.draggableOptions());
        self.bindRemoveWidget(widgetDelete);

        var widgetContainer = $(widget).parent();
        self.setFloat(widgetContainer, widgetPosition, width);

        //Hide the widget from list & deselect
        var selectedWidget = $(".widget-list-item.selected");
        $(selectedWidget).removeClass("selected");
        $(selectedWidget).addClass("hidden");
        $("#dashboard-add-save").addClass("disabled");
        self.isDirty = true;
    };

    this.removeWidget = function(widget) {
        //Reset & remove widget
        var $widget = $(widget);
        var widgetBloc = $widget.parent();
        self.resetWidgetBloc(widgetBloc);
        $widget.remove();

        //Show list item
        var widgetListItem = $(".widget-list-item[data-widget-id='" + $widget.attr("data-widget-id") + "']");
        $(widgetListItem).removeClass("hidden");
        self.isDirty = true;
    };

    this.resetWidgetBloc = function(widgetBloc) {
        //Reset current bloc
        var $widgetBloc = $(widgetBloc);
        $widgetBloc.removeClass();
        $widgetBloc.addClass("col-lg-3 col-ht-1");

        //Cleanup grid based on width/height
        var widgetPositions = [];
        var widgetPosition = $widgetBloc.data("widget-position");
        var width = $widgetBloc.data("widget-width");
        switch (width) {
        case 2:
            widgetPositions.push(widgetPosition + 1);
            break;
        case 3:
            widgetPositions.push(widgetPosition + 1);
            widgetPositions.push(widgetPosition + 2);
            break;
        case 4:
            widgetPositions.push(widgetPosition + 1);
            widgetPositions.push(widgetPosition + 2);
            widgetPositions.push(widgetPosition + 3);
            break;
        }
        //var widgetPositionsToEnable = [];
        //if (width > 3) {
        //    widgetPositionsToEnable.push(widgetPositions);
        //}
        var height = parseInt($widgetBloc.attr("data-widget-height"));
        for (i = 1; i < height; i++) {
            var nextRowPosition = widgetPosition + (4 * i);
            for (j = 0; j < width; j++) {
                widgetPositions.push(nextRowPosition + j);
            }
        }

        $widgetBloc.removeAttr("data-widget-width");
        $widgetBloc.removeAttr("data-widget-height");

        //Re-enable droppable on the row grid panel
        $(".row.grid-panel [data-widget-position='" + widgetPosition + "'] div").droppable("enable");
        //for (var i = 0; i < widgetPositions.length; i++) {
        //    var position = widgetPositions[i];
        //    var $gridBloc = $(".row.grid-panel [data-widget-position='" + position + "'] div");
        //    $gridBloc.droppable("enable");
        //}
        for (var i = 0; i < widgetPositions.length; i++) {
            //add the widget panels back 
            var position = widgetPositions[i];
            var prevPosition = parseInt(position) - 1;
            var $gridBloc = $(".row.grid-panel [data-widget-position='" + position + "'] div");
            $gridBloc.droppable("enable");
            $widgetBloc = $(".row.widget-panel [data-widget-position='" + prevPosition + "']");
            while ($widgetBloc.length == 0) {
                prevPosition = prevPosition - 1;
                $widgetBloc = $(".row.widget-panel [data-widget-position='" + prevPosition + "']");
            }
            $widgetBloc = $widgetBloc.after(self.renderWidgetBloc(position));
        }
    };

    this.setWidgetBloc = function(adjustedWidth, height, targetWidgetBloc) {
        //find empty blocks    
        var emptyBlocs = $(".row.widget-panel [data-widget-position]:not(:has(>div))");
        var allBlocs = $(".row.widget-panel >div");

        //Find target bloc if its not passed
        var validPosition = 0;
        if (!targetWidgetBloc) {
            targetWidgetBloc = self.findValidWidgetBloc(emptyBlocs, adjustedWidth, height);
        }

        //Remove extra blocs if needed
        var targetIndex = emptyBlocs.index(targetWidgetBloc);
        var dirtyIndex = targetIndex + 1;
        var positionOfTargetIndex = $(targetWidgetBloc).attr("data-widget-position");
        positionOfTargetIndex = parseInt(positionOfTargetIndex);
        for (i = 0; i < height; i++) {
            if (i == 0) {
                if (adjustedWidth > 1) {
                    var blocsToRemove = emptyBlocs.slice(dirtyIndex, (targetIndex + adjustedWidth));
                    for (var j = 0; j < blocsToRemove.length; j++) {
                        $(blocsToRemove[j]).remove();
                    }
                }
            } else {
                var nextRowBlockPosition = (positionOfTargetIndex + (4 * (i)));
                for (m = 0; m < adjustedWidth; m++) {
                    var widgetPosition = nextRowBlockPosition + m;
                    for (n = 0; n < emptyBlocs.length; n++) {
                        var emptyBloc = emptyBlocs[n];
                        var currentPos = parseInt($(emptyBloc).attr("data-widget-position"));
                        if (currentPos == widgetPosition) {
                            $(emptyBloc).remove();
                        }
                    }
                }

                //var blocsToRemove = emptyBlocs.slice(nextRowBlockIndex, (nextRowBlockIndex + adjustedWidth));
                //for (var k = 0; k < blocsToRemove.length; k++) {

                //}
            }
        }
        return targetWidgetBloc;
    };

    this.saveDashboard = function() {
        //Loop widgets displayed & post em
        var widgets = $(".row.widget-panel .widget");
        var json = widgets.length > 0 ? {} : null; //Pass null for empty
        for (var i = 0; i < widgets.length; i++) {
            var $widget = $(widgets[i]);
            var id = $widget.attr("data-widget-id");
            var widgetPosition = $widget.parent().attr("data-widget-position");

            if (json) {
                json[id] = widgetPosition;
            }
        }
        ajaxPost(self.saveDashboardApi,
            JSON.stringify({ widgetPositions: json }),
            self.saveDashboardSuccess,
            self.saveDashboardFailure,
            {
                contentType: "application/json"
            });
        self.isDirty = false;
    };

    this.renderWidgetBloc = function(position) {
        return "<div class='col-lg-3 col-ht-1' data-widget-position='" + position + "'></div>";
    };

    this.findValidWidgetBloc = function(emptyBlocs, width, height) {
        //Find first open bloc
        var widgetPositions = emptyBlocs.map(function(e) { return $(this).attr("data-widget-position"); });
        var validPosition = 0;
        var sequence = [];
        for (var i = 0; i < widgetPositions.length; i++) {
            var current = parseInt(widgetPositions[i]);
            var prev = i > 0 ? parseInt(widgetPositions[i - 1]) : null;
            if (prev == null || prev + 1 == current) {
                sequence.push(current);
                if (sequence.length == width) {
                    validPosition = sequence[0];
                    break;
                }
            } else {
                sequence = [];
            }
        }

        //Update valid position & return
        var validIndex = emptyBlocs.map(function(idx, el) {
            if ($(el).data("widget-position") == validPosition) {
                return idx;
            }
        })[0];
        var validBloc = emptyBlocs[validIndex];
        $(validBloc).attr("data-widget-position", validPosition);
        return validBloc;
    };

    this.findNextEmptyBloc = function(widget) {
        //Get empty blocs, default to first
        var width = $(widget).data("widget-width");
        //var widgetHeight = $widget.attr("data-widget-height");
        var emptyBlocs = $(".row.widget-panel [data-widget-position]:not(:has(>div))");
        var emptyBloc = emptyBlocs.first();
        var blocsToRemove = [];
        var widgetPositions = emptyBlocs.map(function(e) { return $(this).attr("data-widget-position"); });

        //Switch on width
        switch (width) {
        case 2:
            var validPosition = self.findValidWidgetPosition(widgetPositions, 2);
            var validIndex = emptyBlocs.map(function(idx, el) {
                if (el.getAttribute("data-widget-position") == validPosition) {
                    return idx;
                }
            });
            validIndex = parseInt(validIndex[0]);
            emptyBloc = emptyBlocs[validIndex];
            var dirtyIndex = validIndex + 1;
            blocsToRemove = emptyBlocs.slice(dirtyIndex, dirtyIndex + 1);
            break;
        case 3:
            emptyBloc = self.findValidWidgetPosition(widgetPositions, 3);
            emptyBlocs.slice(emptyBlocs.index(bloc), 2);
            break;
        case 4:
            emptyBloc = self.findValidWidgetPosition(widgetPositions, 4);
            emptyBlocs.slice(emptyBlocs.index(bloc), 3);
            break;
        }

        //Remove extra blocs
        for (var i = 0; i < blocsToRemove.length; i++) {
            $(blocsToRemove[i]).remove();
        }


        return emptyBloc;
    };

    this.addWidgetSuccess = function(data) {
        self.addWidget(data.widget);
    };

    this.addWidgetFailure = function(data) {
        //Losing
    };

    this.saveDashboardSuccess = function(e) {
        //Winning
    };

    this.saveDashboardFailure = function(e) {
        //Losing
    };

    this.getNewDroppedPosition = function(offsetTop, offsetLeft) {
        var listofWidgetElements = $(".row.grid-panel >div");

        //Get all top poistions  except 1st
        var topPositions = [];
        for (i = 1; i < listofWidgetElements.length / 4; i++) {
            topPositions.push($(listofWidgetElements[i * 4]).offset().top);
        }
        var droppedRow = 0;
        var lastRow = true;
        for (i = 0; i < topPositions.length; i++) {
            if (offsetTop <= topPositions[i]) { //starts from 0
                lastRow = false;
                droppedRow = i;
                break;
            }
        }
        if (lastRow) {
            droppedRow = i;
        }

        //Get left positions except 1st
        var leftPositions = [
            $(listofWidgetElements[1]).offset().left,
            $(listofWidgetElements[2]).offset().left,
            $(listofWidgetElements[3]).offset().left
        ];
        var droppedColumn = 0;
        var lastColumn = true;
        for (i = 0; i < leftPositions.length; i++) { //starts from 0                   
            if (offsetLeft <= leftPositions[i]) {
                lastColumn = false;
                droppedColumn = i;
                break;
            }
        }
        if (lastColumn) {
            droppedColumn = i;
        }
        return (droppedRow * 4) + (droppedColumn + 1);
    };

    this.droppedPositionIsValid = function(position, width, height) {
        var isValid = true;

        //Check position exists
        var newElement = $(".row.widget-panel [data-widget-position='" + position + "']");
        if (newElement.length == 0) {
            return false;
        }

        // Stop dropping in an invalid area . This check is only needed when
        if (width > 1) {
            //check for width to be acceptable
            if (width == 4 && (position % 4 != 1)) {
                isValid = false;
            } else if (width == 3 && (position % 4 == 3)) {
                isValid = false;
            } else if (width == 2 && (position % 4 == 0)) {
                isValid = false;
            }
        }

        // check if all the blocks that are needed for the widgets width are empty
        for (j = 0; j < height; j++) {
            var rowPosition = position + (4 * j); //get next row 
            for (var i = 0; i < width; i++) {
                var positionToCheck = rowPosition + i;
                var elementsToCheck = $(".row.widget-panel [data-widget-position='" + positionToCheck + "']:has(>div)");
                if (elementsToCheck.length != 0) {
                    isValid = false;
                    break;
                }
            }
        }
        return isValid;
    };

    this.setFloat = function(widget, position, width) {
        var furthestColumn = position + (width - 1);
        if (furthestColumn % 4 == 0) {
            widget.addClass("float-right");
        } else {
            widget.removeClass("float-right");
        }
    };

    this.droppableOptions = function(disabled) {
        var options = {
            disabled: disabled ? true : false,
            classes: { "ui-droppable-hover": "ui-state-hover" },
            drop: function(event, ui) {
                var oldWidgetElement = $(".row.widget-panel [data-widget-position='" + self.originalPosition + "']");
                var width = oldWidgetElement.data("widget-width");
                var height = oldWidgetElement.data("widget-height");

                //Get new position
                var newPosition = self.getNewDroppedPosition(ui.offset.top, ui.offset.left);

                //Validate & return if invalid
                if (!self.droppedPositionIsValid(newPosition, width, height)) {
                    $(ui.draggable).draggable("option", "revert", true);
                    return;
                }

                //Replace widget
                var newWidgetElement = $(".row.widget-panel [data-widget-position='" + newPosition + "']");
                newWidgetElement.attr("data-widget-width", width);
                newWidgetElement.attr("data-widget-height", height);

                //Set float
                self.setFloat(newWidgetElement, newPosition, width);

                var targetBloc = $(self.setWidgetBloc(width, height, newWidgetElement));
                if (width != 1 || height != 1) {
                    targetBloc.removeClass("col-lg-3 col-ht-1");
                    targetBloc.addClass("col-lg-" + (width * 3) + " col-ht-" + height);
                }
                $(ui.draggable).detach().css({
                    top: 0,
                    left: 0
                }).appendTo(targetBloc);

                //Clean up old widget  and blocs        
                self.resetWidgetBloc(oldWidgetElement);
                var newPosInt = parseInt(newPosition);
                for (j = 0; j < height; j++) {
                    var rowPosition = newPosInt + (4 * j);
                    for (var i = 0; i < width; i++) {
                        var positionToCheck = rowPosition + i;
                        var gridElement = $(".row.grid-panel [data-widget-position='" + positionToCheck + "'] div");
                        gridElement.droppable("disable");
                        var originalPosInt = parseInt(self.originalPosition, 10) + i;
                        var oldGridElement = $(".row.grid-panel [data-widget-position='" + originalPosInt + "'] div");
                        oldGridElement.droppable("enable");
                    }
                }
                self.isDirty = true;
            }
        };
        return options;
    };

    this.draggableOptions = function(disabled) {
        var options = {
            disabled: disabled ? true : false,
            revert: "invalid",
            start: function(event, ui) {
                self.originalPosition = this.parentElement.getAttribute("data-widget-position");
            }
        };
        return options;
    };
}