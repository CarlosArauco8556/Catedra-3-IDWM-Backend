using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catedra3IDWMBackend.src.Data.Migrations
{
    /// <inheritdoc />
    public partial class addRoleUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c7afcb2f-5ba5-4f46-a98b-08c30f75fd31", null, "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c7afcb2f-5ba5-4f46-a98b-08c30f75fd31");
        }
    }
}
