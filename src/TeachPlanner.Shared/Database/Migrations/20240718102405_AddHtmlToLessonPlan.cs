using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeachPlanner.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddHtmlToLessonPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanningNotesHtml",
                table: "lesson_plans",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanningNotesHtml",
                table: "lesson_plans");
        }
    }
}
