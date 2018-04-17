CREATE TABLE IF NOT EXISTS `label_rule` (
  `id` char(36) NOT NULL COMMENT 'ID',
  `labelId` char(36)  NOT NULL COMMENT '标签id',
  `matchText` varchar(36) NOT NULL COMMENT '关键字',
  `logic` tinyint(1) NOT NULL DEFAULT 0 COMMENT '逻辑关系',
  `describe` varchar(128)  COMMENT '描述',
  `regExpressions` varchar(128) COMMENT '正则表达式',
  `createTime` datetime NOT NULL COMMENT '创建时间',
 `creatorId` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
   `seq` int(2) NOT NULL DEFAULT 0 COMMENT '顺序号',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;