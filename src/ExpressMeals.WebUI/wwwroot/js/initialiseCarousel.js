function initialiseCarousel() {
    
    /*------------------------
        	--> Slick Carousel
        ------------------------*/

        var $html = window.$('html');
        var $body = window.$('body');
        var $uptimoSlickSlider = window.$('.sb-slick-slider');

        /*For RTL*/
        if ($html.attr("dir") === "rtl" || $body.attr("dir") === "rtl") {
            $uptimoSlickSlider.attr("dir", "rtl");
        }

        $uptimoSlickSlider.each(function() {

            /*Setting Variables*/
            var $this = window.$(this),
                $setting = $this.data('slick-setting') ? $this.data('slick-setting') : '',
                $autoPlay = $setting.autoplay ? $setting.autoplay : false,
                $autoPlaySpeed = parseInt($setting.autoplaySpeed, 10) || 2000,
                $asNavFor = $setting.asNavFor ? $setting.asNavFor : null,
                $appendArrows = $setting.appendArrows ? $setting.appendArrows : $this,
                $appendDots = $setting.appendDots ? $setting.appendDots : $this,
                $arrows = $setting.arrows ? $setting.arrows : false,
                $prevArrow = $setting.prevArrow ? '<button class="' + ($setting.prevArrow.buttonClass ? $setting.prevArrow.buttonClass : 'slick-prev') + '"><i class="' + $setting.prevArrow.iconClass + '"></i></button>' : '<button class="slick-prev">previous</button>',
                $nextArrow = $setting.nextArrow ? '<button class="' + ($setting.nextArrow.buttonClass ? $setting.nextArrow.buttonClass : 'slick-next') + '"><i class="' + $setting.nextArrow.iconClass + '"></i></button>' : '<button class="slick-next">next</button>',
                $centerMode = $setting.centerMode ? $setting.centerMode : false,
                $centerPadding = $setting.centerPadding ? $setting.centerPadding : '50px',
                $dots = $setting.dots ? $setting.dots : false,
                $fade = $setting.fade ? $setting.fade : false,
                $focusOnSelect = $setting.focusOnSelect ? $setting.focusOnSelect : false,
                $infinite = $setting.infinite ? $setting.infinite : false,
                $pauseOnHover = $setting.pauseOnHover ? $setting.pauseOnHover : false,
                $rows = parseInt($setting.rows, 10) || 1,
                $slidesToShow = parseInt($setting.slidesToShow, 10) || 1,
                $slidesToScroll = parseInt($setting.slidesToScroll, 10) || 1,
                $swipe = $setting.swipe ? $setting.swipe : true,
                $swipeToSlide = $setting.swipeToSlide ? $setting.swipeToSlide : false,
                $variableWidth = $setting.variableWidth ? $setting.variableWidth : false,
                $vertical = $setting.vertical ? $setting.vertical : false,
                $verticalSwiping = $setting.verticalSwiping ? $setting.verticalSwiping : false,
                $rtl = $setting.rtl || $html.attr('dir="rtl"') || $body.attr('dir="rtl"') ? true : false;

            /*Responsive Variable, Array & Loops*/
            var $responsiveSetting = typeof $this.data('slick-responsive') !== 'undefined' ? $this.data('slick-responsive') : '',
                $responsiveSettingLength = $responsiveSetting.length,
                $responsiveArray = [];
            for (var i = 0; i < $responsiveSettingLength; i++) {
                $responsiveArray[i] = $responsiveSetting[i];
            }
            /*Slider Start*/
            $this.slick({
                autoplay: $autoPlay,
                autoplaySpeed: $autoPlaySpeed,
                asNavFor: $asNavFor,
                appendArrows: $appendArrows,
                appendDots: $appendDots,
                arrows: $arrows,
                dots: $dots,
                centerMode: $centerMode,
                centerPadding: $centerPadding,
                fade: $fade,
                focusOnSelect: $focusOnSelect,
                infinite: $infinite,
                pauseOnHover: $pauseOnHover,
                rows: $rows,
                slidesToShow: $slidesToShow,
                slidesToScroll: $slidesToScroll,
                swipe: $swipe,
                swipeToSlide: $swipeToSlide,
                variableWidth: $variableWidth,
                vertical: $vertical,
                verticalSwiping: $verticalSwiping,
                rtl: $rtl,
                prevArrow: $prevArrow,
                nextArrow: $nextArrow,
                responsive: $responsiveArray
            });

        });
        /*---------------------------
        	--> Dropdown Slide Item
        ----------------------------*/

        window.$(".slide-down--btn").on('click', function(e) {
            e.stopPropagation();
            window.$(this).siblings('.slide-down--item').slideToggle("400");
            window.$(this).siblings('.slide-down--item').toggleClass("show");
            var $selectClass = window.$(this).parents('.slide-wrapper').siblings().children('.slide-down--item');
            window.$(this).parents('.slide-wrapper').siblings().children('.slide-down--item').slideUp('400');
        });

        /*-------------------------------------
        	--> Slideup While clicking On Dom
        ---------------------------------------*/
        function clickDom() {
            window.$('body').on('click', function(e) {
                window.$('.slide-down--item').slideUp('500');
            });
            window.$('.slide-down--item').on('click', function(e) {
                e.stopPropagation();
            });
        };

        clickDom();

        /*-------------------------------------
           --> Product View Mode
       ---------------------------------------*/
        window.$('.product-view-mode a').on('click', function(e) {
            e.preventDefault();

            var shopProductWrap = window.$('.shop-product-wrap');
            var viewMode = window.$(this).data('target');

            window.$('.product-view-mode a').removeClass('active');
            window.$(this).addClass('active');
            shopProductWrap.removeClass('grid list').addClass(viewMode);
            if (shopProductWrap.hasClass('grid')) {
                window.$('.pm-product').removeClass('product-type-list');
            } else {
                window.$('.pm-product').addClass('product-type-list');
            }
        });

        /*-------------------------------------
           --> Product Sorting
       ---------------------------------------*/
        window.$('.product-view-mode a').on('click', function(e) {
            e.preventDefault();

            var shopProductWrap = window.$('.shop-product-wrap');
            var viewMode = window.$(this).data('target');

            window.$('.product-view-mode a').removeClass('active');
            window.$(this).addClass('active');
            shopProductWrap.removeClass('grid list grid-four').addClass(viewMode);
            if (shopProductWrap.hasClass('grid') || shopProductWrap.hasClass('grid-four')) {
                window.$('.product-card').removeClass('card-style-list');
            } else {
                window.$('.product-card').addClass('card-style-list');
            }
        });

}
