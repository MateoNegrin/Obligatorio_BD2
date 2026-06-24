-- =============================================================================
-- Ticketing Mundial 2026 — Datos de prueba (seed)
-- Se ejecuta después de 01_schema.sql. Contiene datos para todas las tablas.
-- =============================================================================

-- ---------------------------------------------------------------------------
-- CATÁLOGOS (tablas simples)
-- ---------------------------------------------------------------------------

-- Comisiones
INSERT INTO comision (porcentaje) VALUES (5.00), (10.00), (15.00);

-- Estados de venta
INSERT INTO estado_venta (nombre) VALUES ('Pendiente'), ('Pagada'), ('Anulada'), ('Reembolsada');

-- Estados de identidad
INSERT INTO estado_identidad (nombre) VALUES ('Verificada'), ('Pendiente'), ('Rechazada');

-- Equipos (8 equipos de prueba)
INSERT INTO equipo (nombre) VALUES
    ('Uruguay'),
    ('Argentina'),
    ('Brasil'),
    ('España'),
    ('Francia'),
    ('Alemania'),
    ('Italia'),
    ('Portugal');

-- Sedes
INSERT INTO sede (nombre) VALUES ('Montevideo'), ('Buenos Aires'), ('São Paulo');

-- ---------------------------------------------------------------------------
-- INFRAESTRUCTURA: Estadios y Sectores
-- ---------------------------------------------------------------------------

-- Estadios
INSERT INTO estadio (nombre_sede, capacidad_maxima, ubicacion) VALUES
    ('Montevideo', 60000, 'Estadio Centenario'),
    ('Montevideo', 45000, 'Estadio Parque Central'),
    ('Buenos Aires', 75000, 'Estadio Alberto J. Armando'),
    ('Buenos Aires', 65000, 'Estadio Juan Domingo Perón'),
    ('São Paulo', 70000, 'Estadio do Morumbi');

-- Sectores (por estadio)
INSERT INTO sector (id_estadio, nombre, capacidad) VALUES
    (1, 'Tribuna Olímpica', 20000),
    (1, 'Tribuna Amsterdam', 15000),
    (1, 'Tribuna del Centenario', 12000),
    (1, 'Palco Este', 13000),
    (2, 'Sector Oriente', 15000),
    (2, 'Sector Occidente', 15000),
    (2, 'Sector Popular', 15000),
    (3, 'Platea Alta', 20000),
    (3, 'Platea Baja', 25000),
    (3, 'Palco', 30000),
    (4, 'Tribuna Norte', 18000),
    (4, 'Tribuna Sur', 18000),
    (4, 'Tribuna Lateral Este', 15000),
    (4, 'Tribuna Lateral Oeste', 14000),
    (5, 'Setor Azul', 20000),
    (5, 'Setor Vermelho', 22000),
    (5, 'Setor Verde', 14000),
    (5, 'Setor Amarelo', 14000);

-- ---------------------------------------------------------------------------
-- USUARIOS y su jerarquía ISA (Usuario General, Funcionario, Administrador)
-- ---------------------------------------------------------------------------

-- Usuarios generales (compradores de entradas)
INSERT INTO usuario (numero_documento, mail, pais, localidad, calle, numero_direccion, codigo_postal) VALUES
    ('87654321', 'maria.garcia@email.com', 'Uruguay', 'Canelones', 'Avenida Italia', '5678', '91500'),
    ('11223344', 'carlos.rodriguez@email.com', 'Argentina', 'Buenos Aires', 'Avenida 9 de Julio', '2000', 'C1043'),
    ('44332211', 'ana.martinez@email.com', 'Argentina', 'Córdoba', 'Calle Rivadavia', '1500', 'X5000'),
    ('55667788', 'pedro.silva@email.com', 'Brasil', 'São Paulo', 'Avenida Paulista', '1000', '01310-100'),
    ('88776655', 'lucia.ferreira@email.com', 'Brasil', 'Rio de Janeiro', 'Avenida Atlântica', '500', '22070-011'),
    ('99887766', 'luis.gonzalez@email.com', 'Uruguay', 'Punta del Este', 'Calle 20', '100', '20100'),
    ('66778899', 'sofia.mendez@email.com', 'Argentina', 'La Plata', 'Calle 1', '123', 'B1900');

