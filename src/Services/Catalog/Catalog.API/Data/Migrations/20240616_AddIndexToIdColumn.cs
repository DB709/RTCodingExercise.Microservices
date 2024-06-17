namespace Catalog.API.Data.Migrations
{
    public partial class AddIndexToIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Plates_Id",
                table: "Plates",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plates_Id",
                table: "Plates");
        }
    }
}