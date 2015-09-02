SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------------------------------------------------------
-- Schema BahamutDB
-- ----------------------------------------------------------------------------
DROP SCHEMA IF EXISTS `BahamutDB` ;
CREATE SCHEMA IF NOT EXISTS `BahamutDB` ;

-- ----------------------------------------------------------------------------
-- Table BahamutDB.Account
-- ----------------------------------------------------------------------------
CREATE  TABLE IF NOT EXISTS `BahamutDB`.`Account` (
  `AccountID` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'account id,backend assign' ,
  `AccountName` VARCHAR(60) NOT NULL UNIQUE COMMENT 'account name,user assign when regist' ,
  `Email` VARCHAR(100) NULL DEFAULT NULL UNIQUE,
  `Mobile` VARCHAR(40) NULL DEFAULT NULL,
  `Name` VARCHAR(100) NULL DEFAULT NULL ,
  `CreateTime` DATETIME NOT NULL ,
  `Password` LONGTEXT NOT NULL ,
  `Extra` LONGTEXT NULL DEFAULT NULL COMMENT 'extra information' ,
  PRIMARY KEY (`AccountID`) )
ENGINE = InnoDB
AUTO_INCREMENT = 147258
DEFAULT CHARACTER SET = utf8
COMMENT = 'Account';

-- ----------------------------------------------------------------------------
-- Table BahamutDB.Account_SharelinkUser
-- ----------------------------------------------------------------------------
CREATE  TABLE IF NOT EXISTS `BahamutDB`.`SharelinkUserAccount` (
  `AccountID` BIGINT NOT NULL ,
  `AppUserID` BIGINT NOT NULL,
  `CreateTime` DATETIME NOT NULL ,
  PRIMARY KEY (`AccountID`) ,
  INDEX `FK_Account_SharelinkUser_AccountID` (`AccountID` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COMMENT = 'relation with the sharelink id';

CREATE  TABLE IF NOT EXISTS `BahamutDB`.`App` (
  `Appkey` varchar(128) NOT NULL,
  `Name` varchar(128) NOT NULL,
  `UserTableName` varchar(128) NOT NULL,
  PRIMARY KEY (`Appkey`),
  UNIQUE KEY `AppKey_UNIQUE` (`Appkey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='app table save the app information';


SET FOREIGN_KEY_CHECKS = 1;