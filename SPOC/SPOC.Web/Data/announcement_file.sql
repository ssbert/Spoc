CREATE TABLE IF NOT EXISTS `announcement_file` (
  `id` char(36) NOT NULL,
  `announcementId` char(36) NOT NULL,
  `uploadFileId` char(36) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;