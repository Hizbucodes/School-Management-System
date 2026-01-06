using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.API.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class Initialauthdbmirgration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e38fd539-efda-453b-9f5f-9c9b1a384f45",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f66a638-ae0d-4811-94f3-0c0c9364129a", "AQAAAAIAAYagAAAAEJl/I9qmNbMFKrCiFEiU8aIB4KLxC6UBu+xHuQuY2KyYM/S6TGtcz9/CuFg5T7qWUA==", "114e9c67-2612-4752-88ff-c2330bf9dee1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e38fd539-efda-453b-9f5f-9c9b1a384f45",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "69783f3e-ffa9-45f6-a5eb-0393ce1c0da3", "AQAAAAIAAYagAAAAEAwEbympUkU1bOOC+6If2A4xJlVMAczj3DXymVlQOSBYZ1V2OW7dGw5JiiSbkjoFhA==", "d0cacda6-66ec-4901-8330-1354df278904" });
        }
    }
}
