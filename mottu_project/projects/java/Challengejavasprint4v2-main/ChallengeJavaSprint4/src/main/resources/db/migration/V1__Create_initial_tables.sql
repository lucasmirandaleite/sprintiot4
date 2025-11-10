CREATE TABLE filial (
    id SERIAL,
    nome VARCHAR(255) NOT NULL,
    endereco VARCHAR(255),
    PRIMARY KEY (id)
);

CREATE TABLE moto (
    id SERIAL,
    marca VARCHAR(255) NOT NULL,
    modelo VARCHAR(255) NOT NULL,
    ano INT,
    filial_id INT,
    PRIMARY KEY (id),
    FOREIGN KEY (filial_id) REFERENCES filial(id)
);

CREATE TABLE registro_rfid (
    id SERIAL,
    codigo_rfid VARCHAR(255) NOT NULL UNIQUE,
    data_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    moto_id INT,
    PRIMARY KEY (id),
    FOREIGN KEY (moto_id) REFERENCES moto(id)
);

