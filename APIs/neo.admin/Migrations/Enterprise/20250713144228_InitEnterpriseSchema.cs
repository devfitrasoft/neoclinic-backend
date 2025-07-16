using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace neo.admin.Migrations.Enterprise
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
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_corporate", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_faskes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    no_faskes = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    corporate_id = table.Column<long>(type: "bigint", nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    registered_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    init_payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expired_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    grace_period = table.Column<int>(type: "integer", nullable: true),
                    grace_penalty = table.Column<decimal>(type: "numeric", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_faskes", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_faskes_sys_corporate_corporate_id",
                        column: x => x.corporate_id,
                        principalTable: "sys_corporate",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_login",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    corporate_id = table.Column<long>(type: "bigint", nullable: true),
                    faskes_id = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_login", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_login_sys_corporate_corporate_id",
                        column: x => x.corporate_id,
                        principalTable: "sys_corporate",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_login_sys_faskes_faskes_id",
                        column: x => x.faskes_id,
                        principalTable: "sys_faskes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_connstring",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_id = table.Column<long>(type: "bigint", nullable: false),
                    db_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    db_host = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    db_username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    db_password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_connstring", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_connstring_sys_login_login_id",
                        column: x => x.login_id,
                        principalTable: "sys_login",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sys_connstring_login_id",
                table: "sys_connstring",
                column: "login_id");

            migrationBuilder.CreateIndex(
                name: "IX_sys_corporate_name",
                table: "sys_corporate",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_faskes_corporate_id",
                table: "sys_faskes",
                column: "corporate_id");

            migrationBuilder.CreateIndex(
                name: "IX_sys_faskes_name",
                table: "sys_faskes",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_sys_faskes_no_faskes",
                table: "sys_faskes",
                column: "no_faskes",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_corporate_id",
                table: "sys_login",
                column: "corporate_id");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_email",
                table: "sys_login",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_faskes_id",
                table: "sys_login",
                column: "faskes_id");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_username",
                table: "sys_login",
                column: "username",
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
