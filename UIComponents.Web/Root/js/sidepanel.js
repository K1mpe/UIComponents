uic.sidePanel = uic.sidePanel || {
    initialize: function (container, startState) {
        container = $(container);
        let isHorizontal = container.hasClass('horizontal');

        let uniqueName = container.data('sidebar-name');
        let setState = startState;
        if (uniqueName.length) {
            try {
                setState = Number(localStorage.getItem(`sidePanel.${uniqueName}.state`));
            } catch { }

        }

        switch (setState) {
            case 0:
                uic.sidePanel.SetCollapse(container);
                break;
            case 1:
                uic.sidePanel.SetOverlay(container);
                break;
            case 2:
                uic.sidePanel.SetPinned(container);
                break;
        }


        if (isHorizontal) {
            uic.sidePanel.setHeightOnChange(container);
        }

        container.find('.side-panel').removeClass('d-none');

        container.find('.side-panel').on('open', function (e) {
            e.stopPropagation();
            uic.sidePanel.setOverlay(container);
        });
        container.find('.side-panel').on('close', function (e) {
            e.stopPropagation();
            uic.sidePanel.setCollapse(container);
        });

        container.find('.btn-sidebar-fixed').click(function (e) {
            e.preventDefault();
            uic.sidePanel.setPinned(container);
        });

        container.find('.btn-sidebar-open').click(function (e) {
            e.preventDefault();
            uic.sidePanel.setOverlay(container);
        });

        container.find('.btn-sidebar-close').click(function (e) {
            e.preventDefault();
            uic.sidePanel.setCollapse(container);
        });



        $(document).click(function (e) {

            let target = $(e.target);
            if (!container.find('.side-panel').hasClass('overlay'))
                return;

            if (target.hasClass('btn-sidebar-open') || target.closest('.btn-sidebar-open').length)
                return;

            if ($(e.target).closest('.side-panel').length)
                return;

            uic.sidePanel.setCollapse(container);
        });
    },
    setHeight: function (container) {
        let height = $(container).find('> .side-panel-content').height();

        $(container).find('.side-panel').height(height + 'px');
    },
    setHeightOnChange: function (container) {
        uic.sidePanel.setHeight(container);

        let content = $(container).find('> .side-panel-content');
        new ResizeObserver((element) => {
            let container = $(element[0].target).closest('.side-panel-container');
            uic.sidePanel.setHeight(container);
        }).observe(content[0]);
    },
    saveState = function (container, state) {
        let name = container.data('sidebar-name');
        if (!name.length)
            return;

        localStorage.setItem(`sidePanel.${name}.state`, state);
    },
    setCollapse: function (container) {
        let sidePanel = container.find('.side-panel');
        sidePanel.removeClass('col col-1 fit-content');

        sidePanel.removeClass('overlay fixed');
        sidePanel.addClass('collapsed');

        container.find('.btn-sidebar-open').removeClass('d-none');
        uic.sidePanel.SaveState(container, 0);
    },
    setOverlay: function (container) {
        let sidePanel = container.find('.side-panel');
        sidePanel.removeClass('col col-1 fit-content');

        sidePanel.removeClass('collapsed fixed');
        sidePanel.addClass('overlay');

        sidePanel.find('.btn-sidebar-fixed').removeClass('d-none');
        container.find('.btn-sidebar-open').addClass('d-none');
        uic.sidePanel.SaveState(container, 1);
    },
    setPinned: function (container) {

        let sidePanel = container.find('.side-panel');
        sidePanel.addClass('col col-1 fit-content');

        sidePanel.removeClass('collapsed overlay');
        sidePanel.addClass('fixed');

        sidePanel.find('.btn-sidebar-fixed').addClass('d-none');
        container.find('.btn-sidebar-open').addClass('d-none');
        uic.sidePanel.SaveState(container, 2);
    }
};
