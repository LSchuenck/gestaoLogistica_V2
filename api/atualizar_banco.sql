-- Só é necessário para quem já tinha o banco criado antes destas mudanças.
-- Em um banco novo a API cria tudo sozinha.
-- Execute apenas o que ainda não existir no seu banco.

-- Troca de senha no primeiro acesso
ALTER TABLE Usuarios ADD COLUMN TrocarSenha tinyint(1) NOT NULL DEFAULT 0;

-- Viagens
CREATE TABLE IF NOT EXISTS Viagens (
    Id          int         NOT NULL AUTO_INCREMENT PRIMARY KEY,
    ArmazemId   int         NOT NULL,
    ClienteId   int         NOT NULL,
    VeiculoId   int         NOT NULL,
    MotoristaId int         NOT NULL,
    Status      longtext    NOT NULL,
    DataSaida   datetime(6) NOT NULL,
    DataChegada datetime(6) NULL,
    CONSTRAINT FK_Viagens_Armazens_ArmazemId FOREIGN KEY (ArmazemId) REFERENCES Armazens (Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Viagens_Clientes_ClienteId FOREIGN KEY (ClienteId) REFERENCES Clientes (Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Viagens_Veiculos_VeiculoId FOREIGN KEY (VeiculoId) REFERENCES Veiculos (Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Viagens_Motoristas_MotoristaId FOREIGN KEY (MotoristaId) REFERENCES Motoristas (Id) ON DELETE RESTRICT
);

-- Distância e valor do frete da viagem
ALTER TABLE Viagens ADD COLUMN DistanciaKm decimal(10,2) NOT NULL DEFAULT 0;
ALTER TABLE Viagens ADD COLUMN ValorPorKm decimal(10,2) NOT NULL DEFAULT 0;

-- Produto e quantidade da viagem
ALTER TABLE Viagens ADD COLUMN ProdutoId int NULL;
ALTER TABLE Viagens ADD COLUMN Quantidade int NOT NULL DEFAULT 0;
ALTER TABLE Viagens ADD CONSTRAINT FK_Viagens_Produtos_ProdutoId FOREIGN KEY (ProdutoId) REFERENCES Produtos (Id) ON DELETE RESTRICT;
