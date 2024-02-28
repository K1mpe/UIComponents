uic.card = uic.card || {
    openCard: async function(card) {
        await card.triggerHandler('uic-before-open');
        card.CardWidget('expand');
    },
    closeCard: async function(card) {
        await card.triggerHandler('uic-before-close');
        card.CardWidget('collapse');
    },
    toggleCard: async function(card) {
        if (card.hasClass('collapsed-card'))
            await uic.card.openCard(card);
        else
            await uic.card.closeCard(card);
    }
};
$(document).ready(function () {
    $('.card').on('expanded.lte.cardwidget', (ev) => { ev.stopPropagation(); $(ev.target).triggerHandler('uic-opened'); });
    $('.card').on('collapsed.lte.cardwidget', (ev) => { ev.stopPropagation(); $(ev.target).triggerHandler('uic-closed'); });
});