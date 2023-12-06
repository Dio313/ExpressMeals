window.global = {
    openModal: function(modalId){
        modalId = '#'+modalId;
        window.$(modalId).modal('show');
    },
    closeModal:function(modalId){
        modalId = '#'+modalId;
        window.$(modalId).modal('hide');
    }
}