-- Usuarios que son funcionarios
INSERT INTO usuario (numero_documento, mail, pais, localidad, calle, numero_direccion, codigo_postal) VALUES
    ('F0001234', 'funcionario1@ticketing.com', 'Uruguay', 'Montevideo', 'Avenida 18 de Julio', '1450', '11100'),
    ('F0005678', 'funcionario2@ticketing.com', 'Argentina', 'Buenos Aires', 'Avenida Corrientes', '3000', 'C1043'),
    ('F0009012', 'funcionario3@ticketing.com', 'Brasil', 'São Paulo', 'Avenida São Bento', '500', '01014-001');

-- Usuarios que son administradores
INSERT INTO usuario (numero_documento, mail, pais, localidad, calle, numero_direccion, codigo_postal) VALUES
    ('ADM001', 'admin.montevideo@ticketing.com', 'Uruguay', 'Montevideo', 'Calle Sarandí', '670', '11000'),
    ('ADM002', 'admin.buenos.aires@ticketing.com', 'Argentina', 'Buenos Aires', 'Calle Florida', '100', 'C1005'),
    ('ADM003', 'admin.sao.paulo@ticketing.com', 'Brasil', 'São Paulo', 'Avenida Ipiranga', '200', '01046-010');

-- Cuentas de prueba para autenticación (Firebase): general, funcionario (supervisor) y administrador
INSERT INTO usuario (numero_documento, mail, pais, localidad, calle, numero_direccion, codigo_postal, fecha_registro) VALUES
    ('12345678', 'test@email.com',       'Uruguay', 'Montevideo', '24',          '222', '11800', '2026-06-20 20:21:50'),
    ('11111111', 'supervisor@email.com', 'Uruguay', 'Montevideo', '18 de julio', '122', '11000', '2026-06-21 00:25:47'),
    ('22222222', 'admin@email.com',      'Uruguay', 'Montevideo', '19 de abril', '102', '11000', '2026-06-21 00:27:38');

-- Documentos de identidad
INSERT INTO documento (usuario_numero_documento, tipo, pais) VALUES
    ('12345678', 'Cédula', 'Uruguay'),
    ('87654321', 'Cédula', 'Uruguay'),
    ('11223344', 'DNI', 'Argentina'),
    ('44332211', 'DNI', 'Argentina'),
    ('55667788', 'RG', 'Brasil'),
    ('88776655', 'RG', 'Brasil'),
    ('99887766', 'Cédula', 'Uruguay'),
    ('66778899', 'DNI', 'Argentina'),
    ('F0001234', 'Cédula', 'Uruguay'),
    ('F0005678', 'DNI', 'Argentina'),
    ('F0009012', 'RG', 'Brasil'),
    ('ADM001', 'Cédula', 'Uruguay'),
    ('ADM002', 'DNI', 'Argentina'),
    ('ADM003', 'RG', 'Brasil');

-- Teléfonos
INSERT INTO telefono (usuario_numero_documento, telefono) VALUES
    ('12345678', '+598 99123456'),
    ('12345678', '+598 24567890'),
    ('87654321', '+598 98765432'),
    ('11223344', '+54 1123456789'),
    ('44332211', '+54 3515551234'),
    ('55667788', '+55 11987654321'),
    ('88776655', '+55 2198765432'),
    ('99887766', '+598 94123456'),
    ('66778899', '+54 2214556789'),
    ('F0001234', '+598 99900001'),
    ('F0005678', '+54 1199900001'),
    ('F0009012', '+55 11999000001'),
    ('ADM001', '+598 24001111'),
    ('ADM002', '+54 1145001111'),
    ('ADM003', '+55 1133001111');

