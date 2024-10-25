uic.card = uic.card || {
    help: function(id){
        console.log($(`#${id}`));
        console.log(`$('#${id}').trigger('uic-open') Open the card`);
        console.log(`$('#${id}').trigger('uic-close') Close the card`);
        console.log(`$('#${id}').trigger('uic-toggle') Open or Close the card`);
        console.log(`$('#${id}').on('uic-before-open', ()=>{...}) Triggered before the card starts opening`);
        console.log(`$('#${id}').on('uic-before-close', ()=>{...}) Triggered before the card starts closing`);
        console.log(`$('#${id}').on('uic-opened', () =>{...}) Triggered when the opening animation is complete`);
        console.log(`$('#${id}').on('uic-closed', () =>{...}) Triggered when the clsoing animation is complete`);
    },
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
    },
    _init: function(card){
        card.on('uic-open', (ev)=>{
            uic.card.openCard(card);
        });
        card.on('uic-close', (ev)=>{
            ev.stopPropagation();
            uic.card.closeCard(card);
        });
        card.on('uic-toggle', (ev)=>{
            ev.stopPropagation();
            uic.card.toggleCard(card);
        });

        if(window.location.hash.includes(card.attr('id')))
            setTimeout(()=>{
                card.trigger('uic-open');
            }, 10);
    }
};
$(document).ready(function () {
    $('.card').on('expanded.lte.cardwidget', (ev) => { ev.stopPropagation(); $(ev.target).triggerHandler('uic-opened'); });
    $('.card').on('collapsed.lte.cardwidget', (ev) => { ev.stopPropagation(); $(ev.target).triggerHandler('uic-closed'); });
});