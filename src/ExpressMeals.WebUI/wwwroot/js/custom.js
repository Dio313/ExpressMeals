jQuery(document).ready(function($) {
    (function($) {

        'use strict';

       

        


        
        /*-------------------------------------
        	--> Range Slider
        ---------------------------------------*/
        $(function() {
            $(".sb-range-slider").slider({
                range: true,
                min: 0,
                max: 753,
                values: [80, 320],
                slide: function(event, ui) {
                    $("#amount").val("£" + ui.values[0] + " - £" + ui.values[1]);
                }
            });
            $("#amount").val("£" + $(".sb-range-slider").slider("values", 0) +
                " - £" + $(".sb-range-slider").slider("values", 1));
        });

       

        /*-------------------------------------
        	--> Quantity
        ---------------------------------------*/
        $('.count-btn').on('click', function() {
            var $button = $(this);
            var oldValue = $button.parent('.count-input-btns').parent().find('input').val();
            if ($button.hasClass('inc-ammount')) {
                var newVal = parseFloat(oldValue) + 1;
            } else {
                // Don't allow decrementing below zero
                if (oldValue > 0) {
                    var newVal = parseFloat(oldValue) - 1;
                } else {
                    newVal = 0;
                }
            }
            $button.parent('.count-input-btns').parent().find('input').val(newVal);
        });
        /*-------------------------------------
        	--> Shipping Form Toggle
        ---------------------------------------*/
        $('[data-shipping]').on('click', function() {
            if ($('[data-shipping]:checked').length > 0) {
                $('#shipping-form').slideDown();
            } else {
                $('#shipping-form').slideUp();
            }
        })
        /*-------------------------------------
        	--> Add To Cart Animation
        ---------------------------------------*/
        $('.add-to-cart').on('click', function(e) {
            e.preventDefault();

            if ($(this).hasClass('added')) {
                $(this).removeClass('added').find('i').removeClass('ti-check').addClass('ti-shopping-cart').siblings('span').text('add to cart');
            } else {
                $(this).addClass('added').find('i').addClass('ti-check').removeClass('ti-shopping-cart').siblings('span').text('added');
            }
        });
        

        
        

       
        /*-------------------------------------
        	--> Payment method select
        ---------------------------------------*/
        $('[name="payment-method"]').on('click', function() {

            var $value = $(this).attr('value');

            $('.single-method p').slideUp();
            $('[data-method="' + $value + '"]').slideDown();

        });
        $('.slide-trigger').on('click', function() {

            var $value = $(this).data('target');

            // $('.single-method p').slideUp();
            $($value).slideToggle();

        });



        
    })(jQuery);

   


    /*-------------------------------------
    	--> Countdown Activation
    ---------------------------------------*/

    $('[data-countdown]').each(function() {
        var $this = $(this),
            finalDate = $(this).data('countdown');
        $this.countdown(finalDate, function(event) {
            $this.html(event.strftime('<div class="single-countdown"><span class="single-countdown__time">%D</span><span class="single-countdown__text">Days</span></div><div class="single-countdown"><span class="single-countdown__time">%H</span><span class="single-countdown__text">Hours</span></div><div class="single-countdown"><span class="single-countdown__time">%M</span><span class="single-countdown__text">mins</span></div><div class="single-countdown"><span class="single-countdown__time">%S</span><span class="single-countdown__text">Secs</span></div>'));
        });
    });
    $('.color-list a').on('click', function(e) {
        e.preventDefault();
        var $this = $(this);
        $this.addClass('active');
        $this.siblings().removeClass('active');
        var $navs = document.querySelectorAll('#product-nav .single-slide');
        var $details = document.querySelectorAll('#products-details .single-slide');
        var $btnColor = $this.data('swatch-color');
        for (var i = 0; i < $navs.length; i++) {
            $navs[i].classList.remove('slick-current');
            if ($navs[i].classList.contains($btnColor)) {
                $navs[i].classList.add('slick-current');
            }
        }
        for (var i = 0; i < $details.length; i++) {
            $details[i].classList.remove('slick-current');
            $details[i].style.opacity = 0;
            if ($details[i].classList.contains($btnColor)) {
                $details[i].classList.add('slick-current');
                $details[i].style.opacity = 1;
            }
        }

    });

   

});