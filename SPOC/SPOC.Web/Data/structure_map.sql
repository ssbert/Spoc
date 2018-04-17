CREATE TABLE IF NOT EXISTS `structure_map` (
  `id` char(36) NOT NULL,
  `creatorId` char(36) NOT NULL,
  `title` varchar(128) NOT NULL,
  `isShow` tinyint(2) NOT NULL,
  `isMain` tinyint(2) NOT NULL,
  `createTime` datetime NOT NULL,
  `mapData` longtext DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;