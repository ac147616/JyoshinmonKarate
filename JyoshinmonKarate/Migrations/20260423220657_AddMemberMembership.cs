using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JyoshinmonKarate.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Memberships_MembershipId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_MembershipId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MembershipId",
                table: "Members");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoPath",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MemberMemberships",
                columns: table => new
                {
                    MemberMembershipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MembershipId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MembershipStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberMemberships", x => x.MemberMembershipId);
                    table.ForeignKey(
                        name: "FK_MemberMemberships_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberMemberships_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "MembershipId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberMemberships_MemberId",
                table: "MemberMemberships",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMemberships_MembershipId",
                table: "MemberMemberships",
                column: "MembershipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberMemberships");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoPath",
                table: "Members");

            migrationBuilder.AddColumn<int>(
                name: "MembershipId",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Members_MembershipId",
                table: "Members",
                column: "MembershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Memberships_MembershipId",
                table: "Members",
                column: "MembershipId",
                principalTable: "Memberships",
                principalColumn: "MembershipId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
