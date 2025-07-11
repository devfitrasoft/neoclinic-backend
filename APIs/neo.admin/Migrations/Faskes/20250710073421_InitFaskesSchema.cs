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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_module", x => x.Id);
                    table.UniqueConstraint("AK_sys_module_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "sys_role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user_faskes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoginId = table.Column<long>(type: "bigint", nullable: false),
                    FaskesId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_faskes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_group",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_group", x => x.Id);
                    table.UniqueConstraint("AK_sys_group_Code", x => x.Code);
                    table.ForeignKey(
                        name: "FK_sys_group_sys_module_ModuleCode",
                        column: x => x.ModuleCode,
                        principalTable: "sys_module",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoginId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    NIK = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PCareUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PCarePasswordEnc = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ICareUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ICarePasswordEnc = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AntrolUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AntrolPasswordEnc = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SSPracticionerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_user_sys_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "sys_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_menu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    GroupCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_menu", x => x.Id);
                    table.UniqueConstraint("AK_sys_menu_Code", x => x.Code);
                    table.ForeignKey(
                        name: "FK_sys_menu_sys_group_GroupCode",
                        column: x => x.GroupCode,
                        principalTable: "sys_group",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sys_menu_sys_module_ModuleCode",
                        column: x => x.ModuleCode,
                        principalTable: "sys_module",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ModuleCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    GroupCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    MenuCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    View = table.Column<bool>(type: "boolean", nullable: false),
                    Add = table.Column<bool>(type: "boolean", nullable: false),
                    Edit = table.Column<bool>(type: "boolean", nullable: false),
                    Delete = table.Column<bool>(type: "boolean", nullable: false),
                    Print = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdaterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_auth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_auth_sys_group_GroupCode",
                        column: x => x.GroupCode,
                        principalTable: "sys_group",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sys_auth_sys_menu_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "sys_menu",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sys_auth_sys_module_ModuleCode",
                        column: x => x.ModuleCode,
                        principalTable: "sys_module",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sys_auth_sys_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "sys_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sys_auth_GroupCode",
                table: "sys_auth",
                column: "GroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_sys_auth_MenuCode",
                table: "sys_auth",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_sys_auth_ModuleCode",
                table: "sys_auth",
                column: "ModuleCode");

            migrationBuilder.CreateIndex(
                name: "IX_sys_auth_RoleId_ModuleCode_GroupCode_MenuCode",
                table: "sys_auth",
                columns: new[] { "RoleId", "ModuleCode", "GroupCode", "MenuCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_group_Code",
                table: "sys_group",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_group_ModuleCode",
                table: "sys_group",
                column: "ModuleCode");

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_Code",
                table: "sys_menu",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_GroupCode",
                table: "sys_menu",
                column: "GroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_ModuleCode",
                table: "sys_menu",
                column: "ModuleCode");

            migrationBuilder.CreateIndex(
                name: "IX_sys_module_Code",
                table: "sys_module",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_role_Name",
                table: "sys_role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_RoleId",
                table: "sys_user",
                column: "RoleId");
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
