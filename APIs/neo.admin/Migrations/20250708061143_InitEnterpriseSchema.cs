using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace neo.admin.Migrations
{
    /// <inheritdoc />
    public partial class InitEnterpriseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_corporate",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_corporate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_faskes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NoFaskes = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CorporateId = table.Column<long>(type: "bigint", nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RegisteredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InitPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GracePeriod = table.Column<int>(type: "integer", nullable: true),
                    GracePenalty = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_faskes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_faskes_sys_corporate_CorporateId",
                        column: x => x.CorporateId,
                        principalTable: "sys_corporate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_login",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CorporateId = table.Column<long>(type: "bigint", nullable: true),
                    FaskesId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_login", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_login_sys_corporate_CorporateId",
                        column: x => x.CorporateId,
                        principalTable: "sys_corporate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_login_sys_faskes_FaskesId",
                        column: x => x.FaskesId,
                        principalTable: "sys_faskes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_connstring",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_id = table.Column<long>(type: "bigint", nullable: false),
                    db_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    db_host = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    db_username = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    db_password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_connstring", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_connstring_sys_login_login_id",
                        column: x => x.login_id,
                        principalTable: "sys_login",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sys_connstring_login_id",
                table: "sys_connstring",
                column: "login_id");

            migrationBuilder.CreateIndex(
                name: "IX_sys_corporate_Name",
                table: "sys_corporate",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_faskes_CorporateId",
                table: "sys_faskes",
                column: "CorporateId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_faskes_Name",
                table: "sys_faskes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_sys_faskes_NoFaskes",
                table: "sys_faskes",
                column: "NoFaskes",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_CorporateId",
                table: "sys_login",
                column: "CorporateId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_Email",
                table: "sys_login",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_FaskesId",
                table: "sys_login",
                column: "FaskesId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_Username",
                table: "sys_login",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_connstring");

            migrationBuilder.DropTable(
                name: "sys_login");

            migrationBuilder.DropTable(
                name: "sys_faskes");

            migrationBuilder.DropTable(
                name: "sys_corporate");
        }
    }
}
