-- =============================================================================
-- Ticketing Mundial 2026 — Datos de prueba (seed) — PLACEHOLDER
-- Se ejecuta después de 01_schema.sql. Completar/ampliar según haga falta.
-- =============================================================================

-- Catálogos
INSERT INTO comision (porcentaje) VALUES (10.00);
INSERT INTO estado_venta (nombre) VALUES ('Pendiente'), ('Pagada'), ('Anulada');
INSERT INTO estado_identidad (nombre) VALUES ('Verificada'), ('Pendiente');

-- Equipos
INSERT INTO equipo (nombre) VALUES
    ('Uruguay'),
    ('Argentina'),
    ('Brasil'),
    ('España');

-- Sedes y estadios
INSERT INTO sede (nombre) VALUES ('Montevideo'), ('Buenos Aires');
INSERT INTO estadio (nombre_sede, capacidad_maxima, ubicacion) VALUES
    ('Montevideo', 60000, 'Centenario');
INSERT INTO sector (id_estadio, nombre, capacidad) VALUES
    (1, 'Tribuna Olímpica', 20000),
    (1, 'Tribuna Amsterdam', 15000);

-- TODO: agregar usuarios, eventos, ventas y entradas de prueba según se vaya
-- implementando la lógica (respetando las FKs declaradas en 01_schema.sql).
