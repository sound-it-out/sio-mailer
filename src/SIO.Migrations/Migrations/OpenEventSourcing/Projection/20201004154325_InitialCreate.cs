using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SIO.Migrations.Migrations.OpenEventSourcing.Projection
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailFailure",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmailId = table.Column<Guid>(nullable: false),
                    Error = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailFailure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Attempts = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Template = table.Column<string>(nullable: true),
                    Payload = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    RecipientId = table.Column<Guid>(nullable: false),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionState",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Position = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionState", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    PasswordToken = table.Column<string>(nullable: true),
                    ActivationToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailFailure_EmailId",
                table: "EmailFailure",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailFailure");

            migrationBuilder.DropTable(
                name: "EmailQueue");

            migrationBuilder.DropTable(
                name: "ProjectionState");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
