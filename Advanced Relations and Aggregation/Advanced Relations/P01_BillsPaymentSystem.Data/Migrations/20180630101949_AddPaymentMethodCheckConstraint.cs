using Microsoft.EntityFrameworkCore.Migrations;

namespace P01_BillsPaymentSystem.Data.Migrations
{
    public partial class AddPaymentMethodCheckConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE PaymentMethods ADD CONSTRAINT Chk_PaymentMethods_CreditCardId_BankAccountId CHECK ((BankAccountId IS NOT NULL AND CreditCardId IS NULL) OR (BankAccountId IS NULL AND CreditCardId IS NOT NULL))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE PaymentMethod DROP CONSTRAINT Chk_PaymentMethods_CreditCardId_BankAccountId");
        }
    }
}
