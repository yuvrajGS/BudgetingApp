using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIncomeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Keywords", "Name" },
                values: new object[] { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Payroll, salary, wages, direct deposits, tax refunds, government benefits, and other income", "payroll,pay,salary,wages,direct deposit,paycheque,paycheck,deposit,employer,payroll deposit,income,pension,cpp,oas,ei,cra refund,tax refund,bonus,commission,interest,dividend", "Income" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
