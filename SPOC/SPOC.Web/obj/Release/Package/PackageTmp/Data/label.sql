CREATE TABLE IF NOT EXISTS `label` (
  `id` char(36) NOT NULL COMMENT 'ID',
  `folderId` char(36) NOT NULL COMMENT '标签分类Id',
  `title` varchar(36) NOT NULL COMMENT '标签名称',
  `describe` varchar(128)  COMMENT '描述',
  `regExpressions` varchar(128)   COMMENT '正则表达式',
  `createTime` datetime NOT NULL COMMENT '创建时间',
 `creatorId` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;