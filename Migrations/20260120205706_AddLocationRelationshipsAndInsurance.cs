using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarReservationSystemApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationRelationshipsAndInsurance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Locations_LocationId",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Cars",
                newName: "CurrentLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Cars_LocationId",
                table: "Cars",
                newName: "IX_Cars_CurrentLocationId");

            migrationBuilder.AddColumn<int>(
                name: "DropoffLocationId",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InsurancePolicyId",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PickupLocationId",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Reservations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "Cars",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Transmission",
                table: "Cars",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InsurancePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PolicyNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PolicyType = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PricePerDay = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "InsurancePolicies",
                columns: new[] { "Id", "Description", "IsActive", "PolicyNumber", "PolicyType", "PricePerDay" },
                values: new object[,]
                {
                    { 1, "Podstawowe ubezpieczenie OC. Pokrywa szkody wyrządzone osobom trzecim.", true, "INS-BASIC-001", "Basic", 15.00m },
                    { 2, "Standardowe ubezpieczenie OC + AC. Pokrywa szkody własne i osobom trzecim.", true, "INS-STANDARD-001", "Standard", 35.00m },
                    { 3, "Pełne ubezpieczenie OC + AC + NNW. Maksymalna ochrona bez udziału własnego.", true, "INS-PREMIUM-001", "Premium", 60.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_DropoffLocationId",
                table: "Reservations",
                column: "DropoffLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_InsurancePolicyId",
                table: "Reservations",
                column: "InsurancePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PickupLocationId",
                table: "Reservations",
                column: "PickupLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Locations_CurrentLocationId",
                table: "Cars",
                column: "CurrentLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_InsurancePolicies_InsurancePolicyId",
                table: "Reservations",
                column: "InsurancePolicyId",
                principalTable: "InsurancePolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Locations_DropoffLocationId",
                table: "Reservations",
                column: "DropoffLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Locations_PickupLocationId",
                table: "Reservations",
                column: "PickupLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Locations_CurrentLocationId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_UserId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_InsurancePolicies_InsurancePolicyId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Locations_DropoffLocationId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Locations_PickupLocationId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "InsurancePolicies");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_DropoffLocationId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_InsurancePolicyId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_PickupLocationId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DropoffLocationId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "InsurancePolicyId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PickupLocationId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Transmission",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "CurrentLocationId",
                table: "Cars",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Cars_CurrentLocationId",
                table: "Cars",
                newName: "IX_Cars_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Locations_LocationId",
                table: "Cars",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
