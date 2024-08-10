using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeachPlanner.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConceptualOrganiserContentDescriptionRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_content_descriptions_conceptual_organisers_YearLevelId",
                table: "content_descriptions");

            migrationBuilder.RenameColumn(
                name: "YearLevelId",
                table: "content_descriptions",
                newName: "ConceptualOrganiserId");

            migrationBuilder.RenameIndex(
                name: "IX_content_descriptions_YearLevelId",
                table: "content_descriptions",
                newName: "IX_content_descriptions_ConceptualOrganiserId");

            migrationBuilder.AddForeignKey(
                name: "FK_content_descriptions_conceptual_organisers_ConceptualOrganis~",
                table: "content_descriptions",
                column: "ConceptualOrganiserId",
                principalTable: "conceptual_organisers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_content_descriptions_conceptual_organisers_ConceptualOrganis~",
                table: "content_descriptions");

            migrationBuilder.RenameColumn(
                name: "ConceptualOrganiserId",
                table: "content_descriptions",
                newName: "YearLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_content_descriptions_ConceptualOrganiserId",
                table: "content_descriptions",
                newName: "IX_content_descriptions_YearLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_content_descriptions_conceptual_organisers_YearLevelId",
                table: "content_descriptions",
                column: "YearLevelId",
                principalTable: "conceptual_organisers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
