
CREATE TABLE IF NOT EXISTS `question_standard_code` (
  `id` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `questionId` char(36) NOT NULL DEFAULT '',
  `code` longtext,
  `type` varchar(16) NOT NULL DEFAULT 'normal',
  `isDefault` tinyint(1) NOT NULL DEFAULT '0' COMMENT '�Ƿ�Ĭ�ϴ� ��ʦ������������õĴ�',
`modifyTime` datetime NOT NULL COMMENT '����ʱ��',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;