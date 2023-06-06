using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AT150732.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Iv",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenCreateTime",
                table: "AspNetUsers",
                newName: "ECDHPrivateKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ECDHPrivateKey",
                table: "AspNetUsers",
                newName: "RefreshTokenCreateTime");

            migrationBuilder.AddColumn<string>(
                name: "Iv",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }
    }
}
