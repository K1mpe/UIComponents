uic.tabs = uic.tabs || {
    open: function (tab) {
        tab = $(tab);
        if (!tab.length)
            return;

        let tabHeader;
        let tabContent;
        if (tab.attr('role') == 'tab') {
            tabHeader = tab;
            tabContent = $(`#${tabHeader.data('target')}`);
            if (!tabContent.length)
                tabContent = $(tabHeader.attr('href'));
        }
        else if (tab.attr('role') == 'tabpanel') {
            tabContent = tab;
            tabHeader = $(`[role=tab][source='#${tabContent.attr('id')}'],[role=tab][href='#${tabContent.attr('id')}']`);
        }



        let oldActiveHeader = tabHeader.parent().children('.active[role=tab]');
        let oldActiveContent = tabContent.parent().children('.active[role=tabpanel]');

        if (tabHeader.length && tabContent.length) {
            tabHeader.parent().children('[role=tab]').removeClass('active');
            tabContent.parent().children('[role=tabpanel]').removeClass('active').removeClass('show');
            tabHeader.addClass('active');
            tabContent.addClass('active show');


            tabContent.triggerHandler('uic-open');
            oldActiveContent.triggerHandler('uic-close');

            let tabContainer = tab.closest('.card-tabs');
            tabContainer.triggerHandler('uic-tab-change', oldActiveHeader, tabHeader);
        }
    },
    setTabHash: function (tab) {
        var hash = [];
        $(tab).parents('.card-tabs').each(function (index, item) {
            var openTab = $(item).find('> .card-header .active[role=tab]');
            if (openTab.length)
                hash[index] = openTab.attr('href').replace("#", "");
        });
        window.location.hash = hash.reverse().join(',');
    }
};

$(document).on('click', '[role="tab"]', async function (ev) {

    let newTab = $(ev.target);        // Newly activated tab
    ev.stopImmediatePropagation();
    open(newTab);
    setTabHash(newTab);


});

$(document).on('.uic.card-tabs', 'uic-help', () => {
    console.log("tabs .on('uic-tab-change', (ev, oldHeader, newHeader) => {...} Triggered when a tab changes");
});
$(document).on('.uic.card-tabs .tab-pane', 'uic-help',()=> {

    console.log("tab .on('uic-open', () => {...} Triggered when the tab opens");
    console.log("tab .on('uic-close', () => {...} Triggered when the tab closes");
});