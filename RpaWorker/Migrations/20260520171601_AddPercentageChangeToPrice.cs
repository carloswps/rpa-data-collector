using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpaWorker.Migrations
{
    /// <inheritdoc />
    public partial class AddPercentageChangeToPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PercentageChange",
                table: "prices",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentageChange",
                table: "prices");
        }
    }
}
