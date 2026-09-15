using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JyoshinmonKarate.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGradingLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Passed",
                table: "MemberGradings");

            migrationBuilder.AlterColumn<int>(
                name: "BeltAfterId",
                table: "MemberGradings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MemberGradings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SessionName",
                table: "Gradings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MemberGradings");

            migrationBuilder.DropColumn(
                name: "SessionName",
                table: "Gradings");

            migrationBuilder.AlterColumn<int>(
                name: "BeltAfterId",
                table: "MemberGradings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Passed",
                table: "MemberGradings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
