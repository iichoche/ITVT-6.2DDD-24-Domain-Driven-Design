using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BCImplementatie.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gebruiken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ZorgtechnologieProductItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InGebruik = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruiken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NeedCategories",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeedCategories", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Ervaringen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GebruikId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Observatie = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ervaringen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ervaringen_Gebruiken_GebruikId",
                        column: x => x.GebruikId,
                        principalTable: "Gebruiken",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CareNeeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GebruikId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    NeedDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NeedCategoryName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdviesZorgtechnologieProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareNeeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareNeeds_Gebruiken_GebruikId",
                        column: x => x.GebruikId,
                        principalTable: "Gebruiken",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CareNeeds_NeedCategories_NeedCategoryName",
                        column: x => x.NeedCategoryName,
                        principalTable: "NeedCategories",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "NeedCategories",
                columns: new[] { "Name", "Description" },
                values: new object[,]
                {
                    { "Gezondheid", "Gezondheidszorg gerelateerde behoeften" },
                    { "Onderwijs", "Onderwijs gerelateerde behoeften" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CareNeeds_GebruikId",
                table: "CareNeeds",
                column: "GebruikId");

            migrationBuilder.CreateIndex(
                name: "IX_CareNeeds_NeedCategoryName",
                table: "CareNeeds",
                column: "NeedCategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_Ervaringen_GebruikId",
                table: "Ervaringen",
                column: "GebruikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareNeeds");

            migrationBuilder.DropTable(
                name: "Ervaringen");

            migrationBuilder.DropTable(
                name: "NeedCategories");

            migrationBuilder.DropTable(
                name: "Gebruiken");
        }
    }
}