-- Estados de identidad para usuarios
INSERT INTO tiene_estado_identidad (usuario_numero_documento, id_estado_identidad) VALUES
    ('12345678', 1), ('87654321', 1), ('11223344', 1), ('44332211', 1),
    ('55667788', 1), ('88776655', 1), ('99887766', 1), ('66778899', 1),
    ('F0001234', 1), ('F0005678', 1), ('F0009012', 1),
    ('ADM001', 1), ('ADM002', 1), ('ADM003', 1);

-- Funcionarios
INSERT INTO funcionario (numero_documento, numero_legajo) VALUES
    ('F0001234', 'LEG-001'),
    ('F0005678', 'LEG-002'),
    ('F0009012', 'LEG-003'),
    ('11111111', 'LEG-004');

-- Dispositivos autorizados (lectores de QR, etc.)
INSERT INTO dispositivo_autorizado (nombre, numero_documento) VALUES
    ('Lector QR 001', 'F0001234'),
    ('Lector QR 002', 'F0001234'),
    ('Tablet Control 001', 'F0005678'),
    ('Lector QR 003', 'F0009012'),
    ('Scanner Supervisor', '11111111');

-- Administradores (asignados a sedes)
INSERT INTO administrador (numero_documento, nombre_sede) VALUES
    ('ADM001', 'Montevideo'),
    ('ADM002', 'Buenos Aires'),
    ('ADM003', 'São Paulo'),
    ('22222222', 'Montevideo');

-- ---------------------------------------------------------------------------
-- EVENTOS DEPORTIVOS
-- ---------------------------------------------------------------------------

-- Eventos (partidos de prueba)
INSERT INTO evento_deportivo (id_equipo_local, id_equipo_visitante, fecha, hora, cantidad_entradas) VALUES
    (1, 2, '2026-06-15', '19:00:00', 50000),   -- Uruguay vs Argentina
    (2, 3, '2026-06-16', '20:30:00', 55000),   -- Argentina vs Brasil
    (3, 1, '2026-06-17', '18:00:00', 45000),   -- Brasil vs Uruguay
    (4, 5, '2026-06-18', '19:00:00', 60000),   -- España vs Francia
    (6, 7, '2026-06-19', '20:00:00', 50000),   -- Alemania vs Italia
    (8, 1, '2026-06-20', '19:00:00', 48000),   -- Portugal vs Uruguay
    (2, 4, '2026-06-21', '20:00:00', 65000),   -- Argentina vs España
    (3, 6, '2026-06-22', '18:30:00', 52000);   -- Brasil vs Alemania

-- Información de entrada: habilitar sectores por evento
INSERT INTO informacion_entrada (id_sector, id_evento_deportivo, numero_documento_administrador) VALUES
    (1, 1, 'ADM001'), (2, 1, 'ADM001'), (3, 1, 'ADM001'), (4, 1, 'ADM001'),
    (5, 2, 'ADM002'), (6, 2, 'ADM002'), (7, 2, 'ADM002'),
    (8, 3, 'ADM002'), (9, 3, 'ADM002'), (10, 3, 'ADM002'),
    (11, 4, 'ADM002'), (12, 4, 'ADM002'), (13, 4, 'ADM002'), (14, 4, 'ADM002'),
    (15, 5, 'ADM003'), (16, 5, 'ADM003'), (17, 5, 'ADM003'), (18, 5, 'ADM003'),
    (1, 6, 'ADM001'), (2, 6, 'ADM001'),
    (8, 7, 'ADM002'), (9, 7, 'ADM002'), (10, 7, 'ADM002'),
    (15, 8, 'ADM003'), (16, 8, 'ADM003'), (17, 8, 'ADM003');

-- ---------------------------------------------------------------------------
-- VENTAS Y ENTRADAS
-- ---------------------------------------------------------------------------

