﻿(function ($) {
    // Add support scrolling of page durign drag and drop (DnD)
    // when not supported by the browser. Smart enough to detect
    // scroll support on the fly.
    // Adds invisible scroll areas top and bottom the viewport.
    // When a dragged element enters either area, will scroll if
    // the browser isn't scrolling automatically.
    $.fn.dndPageScroll = function (options) {
        options || (options = {});
        var defaults = {
            topId: 'top_scroll_page',
            bottomId: 'bottom_scroll_page',
            delay: 50,
            height: 20,
            /* CMS EDIT START */
            activate: true
            /* CMS EDIT END */
        };
        var options = $.extend(defaults, options);

        var top_el = $('#' + options.topId);
        if (!top_el.length)
            top_el = $('<div id="top_scroll_page">&nbsp;</div>').appendTo('body');

        var bottom_el = $('#' + options.bottomId);
        if (!bottom_el.length)
            bottom_el = $('<div id="bottom_scroll_page">&nbsp;</div>').appendTo('body');

        var both_el = $('#top_scroll_page, #bottom_scroll_page');

        both_el.hide();
        both_el.css({
            position: 'fixed', left: 0, right: 0,
            height: options.height, zIndex: 999999
        });
        top_el.css({ top: 0 });
        bottom_el.css({ bottom: 0 });

        // When DnD occurs over a scroll area - scroll the page!
        var lastTop;
        var lastBottom;
        both_el.on('dragenter', function (e) {
            var direction = ($(this).attr('id') == options.topId) ? 'up' : 'down';
            return true;
        });
        both_el.on('dragover', function (e) {
            // Wait a little while before doing stuff here again
            if ($('html,body').is(':animated')) return true;

            var scrollTop = $(window).scrollTop();
            var direction = ($(this).attr('id') == options.topId) ? -1 : 1;
            var last = (direction == -1) ? lastTop : lastBottom;
            var current = (direction == -1) ? scrollTop : $(document).height() - (scrollTop + $(window).height());

            if (last != undefined && last == current && current > 0) {
                var newScrollTop = scrollTop + direction * 50;
                $('html,body').animate(
					{ scrollTop: newScrollTop },
					options.delay,
					'linear'
				);
            }

            if (direction == -1) lastTop = current; else lastBottom = current;
            return true;
        });

        // Function to hide the scroll areas. Reset everything.
        var _hide = function (e) {
            both_el.hide();
            timestamp = undefined;
            scrolltop = 0;
            scrollbottom = 0;
            return true;
        };

        /* CMS EDIT START */
        if (options.activate) {
            // When a DND drag event starts, show the scroll areas
            $(document).on('dragstart', function (e) {
                both_el.show();
                return true;
            });
            // When DND ends, hide it.
            $(document).on('dragend', _hide);
        }
        /* CMS EDIT END */

        // In IE dragend does not always get triggered.
        // Workaround by hiding areas when the mouse enters one.
        both_el.on('mouseover', _hide);
    };
})($cmsj);