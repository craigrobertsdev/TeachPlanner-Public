using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeachPlanner.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeleteBehaviourLPToYD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lesson_plans_yeardata_YearDataId",
                table: "lesson_plans");

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_plans_yeardata_YearDataId",
                table: "lesson_plans",
                column: "YearDataId",
                principalTable: "yeardata",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lesson_plans_yeardata_YearDataId",
                table: "lesson_plans");

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_plans_yeardata_YearDataId",
                table: "lesson_plans",
                column: "YearDataId",
                principalTable: "yeardata",
                principalColumn: "Id");
        }
    }
}
