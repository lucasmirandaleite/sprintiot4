using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MottuRFID.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FILIAIS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    CIDADE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PAIS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CODIGO_FILIAL = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILIAIS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PONTOS_LEITURA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    LOCALIZACAO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    FILIAL_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    POSICAO_X = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    POSICAO_Y = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ATIVO = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PONTOS_LEITURA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PONTOS_LEITURA_FILIAIS_FILIAL_ID",
                        column: x => x.FILIAL_ID,
                        principalTable: "FILIAIS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MOTOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PLACA = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    MODELO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    COR = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NUMERO_SERIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    TAG_RFID = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FILIAL_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PONTO_LEITURA_ATUAL_ID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ULTIMA_ATUALIZACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ATIVA = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTOS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MOTOS_FILIAIS_FILIAL_ID",
                        column: x => x.FILIAL_ID,
                        principalTable: "FILIAIS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MOTOS_PONTOS_LEITURA_PONTO_LEITURA_ATUAL_ID",
                        column: x => x.PONTO_LEITURA_ATUAL_ID,
                        principalTable: "PONTOS_LEITURA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LEITURAS_RFID",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TAG_RFID = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    MOTO_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PONTO_LEITURA_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_HORA_LEITURA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OBSERVACAO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEITURAS_RFID", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LEITURAS_RFID_MOTOS_MOTO_ID",
                        column: x => x.MOTO_ID,
                        principalTable: "MOTOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LEITURAS_RFID_PONTOS_LEITURA_PONTO_LEITURA_ID",
                        column: x => x.PONTO_LEITURA_ID,
                        principalTable: "PONTOS_LEITURA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LEITURAS_RFID_MOTO_ID",
                table: "LEITURAS_RFID",
                column: "MOTO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURAS_RFID_PONTO_LEITURA_ID",
                table: "LEITURAS_RFID",
                column: "PONTO_LEITURA_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MOTOS_FILIAL_ID",
                table: "MOTOS",
                column: "FILIAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MOTOS_PONTO_LEITURA_ATUAL_ID",
                table: "MOTOS",
                column: "PONTO_LEITURA_ATUAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PONTOS_LEITURA_FILIAL_ID",
                table: "PONTOS_LEITURA",
                column: "FILIAL_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LEITURAS_RFID");

            migrationBuilder.DropTable(
                name: "MOTOS");

            migrationBuilder.DropTable(
                name: "PONTOS_LEITURA");

            migrationBuilder.DropTable(
                name: "FILIAIS");
        }
    }
}