-- Ventas (compras de entradas)
INSERT INTO venta (numero_documento_usuario, id_comision, monto_total) VALUES
    ('12345678', 1, 500.00),
    ('87654321', 2, 200.00),
    ('11223344', 1, 300.00),
    ('44332211', 3, 500.00),
    ('55667788', 2, 100.00),
    ('88776655', 1, 200.00),
    ('99887766', 2, 400.00),
    ('66778899', 1, 100.00),
    ('12345678', 2, 500.00),
    ('11223344', 3, 200.00);

-- Estados de venta (vincular cada venta con estado)
INSERT INTO venta_tiene_estado (id_venta, id_estado_venta) VALUES
    (1, 2), (2, 2), (3, 2), (4, 2), (5, 2),
    (6, 2), (7, 2), (8, 2), (9, 2), (10, 2);

-- Entradas (tickets de eventos)
-- Venta 1: 5 entradas para Uruguay vs Argentina (evento 1, sector 1) — comprador 12345678
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '5995', NULL, 500.00, 1, 1, 'ADM001', 1, '12345678'),
    ('Disponible', '2651', NULL, 500.00, 1, 1, 'ADM001', 1, '12345678'),
    ('Disponible', '3270', NULL, 500.00, 1, 1, 'ADM001', 1, '12345678'),
    ('Disponible', '7397', NULL, 500.00, 1, 1, 'ADM001', 1, '12345678'),
    ('Disponible', '8177', NULL, 500.00, 2, 1, 'ADM001', 1, '12345678');

-- Venta 2: 4 entradas para Argentina vs Brasil (evento 2, sector 5) — comprador 87654321
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '7990', NULL, 800.00, 5, 2, 'ADM002', 2, '87654321'),
    ('Disponible', '7364', NULL, 800.00, 5, 2, 'ADM002', 2, '87654321'),
    ('Disponible', '2850', NULL, 800.00, 6, 2, 'ADM002', 2, '87654321'),
    ('Disponible', '9160', NULL, 800.00, 6, 2, 'ADM002', 2, '87654321');

-- Venta 3: 3 entradas para Brasil vs Uruguay (evento 3, sector 8) — comprador 11223344
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '9250', NULL, 700.00, 8, 3, 'ADM002', 3, '11223344'),
    ('Disponible', '8772', NULL, 700.00, 8, 3, 'ADM002', 3, '11223344'),
    ('Disponible', '6107', NULL, 700.00, 9, 3, 'ADM002', 3, '11223344');

-- Venta 4: 5 entradas para España vs Francia (evento 4, sector 11-12) — comprador 44332211
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '3223', NULL, 900.00, 11, 4, 'ADM002', 4, '44332211'),
    ('Disponible', '5793', NULL, 900.00, 11, 4, 'ADM002', 4, '44332211'),
    ('Disponible', '9296', NULL, 900.00, 12, 4, 'ADM002', 4, '44332211'),
    ('Disponible', '1100', NULL, 900.00, 12, 4, 'ADM002', 4, '44332211'),
    ('Disponible', '3615', NULL, 900.00, 13, 4, 'ADM002', 4, '44332211');

-- Venta 5: 3 entradas para Alemania vs Italia (evento 5, sector 15) — comprador 55667788
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '4773', NULL, 620.00, 15, 5, 'ADM003', 5, '55667788'),
    ('Disponible', '3022', NULL, 620.00, 15, 5, 'ADM003', 5, '55667788'),
    ('Disponible', '8792', NULL, 620.00, 16, 5, 'ADM003', 5, '55667788');

-- Venta 6: 2 entradas para Portugal vs Uruguay (evento 6, sector 1-2) — comprador 88776655
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '6895', NULL, 550.00, 1, 6, 'ADM001', 6, '88776655'),
    ('Disponible', '7098', NULL, 550.00, 2, 6, 'ADM001', 6, '88776655');

