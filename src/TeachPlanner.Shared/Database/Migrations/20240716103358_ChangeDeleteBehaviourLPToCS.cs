using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeachPlanner.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeleteBehaviourLPToCS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lesson_plans_curriculum_subjects_SubjectId",
                table: "lesson_plans");

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_plans_curriculum_subjects_SubjectId",
                table: "lesson_plans",
                column: "SubjectId",
                principalTable: "curriculum_subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lesson_plans_curriculum_subjects_SubjectId",
                table: "lesson_plans");

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_plans_curriculum_subjects_SubjectId",
                table: "lesson_plans",
                column: "SubjectId",
                principalTable: "curriculum_subjects",
                principalColumn: "Id");
        }
    }
}
