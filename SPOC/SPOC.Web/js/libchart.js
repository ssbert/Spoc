(function(win, eacharts, $){

    var _self,
        obj,
        chart,
        title,
        saveName,
		nodesUrl,
		linesUrl,
        labelLink,
        labelCallBack,
        ruleCallBack,
        callback,
		datas = [], 
		links = [];

	var options = {
		backgroundColor: '#fff',
        title: {
			top: "top",
            left: "center"
		},
		tooltip: {
			show: false
		},
		legend: [{
			left: 0,
			itemGap: 20,
			formatter: function (name) {
				return echarts.format.truncateText(name, 60, '14px Microsoft Yahei', '…');
			},
			tooltip: {
				show: true
			},
			selectedMode: true,
			data: ['已掌握', '未掌握', '不稳定', '无反馈']
		}],
		toolbox: {
			show: true,
			right: 20,
			itemGap: 15,
			feature: {
				restore: {
					show: true
				},
				saveAsImage: {
                    show: true,
                    name: saveName
				}
			}
		},
		animationDuration: 500,
		animationEasingUpdate: 'cubicOut',
		series: [{
			type: 'graph',
			top: 160,
			bottom: 100,
			layout: 'force',
			draggable: true,
			cursor: "pointer",
			force: {
				repulsion: 300,
				edgeLength: 80
			},
			data: datas,
			links: links,
			symbolSize: function (value, params) {
				var spaceWidth = 14;
				if (params.name.length > 4) {
					return [spaceWidth * params.name.length, 36]
				}
				return [60, 36]
			},
			categories: [
			{
				name: "章节",
				itemStyle: {
					normal: {
						color: "#fff"
					}
				}
			},
			{
				name: '语言',
				itemStyle: {
					normal: {
						color: "#fff"
					}
				}
			}, {
				name: '已掌握',
				itemStyle: {
					normal: {
						color: '#66CDAA'
					}
				}
			}, {
				name: '未掌握',
				itemStyle: {
					normal: {
						color: '#FF7256'
					}
				}
			}, {
				name: '不稳定',
				itemStyle: {
					normal: {
						color: '#FFFF00'
					}
				}
			}, {
				name: '无反馈',
				itemStyle: {
					normal: {
						color: '#B0C4DE'
					}
				}
			}
			],
			focusNodeAdjacency: true,
			roam: true,
			label: {
				normal: {
					show: true,
					position: 'top',
					color: "#363636"
				}
			},
			itemStyle: {
				normal: {
					color: '#aaa',
					borderColor: '#86d0fe',
					borderWidth: 2,
					shadowBlur: 3,
					shadowColor: '#87CEFF',
				}
			},
			lineStyle: {
				normal: {
					color: '#86d0fe',
					curveness: 0,
					type: "solid",
					width: 2,
					curveness: 0.1
				}
			},
			textStyle: {
				color: "#B03A5B"
			}
		}]

	};

	var libChart = {

		initChart: function(options, callBack){
			_self = this;
            obj = options.obj || '#libchart';
            saveName = options.saveName || "知识图谱";
			nodesUrl = options.nodesUrl;
			linesUrl = options.linesUrl;
            labelLink = options.labelLink;
            labelCallBack = options.labelCallBack;
            ruleCallBack = options.ruleCallBack;
            callback = callBack;
            chart = echarts.init(document.querySelector(obj));
            _self.getData(nodesUrl, linesUrl);
		},

		/**
		 * 获取数据
		 * 无返回数据
		 */
		getData: function(nodesUrl, linesUrl){
			$.ajax({
			    method: "GET",
                url: nodesUrl + "?" + new Date().getTime(),
                success: function (nodeData) {
                    if (!nodeData) {
                        $(obj).html("<div class='no-data-tip iconfont icon-nodata' style='line-height:800px'></div>");
                        return false;
                    }
					var nodeObj = $.parseJSON(nodeData);
					//var nodeObj = nodeData;
					$.ajax({
						method: "GET",
						url: linesUrl,
						success: function(staticData){
							//var staticObj = $.parseJSON(staticData).result;
							var staticObj = staticData.result;
                            _self.mergeData(nodeObj, staticObj);
                            _self.setContainerWidth();
                            chart.setOption(options);
                            _self.setSaveName(saveName);
                            _self.onEvent();
                            if (callback && typeof callback == "function") {
                                callback();
                            }
	                    }
	                })
				}
			});
		},

		/**
		 * 合并数据
		 * @param  {Object} nodeObj   节点
		 * @param  {Object} staticObj 状态         
		 */
		mergeData: function (nodeObj, staticObj) {
			title = nodeObj.title;
			var nodes = nodeObj.nodes,
			lines = nodeObj.lines;
			$.each(nodes, function (key, value) {
				if (value.labelId) {
					staticObj[value.labelId] == undefined ? value.static = "none" : value.static = staticObj[value.labelId]
				}
				_self.mergeNodes(key, value);
			});
			$.each(lines, function (key, value) {
				_self.mergeLines(value);
			})
		},

		/**
		 * 节点处理
		 * @param  {String} id    节点id
		 * @param  {Object} value 节点id对应的对象
		 */
		mergeNodes: function (id, value) {
			if (value.type === "cube") {
				_self.setCubeNode(id, value);
			} else if (value.type === "tag round") {
				_self.setRoundNode(id, value);
			}
		},

		/**
		 * 连线处理
		 * @param  {Object} value 连线关系对象
		 */
		mergeLines: function (value) {
			links.push({
				source: value.from,
				target: value.to
			})
		},

		/**
		 * 配置普通节点
		 * @param {String} id    节点id
		 * @param {Object} value 节点id对应的对象
		 */
		setCubeNode: function (id, value) {
			var node = {
				id: id,
				name: value.name,
				symbol: 'roundRect',
				category: "章节",
				label: {
					normal: {
						position: 'inside',
						color: "#363636"
					}
				}
			};
			if (value.rootNode) {
				node.category = "语言",
				node.symbolSize = [66, 66];
				node.label.normal = {
					position: 'inside',
					color: "#363636",
					fontSize: 18,
					fontWeight: 600
				}
			}
			datas.push(node);
		},

		/**
		 * 配置标签节点
		 * @param {String} id    节点id
		 * @param {Object} value 节点id对应的对象
		 */
		setRoundNode: function (id, value) {
			var node = {
				id: id,
				name: value.name,
				symbol: 'circle',
				symbolSize: 36,
				labelId: value.labelId
			};

            if (typeof ruleCallBack == "function") {
                ruleCallBack(setStatic, value.static);
            } else {
                throw ("$ ruleCallBack 是必须项");
                return false;
            }

            datas.push(node);

            function setStatic(static) {
                node.category = static;
            }
        },

		/**
		 * 开启事件绑定
		 */
		onEvent: function(){
			chart.on('click', function (params) {
                if (params.dataType == "node" && params.data.symbol == "circle") {
                    if (typeof labelCallBack == "function") {
                        labelCallBack(labelLink, params.data.labelId);
                        return false;
                    }
		        	window.open(labelLink + params.data.labelId);
		        }
		    });

			chart.on('mouseover', function (params) {
		        if (params.dataType == "node" && params.data.symbol == "circle") {
		        	document.querySelector(obj + " canvas").style.cursor = "pointer"
		        }
		    });

			chart.on('mouseout', function (params) {
				document.querySelector(obj + " canvas").style.cursor = "move"
			});

			$(window).on('resize', function () {
                _self.setContainerWidth();
			})
        },

        /**
		 * 获取外层容器宽度
		 */
        setContainerWidth: function () {
            var width = $(obj).parent().width();
            setTimeout(function () {
                $(obj).css('width', width + 'px');
                chart.resize();
            }, 300);
        },

        /**
		 * 另存为图片时的图片名称
		 * @param {String} name    保存图片的名称
		 */
        setSaveName: function (name) {
            chart.setOption({
                toolbox: {
                    feature: {
                        saveAsImage: {
                            name: name
                        }
                    }
                }
            });
        }

	} 

	win.libChart = libChart

})(window, echarts, jQuery);





