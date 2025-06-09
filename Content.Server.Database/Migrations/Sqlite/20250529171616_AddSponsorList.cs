using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddSponsorList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sponsors_list",
                columns: table => new
                {
                    player_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    extra_char_slots = table.Column<int>(type: "INTEGER", nullable: false),
                    server_priority_join = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsors_list", x => x.player_user_id);
                    table.ForeignKey(
                        name: "FK_sponsors_list_player_player_user_id",
                        column: x => x.player_user_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sponsors_prototypes",
                columns: table => new
                {
                    sponsors_prototypes_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    prototype = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    player_user_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsors_prototypes", x => x.sponsors_prototypes_id);
                    table.ForeignKey(
                        name: "FK_sponsors_prototypes_player_player_id",
                        column: x => x.player_user_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sponsors_prototypes_player_user_id",
                table: "sponsors_prototypes",
                column: "player_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sponsors_list");

            migrationBuilder.DropTable(
                name: "sponsors_prototypes");
        }
    }
}
