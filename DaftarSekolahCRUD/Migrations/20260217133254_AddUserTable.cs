using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DaftarSekolahCRUD.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_ClassRooms_ClassRoomId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));

            migrationBuilder.DeleteData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"));

            migrationBuilder.DeleteData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"));

            migrationBuilder.DeleteData(
                table: "Extracurriculars",
                keyColumn: "Id",
                keyValue: new Guid("a7b8c9d0-e1f2-3456-abcd-567890123456"));

            migrationBuilder.DeleteData(
                table: "Extracurriculars",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"));

            migrationBuilder.DeleteData(
                table: "Extracurriculars",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-efab-345678901234"));

            migrationBuilder.DeleteData(
                table: "Extracurriculars",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345"));

            migrationBuilder.AddForeignKey(
                name: "FK_Students_ClassRooms_ClassRoomId",
                table: "Students",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_ClassRooms_ClassRoomId",
                table: "Students");

            migrationBuilder.InsertData(
                table: "ClassRooms",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "X IPA" },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), "X IPS" },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), "X Bahasa" }
                });

            migrationBuilder.InsertData(
                table: "Extracurriculars",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a7b8c9d0-e1f2-3456-abcd-567890123456"), "Robotics" },
                    { new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"), "Basket" },
                    { new Guid("e5f6a7b8-c9d0-1234-efab-345678901234"), "Futsal" },
                    { new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345"), "Pramuka" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_ClassRooms_ClassRoomId",
                table: "Students",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
