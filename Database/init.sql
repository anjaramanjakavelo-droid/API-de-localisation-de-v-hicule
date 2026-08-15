-- Script de création de la base de données pour XAMPP / MySQL
-- Exécuter dans phpMyAdmin ou en ligne de commande MySQL

CREATE DATABASE IF NOT EXISTS vehicle_tracking
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE vehicle_tracking;

CREATE TABLE IF NOT EXISTS vehicule (
    id INT AUTO_INCREMENT PRIMARY KEY,
    immatriculation VARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS position_gps (
    id INT AUTO_INCREMENT PRIMARY KEY,
    vehicule_id INT NOT NULL,
    latitude DECIMAL(10, 7) NOT NULL,
    longitude DECIMAL(10, 7) NOT NULL,
    date_position DATETIME NOT NULL,
    CONSTRAINT fk_position_vehicule
        FOREIGN KEY (vehicule_id) REFERENCES vehicule(id)
        ON DELETE CASCADE
);

CREATE INDEX idx_position_vehicule ON position_gps(vehicule_id);
CREATE INDEX idx_position_date ON position_gps(date_position);
