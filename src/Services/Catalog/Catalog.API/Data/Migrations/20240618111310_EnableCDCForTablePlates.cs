using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Migrations
{
    public partial class EnableCDCForTablePlates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"EXEC sys.sp_cdc_enable_db;");

            migrationBuilder.Sql(@"
            EXEC sys.sp_cdc_enable_table 
            @source_schema = 'dbo', 
            @source_name = 'Plates', 
            @role_name = NULL, 
            @supports_net_changes = 1;
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"EXEC sys.sp_cdc_disable_db;");

            migrationBuilder.Sql(@"
            EXEC sys.sp_cdc_disable_table 
            @source_schema = 'dbo', 
            @source_name = 'Plates', 
            @capture_instance = 'dbo_Plates';
        ");
        }
    }
}
