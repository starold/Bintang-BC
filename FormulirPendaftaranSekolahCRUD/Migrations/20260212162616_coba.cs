using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FormulirPendaftaranSekolahCRUD.Migrations
{
    /// <inheritdoc />
    public partial class coba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kelass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaKelas = table.Column<string>(type: "TEXT", nullable: false),
                    Deskripsi = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kelass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaLengkap = table.Column<string>(type: "TEXT", nullable: false),
                    Alamat = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    TanggalLahir = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NamaOrangTua = table.Column<string>(type: "TEXT", nullable: false),
                    KelasId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Kelass_KelasId",
                        column: x => x.KelasId,
                        principalTable: "Kelass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Kelass",
                columns: new[] { "Id", "Deskripsi", "NamaKelas" },
                values: new object[,]
                {
                    { 1, "Ilmu Pengetahuan Alam", "IPA" },
                    { 2, "Ilmu Pengetahuan Sosial", "IPS" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_KelasId",
                table: "Students",
                column: "KelasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Kelass");
        }
    }
}
