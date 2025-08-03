using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace neo.admin.Migrations.Enterprise
{
    /// <inheritdoc />
    public partial class InitEnterpriseAdminSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_auth_session",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_id = table.Column<long>(type: "bigint", nullable: false),
                    refresh_token_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    device_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    issued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_active_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_auth_session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_billing_setting",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    default_grace_period_months = table.Column<int>(type: "integer", nullable: false),
                    default_grace_penalty = table.Column<decimal>(type: "numeric", nullable: false),
                    registration_fee = table.Column<decimal>(type: "numeric", nullable: false),
                    transaction_price_per_unit = table.Column<decimal>(type: "numeric", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_billing_setting", x => x.id);
                });

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
                    npwp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    corporate_id = table.Column<long>(type: "bigint", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    registered_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                name: "sys_billing",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    faskes_id = table.Column<long>(type: "bigint", nullable: false),
                    billing_type = table.Column<int>(type: "integer", nullable: false),
                    period_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    period_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    suspension_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    grace_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_paid = table.Column<bool>(type: "boolean", nullable: false),
                    payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    transaction_count = table.Column<long>(type: "bigint", nullable: true),
                    amount_due = table.Column<decimal>(type: "numeric", nullable: true),
                    grace_penalty = table.Column<decimal>(type: "numeric", nullable: true),
                    sum_grace_penalty = table.Column<decimal>(type: "numeric", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_billing", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_billing_sys_faskes_faskes_id",
                        column: x => x.faskes_id,
                        principalTable: "sys_faskes",
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
                name: "sys_pic",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    faskes_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    pic_type = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_pic", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_pic_sys_faskes_faskes_id",
                        column: x => x.faskes_id,
                        principalTable: "sys_faskes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_sys_billing_faskes_id",
                table: "sys_billing",
                column: "faskes_id");

            migrationBuilder.CreateIndex(
                name: "IX_sys_billing_setting_is_active",
                table: "sys_billing_setting",
                column: "is_active");

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

            migrationBuilder.CreateIndex(
                name: "IX_sys_pic_faskes_id",
                table: "sys_pic",
                column: "faskes_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_auth_session");

            migrationBuilder.DropTable(
                name: "sys_billing");

            migrationBuilder.DropTable(
                name: "sys_billing_setting");

            migrationBuilder.DropTable(
                name: "sys_connstring");

            migrationBuilder.DropTable(
                name: "sys_pic");

            migrationBuilder.DropTable(
                name: "sys_login");

            migrationBuilder.DropTable(
                name: "sys_faskes");

            migrationBuilder.DropTable(
                name: "sys_corporate");
        }
    }
}
