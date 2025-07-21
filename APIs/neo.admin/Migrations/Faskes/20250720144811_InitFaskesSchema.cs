using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace neo.admin.Migrations.Faskes
{
    /// <inheritdoc />
    public partial class InitFaskesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_module",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_module", x => x.id);
                    table.UniqueConstraint("AK_sys_module_code", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "sys_role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user_faskes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_id = table.Column<long>(type: "bigint", nullable: false),
                    faskes_id = table.Column<long>(type: "bigint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_faskes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_group",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_group", x => x.id);
                    table.UniqueConstraint("AK_sys_group_code", x => x.code);
                    table.ForeignKey(
                        name: "FK_sys_group_sys_module_module_code",
                        column: x => x.module_code,
                        principalTable: "sys_module",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    nik = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    pcare_username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    pcare_password_enc = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    icare_username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    icare_password_enc = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    antrol_username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    antrol_password_enc = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ss_practicioner_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_user_sys_role_role_id",
                        column: x => x.role_id,
                        principalTable: "sys_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_menu",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    group_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_menu", x => x.id);
                    table.UniqueConstraint("AK_sys_menu_code", x => x.code);
                    table.ForeignKey(
                        name: "FK_sys_menu_sys_group_group_code",
                        column: x => x.group_code,
                        principalTable: "sys_group",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_menu_sys_module_module_code",
                        column: x => x.module_code,
                        principalTable: "sys_module",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    module_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    group_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    menu_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    view = table.Column<bool>(type: "boolean", nullable: false),
                    add = table.Column<bool>(type: "boolean", nullable: false),
                    edit = table.Column<bool>(type: "boolean", nullable: false),
                    delete = table.Column<bool>(type: "boolean", nullable: false),
                    print = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updater_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_auth", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_auth_sys_group_group_code",
                        column: x => x.group_code,
                        principalTable: "sys_group",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_auth_sys_menu_menu_code",
                        column: x => x.menu_code,
                        principalTable: "sys_menu",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_auth_sys_module_module_code",
                        column: x => x.module_code,
                        principalTable: "sys_module",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sys_auth_sys_role_role_id",
                        column: x => x.role_id,
                        principalTable: "sys_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sys_auth_group_code",
                table: "sys_auth",
                column: "group_code");

            migrationBuilder.CreateIndex(
                name: "IX_sys_auth_menu_code",
                table: "sys_auth",
                column: "menu_code");

            migrationBuilder.CreateIndex(
                name: "IX_sys_auth_module_code",
                table: "sys_auth",
                column: "module_code");

            migrationBuilder.CreateIndex(
                name: "IX_sys_auth_role_id_module_code_group_code_menu_code",
                table: "sys_auth",
                columns: new[] { "role_id", "module_code", "group_code", "menu_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_group_code",
                table: "sys_group",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_group_module_code",
                table: "sys_group",
                column: "module_code");

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_code",
                table: "sys_menu",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_group_code",
                table: "sys_menu",
                column: "group_code");

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_module_code",
                table: "sys_menu",
                column: "module_code");

            migrationBuilder.CreateIndex(
                name: "IX_sys_module_code",
                table: "sys_module",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_role_name",
                table: "sys_role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_role_id",
                table: "sys_user",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_auth");

            migrationBuilder.DropTable(
                name: "sys_user");

            migrationBuilder.DropTable(
                name: "sys_user_faskes");

            migrationBuilder.DropTable(
                name: "sys_menu");

            migrationBuilder.DropTable(
                name: "sys_role");

            migrationBuilder.DropTable(
                name: "sys_group");

            migrationBuilder.DropTable(
                name: "sys_module");
        }
    }
}
