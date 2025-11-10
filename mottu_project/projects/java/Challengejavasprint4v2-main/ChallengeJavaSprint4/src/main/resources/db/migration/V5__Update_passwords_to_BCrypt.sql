-- Atualiza as senhas para o formato BCrypt, compatível com BCryptPasswordEncoder

UPDATE usuario SET password = '$2b$12$2bbPzKqFu5t7qRx2syagguatLwQnZqiSdrG2KaA8e4cvNGrgOFtve' WHERE username = 'user';
UPDATE usuario SET password = '$2b$12$MYdawlI8zR2beenNBLFRKe4HBRCR3cc8UFlR1fVEP51pFZsfMt4fe' WHERE username = 'admin';
