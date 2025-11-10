-- Script de inicialização de dados para desenvolvimento
-- Este script será executado automaticamente no perfil dev com H2

-- Criar tabela de usuários para Spring Security
CREATE TABLE IF NOT EXISTS usuario (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    role VARCHAR(20) NOT NULL
);

-- Inserir usuários de teste
-- Usuário: admin | Senha: admin123
-- Usuário: user  | Senha: user123
INSERT INTO usuario (username, password, role) VALUES 
('admin', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'ROLE_ADMIN');

INSERT INTO usuario (username, password, role) VALUES 
('user', '$2a$10$7PtcjEnWb/ZkgyXyxY0C3O6XcjmJYQYJJfgDxBcXXJgQqJqGNPPPa', 'ROLE_USER');
