
window.jQueryWidgets = {
    initialize: function() {

         /*-----------------------
        --> Off Canvas Menu
        -------------------------*/
        /*Variables*/
        var $offCanvasNav = window.$('.off-canvas-nav'),
            $offCanvasNavSubMenu = $offCanvasNav.find('.sub-menu');

        /*Add Toggle Button With Off Canvas Sub Menu*/
        $offCanvasNavSubMenu.parent().prepend('<span class="menu-expand"><i class="fas fa-chevron-down"></i></span>');

        /*Close Off Canvas Sub Menu*/
        $offCanvasNavSubMenu.slideUp();

        /*Category Sub Menu Toggle*/
        $offCanvasNav.on('click', 'li a, li .menu-expand', function(e) {
            var $this = window.$(this);
            if (($this.parent().attr('class').match(/\b(menu-item-has-children|has-children|has-sub-menu)\b/)) && ($this.attr('href') === '#' || $this.hasClass('menu-expand'))) {
                e.preventDefault();
                if ($this.siblings('ul:visible').length) {
                    $this.parent('li').removeClass('active');
                    $this.siblings('ul').slideUp();
                } else {
                    $this.parent('li').addClass('active');
                    $this.closest('li').siblings('li').find('ul:visible').slideUp();
                    $this.siblings('ul').slideDown();
                }
            }
        });

        // Off Canvas Open close
        window.$(".off-canvas-btn").on('click', function() {
            window.$(".off-canvas-wrapper").addClass('open');
        });

        window.$(".btn-close-off-canvas").on('click', function() {
            window.$(".off-canvas-wrapper").removeClass('open');
        });

        /**********************
         *Expand Category Menu
         ***********************/

        function categoryMenuExpand() {
            window.$(".hidden-menu-item").css('display', 'none');

            window.$(window).on({
                load: function() {
                    var ww = window.$(window).width();
                    if (ww <= 1200) {
                        window.$(".hidden-lg-menu-item").css('display', 'none');
                    }
                },
                resize: function() {

                    var ww = window.$(window).width();
                    if (ww <= 1200) {
                        window.$(".hidden-lg-menu-item").css('display', 'none');
                    }
                }
            });
            window.$(".js-expand-hidden-menu").on('click', function(e) {
                e.preventDefault();
                window.$(".hidden-menu-item").toggle(500);
                var window_width = window.$(window).width();
                if (window_width <= 1200) {
                    window.$(".hidden-lg-menu-item").toggle(500);
                }
                var htmlAfter = "Close Categories";
                var htmlBefore = "More Categories";

                window.$(this).html(window.$(this).text() === htmlAfter ? htmlBefore : htmlAfter);
                window.$(this).toggleClass("menu-close");
            });
        }
        /**********************
         *Expand Category Mobile Menu 
         ***********************/

        function categoryMenuExpandInMobile() {
            window.$('.category-menu .has-children > a').on('click', function(e) {
                e.preventDefault();
                window.$(this).siblings('.sub-menu').slideToggle('500');
            });
        }
        categoryMenuExpand();
        categoryMenuExpandInMobile();

        /*------------------------
        	--> Search PopUp
        ------------------------*/
        (function() {
            window.$(".search-trigger").on('click', function() {
                window.$(".search-wrapper").addClass('open');
            });
            window.$(".search-dismiss,body").on('click', function() {
                window.$(".search-wrapper").removeClass('open');
            });
            // $("body").on('click', function () { 
            // 	$(".search-wrapper").removeClass('open')
            // })
            window.$(".search-box,.search-trigger").on('click', function(e) {
                e.stopPropagation();
            });
        })();

        window.$('.category-trigger').on('click', function(e) {
            window.$('.category-nav').toggleClass('show');
            e.stopPropagation();
        });

        /*-------------------------------------
        	--> Sticky Header
        ---------------------------------------*/
        function stickyHeader() {

            var headerHeight = window.$('.site-header')[0].getBoundingClientRect().height;
            window.$(window).on({
                resize: function() {
                    var width = window.$(window).width();
                    if ((width <= 991)) {
                        window.$('.sticky-init').removeClass('fixed-header');
                        if (window.$('.sticky-init').hasClass('sticky-header')) {
                            window.$('.sticky-init').removeClass('sticky-header');
                        }

                    } else {
                        window.$('.sticky-init').addClass('fixed-header');
                    }
                },
                load: function() {
                    var width = window.$(window).width();
                    if ((width <= 991)) {
                        window.$('.sticky-init').removeClass('fixed-header');
                        if (window.$('.sticky-init').hasClass('sticky-header')) {
                            window.$('.sticky-init').removeClass('sticky-header');
                        }

                    } else {
                        window.$('.sticky-init').addClass('fixed-header');
                    }
                }
            });
            window.$(window).on('scroll', function() {
                if (window.$(window).scrollTop() >= headerHeight) {
                    window.$('.fixed-header').addClass('sticky-header');
                } else {
                    window.$('.fixed-header').removeClass('sticky-header');
                }
            });


        }
        stickyHeader();

        /*-------------------------------------
        	--> Data Background Image
        ---------------------------------------*/
        function bgImageSettings() {
            window.$('.bg-image').each(function() {
                var $this = window.$(this),
                    $image = $this.data('bg');

                $this.css({
                    'background-image': 'url(' + $image + ')'
                });
            });
        }

        bgImageSettings();

        /*-------------------------------------
        	--> NIce Select
        ---------------------------------------*/
        window.$('.nice-select').niceSelect();

        /*---------------------------------------------------------------------------------------
        --> Scroll Top (When the user clicks on the button, scroll to the top of the document)
        -----------------------------------------------------------------------------------------*/
        window.$.scrollUp({
            scrollText: '<i class="ion-chevron-right"></i><i class="ion-chevron-right"></i>',
            easingType: 'linear',
            scrollSpeed: 900
        });
    }
};