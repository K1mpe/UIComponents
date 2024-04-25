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


            tabContent.triggerHandler('uic-opened');
            oldActiveContent.triggerHandler('uic-closed');

            let tabContainer = tab.closest('.card-tabs');
            tabContainer.triggerHandler('uic-tab-change', tabHeader, oldActiveHeader);
        }
    },
    setTabHash: function (tab) {
        var hash = [];
        $(tab).parents('.card-tabs').each(function (index, item) {
            var openTab = $(item).find('> .card-header .active[role=tab],>.row > .col-tab-headers .active[role=tab]');
            if (openTab.length)
                hash[index] = openTab.attr('href').replace("#", "");
        });
        let scrollTop = $('html').scrollTop();
        window.location.hash = hash.reverse().join(',');
        $('html, body').scrollTop(scrollTop);
    },
    openFirstTab: function (tabContainer) {
        let url = window.location;
        let hashes = url.hash.split(",");
        let conti= true;
        hashes.forEach(function (hash, index) {
            hash = hash.replace("#", "");
            let tab = tabContainer.find(`[role="tab"][href="#${hash}"]`);
            if (tab.length) {
                uic.tabs.open(tab);
                conti = false;
                return;
            }
                
            
        })
        if (!conti)
            return;
        try {
            let tabId = tabContainer.attr('id');
            let openTabId = localStorage.getItem(`tabs-lastState-${tabId}`);
            if ($(openTabId).length) {
                uic.tabs.open($(openTabId));
                conti = false;
                return;
            }
                
        } catch { }

        if (!conti)
            return;
        let firstTab = tabContainer.find('[role="tab"]')[0];
        uic.tabs.open($(firstTab));

    }
};
$(document).on('click', '.uic.card-tabs [role="tab"]', async function (ev) {

    let newTab = $(ev.target).closest('[role="tab"]');        // Newly activated tab
    ev.stopImmediatePropagation();
    uic.tabs.open(newTab);
    uic.tabs.setTabHash(newTab);


});
$(document).on('.uic.card-tabs', 'uic-help', () => {
    console.log("tabs .on('uic-tab-change', (ev, oldHeader, newHeader) => {...} Triggered when a tab changes");
});
$(document).on('.uic.card-tabs .tab-pane', 'uic-help',()=> {

    console.log("tab .on('uic-open', () => {...} Triggered when the tab opens");
    console.log("tab .on('uic-close', () => {...} Triggered when the tab closes");
});