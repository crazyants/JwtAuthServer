﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace LegnicaIT.DataAccess.Migrations
{
    public partial class RoleTypeChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "UserApps",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Role",
                table: "UserApps",
                nullable: false);
        }
    }
}
