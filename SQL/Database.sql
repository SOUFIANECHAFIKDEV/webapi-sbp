--- 16/11/2018
---- Creation Table Client
CREATE TABLE IF NOT EXISTS `client` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `reference` varchar(255) DEFAULT NULL,
  `nom` varchar(255) DEFAULT NULL,
  `raison_sociale` varchar(45) DEFAULT NULL,
  `telephone` varchar(45) DEFAULT NULL,
  `fax` varchar(45) DEFAULT NULL,
  `email` varchar(45) DEFAULT NULL,
  `site_web` varchar(45) DEFAULT NULL,
  `siret` varchar(45) DEFAULT NULL,
  `tva_intra_communautaire` varchar(45) DEFAULT NULL,
  `typeClient` int(11) DEFAULT 0,
  `adresses` longtext DEFAULT NULL,
  `memos` longtext DEFAULT NULL,
  `contacts` longtext DEFAULT NULL,
  `historique` longtext DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `reference_unique` (`reference`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--- 22/10/2018
---- Creation Table departement
CREATE TABLE IF NOT EXISTS `departement` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `departement_code` varchar(3) DEFAULT NULL,
  `departement_nom` varchar(255) DEFAULT NULL,
  `id_pays` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `departement_code` (`departement_code`),
  KEY `fk_pays_departement` (`id_pays`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--- 23/10/2018
---- Creation Table Fournisseur
CREATE TABLE IF NOT EXISTS `fournisseur` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `reference` varchar(255) DEFAULT NULL,
  `nom` varchar(45) DEFAULT NULL,
  `raison_sociale` varchar(45) DEFAULT NULL,
  `id_pays` int(11) DEFAULT NULL,
  `adresse` varchar(45) DEFAULT NULL,
  `complement_adresse` varchar(45) DEFAULT NULL,
  `id_ville` int(11) DEFAULT NULL,
  `autre_ville` varchar(45) DEFAULT NULL,
  `code_postal` varchar(45) DEFAULT NULL,
  `id_departement` int(11) DEFAULT NULL,
  `telephone` varchar(45) DEFAULT NULL,
  `fax` varchar(45) DEFAULT NULL,
  `email` varchar(45) DEFAULT NULL,
  `site_web` varchar(45) DEFAULT NULL,
  `siret` varchar(45) DEFAULT NULL,
  `tva_intra_communautaire` varchar(45) DEFAULT NULL,
  `memos` longtext DEFAULT NULL,
  `contacts` longtext DEFAULT NULL,
  `historique` longtext DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `reference_unique` (`reference`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
-- 20/11/2018
alter table fournisseur drop column id_departement;
alter table fournisseur add column departement longtext;

-- 21/11/2018
ALTER TABLE `fournisseur`
	ADD COLUMN `id_agent` INT NULL AFTER `historique`,
	ADD CONSTRAINT `FK_agent_fournissseur` FOREIGN KEY (`id_agent`) REFERENCES `user` (`id`);
	
-- 22/11/2018
ALTER TABLE `pb`.`fournisseur` 
	DROP COLUMN `id_ville`,
	CHANGE COLUMN `autre_ville` `ville` VARCHAR(45) NULL DEFAULT NULL ;


--- 22/10/2018
---- Creation Table Pays
CREATE TABLE IF NOT EXISTS `pays` (
  `id` int(11) NOT NULL,
  `code` int(3) NOT NULL,
  `nom_en_gb` varchar(45) NOT NULL,
  `nom_fr_fr` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `code_unique` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--- 22/10/2018
---- Creation Table Ville
CREATE TABLE IF NOT EXISTS `ville` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `id_departement` int(11) DEFAULT NULL,
  `ville_nom` varchar(100) DEFAULT 'NULL',
  `ville_nom_reel` varchar(100) DEFAULT 'NULL',
  `code_postal` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_department_ville` (`id_departement`),
  CONSTRAINT `fk_department_ville` FOREIGN KEY (`id_departement`) REFERENCES `departement` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--- 30/11/2018
---- ajouter  ON DELETE CASCADE ON UPDATE CASCADE
drop table user_profile;

CREATE TABLE `user_profile` (
  `iduser` int(11) NOT NULL,
  `idprofile` int(11) NOT NULL,
  PRIMARY KEY (`iduser`,`idprofile`),
  KEY `fk_user_profile_profile_idx` (`idprofile`),
  CONSTRAINT `fk_user_profile_profile` FOREIGN KEY (`idprofile`) REFERENCES `profile` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_user_profile_user` FOREIGN KEY (`iduser`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
);

drop table projet_base.profile_action;

CREATE TABLE profile_action (
  id int(11) NOT NULL AUTO_INCREMENT,
  idprofile int(11) NOT NULL,
  idaction int(11) NOT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB AUTO_INCREMENT=247 DEFAULT CHARSET=utf8;



--- 04/12/2018
create table societe( 	
	idsociete varchar(50) NOT NULL PRIMARY KEY, 	
    idsocietemere varchar(50) NULL, 	
    CONSTRAINT fk_soeciete 	FOREIGN KEY (idsocietemere) REFERENCES societe (idsociete) 
    );


ALTER TABLE user ADD idsociete varchar(50) NULL;
ALTER TABLE user ADD CONSTRAINT FK_societe FOREIGN KEY (idsociete) REFERENCES societe(idsociete);
ALTER TABLE user ADD matricule varchar(20) NULL;


ALTER TABLE fournisseur ADD idsociete varchar(50) NULL;
ALTER TABLE fournisseur ADD CONSTRAINT FK_societe FOREIGN KEY (idsociete) REFERENCES societe(idsociete);


ALTER TABLE profile ADD idsociete varchar(50) NULL;
ALTER TABLE profile ADD CONSTRAINT FK_profile FOREIGN KEY (idsociete) REFERENCES societe(idsociete);


ALTER TABLE produit ADD idsociete varchar(50) NULL;
ALTER TABLE produit ADD CONSTRAINT FK_profile FOREIGN KEY (idsociete) REFERENCES societe(idsociete);
ALTER TABLE produit ADD labels longtext;


create table labels( 	
	id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY, 
	label varchar(255) NOT NULL , 
    idsociete varchar(50) NOT NULL, 	
    CONSTRAINT fk_labels_soeciete 	FOREIGN KEY (idsociete) REFERENCES societe (idsociete) 
    );
