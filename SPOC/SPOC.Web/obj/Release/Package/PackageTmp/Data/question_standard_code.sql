
CREATE TABLE IF NOT EXISTS `question_standard_code` (
  `id` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `questionId` char(36) NOT NULL DEFAULT '',
  `code` longtext,
  `type` varchar(16) NOT NULL DEFAULT 'normal',
  `isDefault` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否默认答案 教师试题管理内设置的答案',
`modifyTime` datetime NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;