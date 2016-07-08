/************************************************************************************/
/*  JQuery Plugin:  LOADING                                                       	*/
/*  Author:	William Liu                                                            	*/
/*  Date: 	2012-3-30      															*/
/*  Files: 	jquery.lwh.loading.js ;  jquery.lwh.loading.css							*/
/************************************************************************************/
$.fn.extend({
	lwhLoading:function( opts ){
		var def_settings = {
								loadMsg:	"LOADING...",
								zIndex:		99000
						 	};
		$.extend(def_settings, opts);
		
		var mask_zidx	= def_settings.zIndex;
		var mask_ifrm	= "#lwhLoading_mask_ifrm";
		var mask_div	= "#lwhLoading_mask_div";
		if($(mask_ifrm).length <= 0 )	{
			$(document.body).append('<iframe id="lwhLoading_mask_ifrm" class="lwhLoading-mask-ifrm" style="z-index:' + mask_zidx  + ';"></iframe>');
		} 

		if($(mask_div).length <= 0 ) 	{
			$(document.body).append('<div id="lwhLoading_mask_div" class="lwhLoading-mask-div" style="z-index:' + (mask_zidx + 1) + ';"></div>');
		}
		def_settings.zIndex = mask_zidx + 2;
		
		return this.each( function(idx, el) { 
			$(el).data("default_settings", def_settings);
			$(el).append('<div class="lwhLoading-msgText">' + def_settings.loadMsg + '</div><div class="lwhLoading-loadingImage"></div>');
			$(el).css("zIndex", def_settings.zIndex);
			def_settings.zIndex++;

		});
	},
	
	loadShow:function(opts){
		return this.each( function(idx, el) {
				var mask_ifrm	= "#lwhLoading_mask_ifrm";
				var mask_div 	= "#lwhLoading_mask_div";
				
				var def_settings = $(el).data("default_settings");
				$.extend(def_settings, opts);
				if(opts && opts.loadMsg && opts.loadMsg != "") {
					$(".lwhLoading-msgText", el).html(opts.loadMsg);
				} 

				$(mask_ifrm).show();
				$(mask_div).show();
				$(el).show();
		});
	},
	
	loadHide:function(){
		return this.each( function(idx, el) {
				var mask_ifrm	= "#lwhLoading_mask_ifrm";
				var mask_div 	= "#lwhLoading_mask_div";
				var def_settings = $(el).data("default_settings");
				
				$(el).hide();
                				
				if( $(".lwhLoading:visible").length <= 0 ) {
						$(mask_ifrm).hide();
						$(mask_div).hide();
				}
		});
	}
});


/************************************************************************************/
/*  JQuery Plugin  Tooltips		                                                 	*/
/*  Author:	William Liu                                                            	*/
/*  Date: 	2012-2-5      															*/
/*  Files: 	jquery.lwh.tooltips.js ;  jquery.lwh.tooltips.css						*/
/************************************************************************************/
$.fn.extend({
	lwhTooltip:function( opts ){
			var def_settings = {
									hAlign:		"center",   //"left", "center", "right"
									vAlign:		"middle",  //"top", 	"middle", "bottom"
									offsetww:	20,
									offsethh:	20 	
							};
			$.extend(def_settings, opts);
			return this.each( function(idx, el) { 
				$(el).data("default_settings", def_settings);
			});
	},

	TShow: function( msg ) {
		return this.each( function(idx, el) {
			var def_settings = $(el).data("default_settings");
			$(".lwhTooltip_message", el).html(msg);
            $(el).show();
		});
	},
	
	THide: function() {
		$(el).fadeOut(1000);
	},
	
	autoTShow: function( msg ) {
		return this.each( function(idx, el) {
			var def_settings = $(el).data("default_settings");
			$(".lwhTooltip_message", el).html(msg);
			
			var el_ww 	= $(el).outerWidth();  // include border and padding
			var el_hh 	= $(el).outerHeight();
			//console.log( $(el).outerWidth() + " : " + $(el).outerHeight() );  // include border and padding
			//console.log( $(el).innerWidth() + " : " + $(el).innerHeight() );   // include padding
			//console.log( $(el).width() + " : " + $(el).height() );			   // no border and no padding

			var win_ww = $(window).width();  
			var win_hh = $(window).height();  
			// $(window).height();	  browser height 
			// $(document).height();  whole html document height; 
			// width
			if( def_settings ) {
				switch( def_settings.hAlign.toLowerCase() ) {
					case "left":
						$(el).css({"margin-left":"0px", "left":"20px"});
						break;
					case "right":
						var pos_left = ( win_ww - el_ww )> 0 ? ( win_ww - el_ww ) - 20 : 20;
						$(el).css({"margin-left":"0px", "left": pos_left + "px" });						
						break;
					default:
						var margin_left = ( win_ww - el_ww )> 0 ? el_ww / 2 : 20;
						margin_left = margin_left * -1;  							
						$(el).css("margin-left", margin_left + "px");
						break;
				}
				// end of width
				// height
				switch( def_settings.vAlign.toLowerCase() ) {
					case "top":
						$(el).css({"margin-top":"0px", "top":"20px"});
						break;
					case "bottom":
						var pos_top = ( win_hh - el_hh )> 0 ? ( win_hh - el_hh ) - 20 : 20;
						$(el).css({"margin-top":"0px", "top": pos_top + "px" });						
						break;
					default:
						var margin_top = ( win_hh - el_hh )> 0 ? el_hh / 2 : 20;
						margin_top = margin_top * -1;  							
						$(el).css("margin-top", margin_top + "px");
						break;
				}
				// end of height
			}
			$(el).stop(true, true).fadeIn(10).delay(2000).fadeOut(1000);
		});
	}
});