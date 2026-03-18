using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameMerchantAliasCleanNameToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CleanName",
                table: "MerchantAlias",
                newName: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "MerchantAlias",
                newName: "CleanName");
        }
    }
}
