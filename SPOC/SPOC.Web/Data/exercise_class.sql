CREATE TABLE IF NOT EXISTS `exercise_class` (
  `id` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  `classId` char(36) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '',
  PRIMARY KEY (`id`,`classId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;