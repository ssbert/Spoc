

selectStar($('.rate'));

// 评价星星
function selectStar(obj) {

	//鼠标移入事件
	obj.children('.star').on("mouseenter", function(e){
		var target = $(e.target);
		target.removeClass('glyphicon-star-empty').addClass('glyphicon-star').prevAll().removeClass('glyphicon-star-empty').addClass('glyphicon-star');
	});

	//鼠标移除事件
	obj.children('.star').on("mouseleave", function(e){
		var target = $(e.target);
		target.removeClass('glyphicon-star').addClass('glyphicon-star-empty').prevAll().removeClass('glyphicon-star').addClass('glyphicon-star-empty');

		//处理有selected类标记的星星
		$('.star.selected').removeClass('glyphicon-star-empty').addClass('glyphicon-star').prevAll().removeClass('glyphicon-star-empty').addClass('glyphicon-star');
		$('.star.selected').nextAll().removeClass('glyphicon-star').addClass('glyphicon-star-empty');
	});

	//点击事件
	obj.children('.star').on("click", function(e){
		var target = $(e.target);
		// 给点击的星星添加selected类标记
		target.addClass('selected').nextAll().removeClass('glyphicon-star').addClass('glyphicon-star-empty');
		target.siblings().removeClass('selected');
	});
	
}

// 左侧目录
function setSlideList(obj, btn){
    if(!obj || !btn) {
        return;
    }
    if ($(window).outerWidth() >= 768) {
        return;
    }
    // 目录按钮点击事件
    btn.on('click', function(){
        obj.css('transform', 'translateX(0)')
    })

    obj.on('click', function(e){
        e.stopPropagation();
    })

    obj.parents('body').mouseup(function(e){  
      if(!obj.is(e.target) && obj.has(e.target).length === 0){ 
        obj.css('transform', 'translateX(-99%)');
      }
    });
}


