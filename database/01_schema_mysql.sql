-- =============================================================================
-- Ticketing Mundial 2026 — Esquema (DDL) - MySQL
-- Adaptado de PostgreSQL a MySQL
-- =============================================================================

-- ---------------------------------------------------------------------------
-- Catálogos / tablas simples
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS comision (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    porcentaje  DECIMAL(5, 2) NOT NULL
);

CREATE TABLE IF NOT EXISTS estado_venta (
    id     INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS estado_identidad (
    id     INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS equipo (
    id     INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS sede (
    nombre VARCHAR(100) PRIMARY KEY
);

-- ---------------------------------------------------------------------------
-- Usuarios y jerarquía ISA (Funcionario / Administrador)
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS usuario (
    numero_documento VARCHAR(30) PRIMARY KEY,
    mail             VARCHAR(150) NOT NULL,
    pais             VARCHAR(100),
    localidad        VARCHAR(100),
    calle            VARCHAR(150),
    numero_direccion VARCHAR(20),
    codigo_postal    VARCHAR(20),
    fecha_registro   TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS documento (
    usuario_numero_documento VARCHAR(30) NOT NULL,
    tipo                     VARCHAR(50) NOT NULL,
    pais                     VARCHAR(100),
    PRIMARY KEY (usuario_numero_documento, tipo),
    FOREIGN KEY (usuario_numero_documento) REFERENCES usuario (numero_documento)
);

CREATE TABLE IF NOT EXISTS telefono (
    usuario_numero_documento VARCHAR(30) NOT NULL,
    telefono                 VARCHAR(30) NOT NULL,
    PRIMARY KEY (usuario_numero_documento, telefono),
    FOREIGN KEY (usuario_numero_documento) REFERENCES usuario (numero_documento)
);

CREATE TABLE IF NOT EXISTS tiene_estado_identidad (
    usuario_numero_documento VARCHAR(30) NOT NULL,
    id_estado_identidad      INT NOT NULL,
    PRIMARY KEY (usuario_numero_documento, id_estado_identidad),
    FOREIGN KEY (usuario_numero_documento) REFERENCES usuario (numero_documento),
    FOREIGN KEY (id_estado_identidad) REFERENCES estado_identidad (id)
);

CREATE TABLE IF NOT EXISTS funcionario (
    numero_documento VARCHAR(30) PRIMARY KEY,
    numero_legajo    VARCHAR(30) NOT NULL,
    FOREIGN KEY (numero_documento) REFERENCES usuario (numero_documento)
);

CREATE TABLE IF NOT EXISTS dispositivo_autorizado (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    nombre           VARCHAR(100) NOT NULL,
    numero_documento VARCHAR(30) NOT NULL,
    FOREIGN KEY (numero_documento) REFERENCES funcionario (numero_documento)
);

CREATE TABLE IF NOT EXISTS administrador (
    numero_documento VARCHAR(30) PRIMARY KEY,
    fecha_asignacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    nombre_sede      VARCHAR(100) NOT NULL,
    FOREIGN KEY (numero_documento) REFERENCES usuario (numero_documento),
    FOREIGN KEY (nombre_sede) REFERENCES sede (nombre)
);

-- ---------------------------------------------------------------------------
-- Infraestructura: estadios y sectores
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS estadio (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    nombre_sede      VARCHAR(100) NOT NULL,
    capacidad_maxima INT NOT NULL,
    ubicacion        VARCHAR(200),
    FOREIGN KEY (nombre_sede) REFERENCES sede (nombre)
);

CREATE TABLE IF NOT EXISTS sector (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    id_estadio INT NOT NULL,
    nombre     VARCHAR(100) NOT NULL,
    capacidad  INT NOT NULL,
    FOREIGN KEY (id_estadio) REFERENCES estadio (id)
);

-- ---------------------------------------------------------------------------
-- Eventos deportivos
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS evento_deportivo (
    id                   INT AUTO_INCREMENT PRIMARY KEY,
    id_equipo_local      INT NOT NULL,
    id_equipo_visitante  INT NOT NULL,
    fecha                DATE NOT NULL,
    hora                 TIME NOT NULL,
    cantidad_entradas    INT NOT NULL,
    FOREIGN KEY (id_equipo_local) REFERENCES equipo (id),
    FOREIGN KEY (id_equipo_visitante) REFERENCES equipo (id)
);

-- Sectores habilitados por evento (PK compuesta sector + evento).
CREATE TABLE IF NOT EXISTS informacion_entrada (
    id_sector                      INT NOT NULL,
    id_evento_deportivo            INT NOT NULL,
    numero_documento_administrador VARCHAR(30) NOT NULL,
    PRIMARY KEY (id_sector, id_evento_deportivo),
    FOREIGN KEY (id_sector) REFERENCES sector (id),
    FOREIGN KEY (id_evento_deportivo) REFERENCES evento_deportivo (id),
    FOREIGN KEY (numero_documento_administrador) REFERENCES administrador (numero_documento)
);

-- ---------------------------------------------------------------------------
-- Ventas y entradas
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS venta (
    id                       INT AUTO_INCREMENT PRIMARY KEY,
    numero_documento_usuario VARCHAR(30) NOT NULL,
    id_comision              INT NOT NULL,
    fecha                    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    monto_total              DECIMAL(12, 2) NOT NULL,
    FOREIGN KEY (numero_documento_usuario) REFERENCES usuario (numero_documento),
    FOREIGN KEY (id_comision) REFERENCES comision (id)
);

CREATE TABLE IF NOT EXISTS venta_tiene_estado (
    id_venta        INT NOT NULL,
    id_estado_venta INT NOT NULL,
    PRIMARY KEY (id_venta, id_estado_venta),
    FOREIGN KEY (id_venta) REFERENCES venta (id),
    FOREIGN KEY (id_estado_venta) REFERENCES estado_venta (id)
);

CREATE TABLE IF NOT EXISTS entrada (
    id                             INT AUTO_INCREMENT PRIMARY KEY,
    estado                         VARCHAR(50) NOT NULL,
    fecha                          TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    estado_seed                    VARCHAR(100),
    qr_usado                       TINYINT(1) NOT NULL DEFAULT 0,
    costo                          DECIMAL(12, 2) NOT NULL,
    id_sector                      INT NOT NULL,
    id_evento_deportivo            INT NOT NULL,
    numero_documento_administrador VARCHAR(30),
    id_venta                       INT NOT NULL,
    FOREIGN KEY (id_sector) REFERENCES sector (id),
    FOREIGN KEY (id_evento_deportivo) REFERENCES evento_deportivo (id),
    FOREIGN KEY (numero_documento_administrador) REFERENCES administrador (numero_documento),
    FOREIGN KEY (id_venta) REFERENCES venta (id)
);

CREATE TABLE IF NOT EXISTS transferencia (
    numero_documento_emisor   VARCHAR(30) NOT NULL,
    numero_documento_receptor VARCHAR(30) NOT NULL,
    id_entrada                INT NOT NULL,
    fecha                     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (numero_documento_emisor, numero_documento_receptor, id_entrada),
    FOREIGN KEY (numero_documento_emisor) REFERENCES usuario (numero_documento),
    FOREIGN KEY (numero_documento_receptor) REFERENCES usuario (numero_documento),
    FOREIGN KEY (id_entrada) REFERENCES entrada (id)
);


CREATE INDEX idx_entrada_evento    ON entrada(id_evento_deportivo);
CREATE INDEX idx_entrada_venta     ON entrada(id_venta);
CREATE INDEX idx_venta_usuario     ON venta(numero_documento_usuario);
CREATE INDEX idx_transf_entrada    ON transferencia(id_entrada);
CREATE INDEX idx_transf_receptor   ON transferencia(numero_documento_receptor);
CREATE INDEX idx_evento_fecha      ON evento_deportivo(fecha);
