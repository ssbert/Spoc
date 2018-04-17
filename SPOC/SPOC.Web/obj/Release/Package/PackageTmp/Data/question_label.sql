
CREATE TABLE IF NOT EXISTS `question_label` (
  `id` char(36) NOT NULL COMMENT 'ID',
  `questionId` char(36) NOT NULL COMMENT '题库Id',
  `questionType` varchar(16) NOT NULL DEFAULT 'normal' COMMENT '题库类型 challenge:挑战题 normal:题库',
  `labelId` char(36) NOT NULL  COMMENT '描述',
  `labelType`  tinyint(1) NOT NULL DEFAULT '0'  COMMENT '标签类型',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;