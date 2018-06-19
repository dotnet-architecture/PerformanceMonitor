using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PerfMonitor.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CPU_Data",
                columns: table => new
                {
                    CPU_Usage = table.Column<float>(nullable: false),
                    date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPU_Data", x => x.date);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CPU_Data");
        }
    }
}
