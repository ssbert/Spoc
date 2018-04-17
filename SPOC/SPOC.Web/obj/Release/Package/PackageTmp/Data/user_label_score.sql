CREATE TABLE IF NOT EXISTS `user_label_score` (
  `id` char(36) NOT NULL,
  `userId` char(36) NOT NULL,
  `labelId` char(36) NOT NULL,
  `score` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
