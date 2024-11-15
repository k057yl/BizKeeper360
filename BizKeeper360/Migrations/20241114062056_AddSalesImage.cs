using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizKeeper360.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemImagePath",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemImagePath",
                table: "Sales");
        }
    }
}