-- Venta 7: 4 entradas para Argentina vs España (evento 7, sector 8) — comprador 99887766
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '4807', NULL, 750.00, 8, 7, 'ADM002', 7, '99887766'),
    ('Disponible', '1742', NULL, 750.00, 8, 7, 'ADM002', 7, '99887766'),
    ('Disponible', '2290', NULL, 750.00, 9, 7, 'ADM002', 7, '99887766'),
    ('Disponible', '5611', NULL, 750.00, 10, 7, 'ADM002', 7, '99887766');

-- Venta 8: 3 entradas para Brasil vs Alemania (evento 8, sector 15-16) — comprador 66778899
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '3344', NULL, 680.00, 15, 8, 'ADM003', 8, '66778899'),
    ('Disponible', '5566', NULL, 680.00, 16, 8, 'ADM003', 8, '66778899'),
    ('Disponible', '7788', NULL, 680.00, 17, 8, 'ADM003', 8, '66778899');

-- Venta 9: 5 entradas para Uruguay vs Argentina (evento 1, sector 3-4) — comprador 12345678
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '1122', NULL, 500.00, 3, 1, 'ADM001', 9, '12345678'),
    ('Disponible', '3344', NULL, 500.00, 3, 1, 'ADM001', 9, '12345678'),
    ('Disponible', '5566', NULL, 500.00, 4, 1, 'ADM001', 9, '12345678'),
    ('Disponible', '7788', NULL, 500.00, 4, 1, 'ADM001', 9, '12345678'),
    ('Disponible', '9900', NULL, 500.00, 4, 1, 'ADM001', 9, '12345678');

-- Venta 10: 6 entradas para Argentina vs Brasil (evento 2, sector 7) — comprador 11223344
INSERT INTO entrada (estado, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, numero_documento_propietario_actual) VALUES
    ('Disponible', '2185', NULL, 800.00, 7, 2, 'ADM002', 10, '11223344'),
    ('Disponible', '4396', NULL, 800.00, 7, 2, 'ADM002', 10, '11223344'),
    ('Disponible', '5617', NULL, 800.00, 7, 2, 'ADM002', 10, '11223344'),
    ('Disponible', '6828', NULL, 800.00, 7, 2, 'ADM002', 10, '11223344'),
    ('Disponible', '7949', NULL, 800.00, 7, 2, 'ADM002', 10, '11223344'),
    ('Disponible', '9060', NULL, 800.00, 7, 2, 'ADM002', 10, '11223344');

-- ---------------------------------------------------------------------------
-- TRANSFERENCIAS (ejemplos de transferencias de entradas)
-- ---------------------------------------------------------------------------

-- Transferencia 1: Usuario 12345678 transfiere entrada 1 a usuario 87654321
INSERT INTO transferencia (numero_documento_emisor, numero_documento_receptor, id_entrada) VALUES
    ('12345678', '87654321', 1);

-- Transferencia 2: Usuario 87654321 transfiere entrada 13 a usuario 11223344
INSERT INTO transferencia (numero_documento_emisor, numero_documento_receptor, id_entrada) VALUES
    ('87654321', '11223344', 13);

-- Transferencia 3: Usuario 11223344 transfiere entrada 20 a usuario 44332211
INSERT INTO transferencia (numero_documento_emisor, numero_documento_receptor, id_entrada) VALUES
    ('11223344', '44332211', 20);

-- Cada transferencia cambia el dueño actual de la entrada (lo mismo que hace el flujo en runtime).
UPDATE entrada SET numero_documento_propietario_actual = '87654321' WHERE id = 1;
UPDATE entrada SET numero_documento_propietario_actual = '11223344' WHERE id = 13;
UPDATE entrada SET numero_documento_propietario_actual = '44332211' WHERE id = 20;

-- Fin del seed
-- Total: 20 tablas con datos coherentes y respetando todas las FK.
