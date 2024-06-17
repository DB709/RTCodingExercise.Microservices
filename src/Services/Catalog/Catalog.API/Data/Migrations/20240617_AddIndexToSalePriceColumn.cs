namespace Catalog.API.Data.Migrations
{
    public partial class AddIndexToSalePriceColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Plates_SalePrice",
                table: "Plates",
                column: "SalePrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plates_SalePrice",
                table: "Plates");
        }
    }
}