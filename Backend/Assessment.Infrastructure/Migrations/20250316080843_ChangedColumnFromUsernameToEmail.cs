using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedColumnFromUsernameToEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8f0413f9-53ab-40cf-89f7-573ef6c5893e"));

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Email");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "Role" },
                values: new object[] { new Guid("9990ffbf-d616-44b5-8fae-38c22e2328b2"), "admin@gmail.com", "$2a$11$E26dABEdL3bVEXshiNnX8OmuylB1jzLyPkN96inzRGc6K7XbcHlmK", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9990ffbf-d616-44b5-8fae-38c22e2328b2"));

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "Username");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "Role", "Username" },
                values: new object[] { new Guid("8f0413f9-53ab-40cf-89f7-573ef6c5893e"), "$2a$11$gFp/POcYpQFcx10M8wBtV.owRIAbmd1HshXtOnO19RuaWA0chEk.C", "Admin", "admin" });
        }
    }
}
