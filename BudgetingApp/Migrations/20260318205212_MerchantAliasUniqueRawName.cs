using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Migrations
{
    /// <inheritdoc />
    public partial class MerchantAliasUniqueRawName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MerchantAlias_RawName",
                table: "MerchantAlias",
                column: "RawName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MerchantAlias_RawName",
                table: "MerchantAlias");
        }
    }
}
