using TeachPlanner.Api.Features.Account;
using TeachPlanner.Api.Features.Authentication;
using TeachPlanner.Api.Features.Curriculum;
using TeachPlanner.Api.Features.Dev;
using TeachPlanner.Api.Features.LessonPlans;
using TeachPlanner.Api.Features.PlannerTemplates;
using TeachPlanner.Api.Features.Services;
using TeachPlanner.Api.Features.Subjects;
using TeachPlanner.Api.Features.Teachers;
using TeachPlanner.Api.Features.TermPlanners;
using TeachPlanner.Api.Features.WeekPlanners;
using TeachPlanner.Api.Features.YearDataRecords;

namespace TeachPlanner.Api;

public static class RouteMapper
{
    public static void MapApi(this WebApplication app)
    {
        var authReqGroup = app.MapGroup("/api/{teacherId}");
        var noAuthGroup = app.MapGroup("/api");
        var devGroup = app.MapGroup("/api/dev");

        noAuthGroup
            .MapAuth()
            .MapServices();

        authReqGroup
            .MapAssessments()
            .MapCurriculum()
            .MapLessonPlans()
            .MapStudents()
            .MapSubjects()
            .MapTeachers()
            .MapTemplates()
            .MapTermPlanners()
            .MapWeekPlanners()
            .MapYearData()
            .RequireAuthorization();

        devGroup.MapDevelopmentEndpoints();
    }

    private static RouteGroupBuilder MapAuth(this RouteGroupBuilder group)
    {
        var authGroup = group.MapGroup("/authentication");
        authGroup.MapPost("/register", Register.Delegate);
        authGroup.MapPost("/login", Login.Delegate);
        authGroup.MapPost("/refresh", Refresh.Delegate);
        authGroup.MapPost("/revoke", Revoke.Delegate);

        return group;
    }


    private static RouteGroupBuilder MapAssessments(this RouteGroupBuilder group)
    {
        var assessmentGroup = group.MapGroup("/assessments");
        return group;
    }

    private static RouteGroupBuilder MapCurriculum(this RouteGroupBuilder group)
    {
        var curriculumGroup = group.MapGroup("/curriculum");
        curriculumGroup.MapGet("/subject-names", GetCurriculumSubjectNames.Delegate);
        curriculumGroup.MapGet("/content-descriptions", GetContentDescriptions.Delegate);
        curriculumGroup.MapGet("/yearLevels", GetYearLevels.Delegate);
        return group;
    }

    private static RouteGroupBuilder MapLessonPlans(this RouteGroupBuilder group)
    {
        var lessonPlanGroup = group.MapGroup("/lesson-plans");
        lessonPlanGroup.MapGet("/", GetLessonPlan.Delegate);
        lessonPlanGroup.MapPost("/", CreateLessonPlan.Delegate);
        lessonPlanGroup.MapGet("/check-overlap", CheckLessonOverlap.Delegate);

        return group;
    }

    private static RouteGroupBuilder MapServices(this RouteGroupBuilder group)
    {
        var serviceGroup = group.MapGroup("/services");
        serviceGroup.MapPost("/term-dates", SetTermDates.Delegate);
        serviceGroup.MapGet("/term-dates", GetTermDates.Delegate);
        serviceGroup.MapPost("/parse-curriculum", ParseCurriculum.Delegate);

        return serviceGroup;
    }

    private static RouteGroupBuilder MapStudents(this RouteGroupBuilder group)
    {
        var studentGroup = group.MapGroup("/students");
        return group;
    }

    private static RouteGroupBuilder MapSubjects(this RouteGroupBuilder group)
    {
        var subjectGroup = group.MapGroup("/subjects");
        subjectGroup.MapGet("/curriculum", GetCurriculumSubjects.Delegate);

        return group;
    }

    private static RouteGroupBuilder MapTeachers(this RouteGroupBuilder group)
    {
        var teacherGroup = group.MapGroup("");
        teacherGroup.MapGet("/check-account-setup", GetAccountSetupStatus.Delegate);
        teacherGroup.MapGet("/settings", GetTeacherSettings.Delegate);
        teacherGroup.MapGet("/resources/{subjectId}", GetResources.Delegate);
        teacherGroup.MapGet("/year-levels-taught", GetYearLevelsTaught.Delegate);
        teacherGroup.MapPost("/resources", CreateResource.Delegate);
        teacherGroup.MapPost("/setup", AccountSetup.Delegate);
        teacherGroup.MapDelete("/", DeleteAccount.Delegate);

        return group;
    }

    private static RouteGroupBuilder MapTemplates(this RouteGroupBuilder group)
    {
        var templateGroup = group.MapGroup("/templates");
        templateGroup.MapPatch("/", UpdateWeekStructure.Delegate);
        templateGroup.MapGet("/create", GetLessonTemplateCreatorData.Delegate);

        return group;
    }

    private static RouteGroupBuilder MapTermPlanners(this RouteGroupBuilder group)
    {
        var termPlannerGroup = group.MapGroup("/term-planners");
        termPlannerGroup.MapPost("/", CreateTermPlanner.Delegate);
        termPlannerGroup.MapGet("/", GetTermPlanner.Delegate);
        return group;
    }

    private static RouteGroupBuilder MapWeekPlanners(this RouteGroupBuilder group)
    {
        var weekPlannerGroup = group.MapGroup("/week-planner");
        weekPlannerGroup.MapGet("/", GetWeekPlanner.Delegate);
        weekPlannerGroup.MapPost("/", CreateWeekPlanner.Delegate);
        return group;
    }

    private static RouteGroupBuilder MapYearData(this RouteGroupBuilder group)
    {
        var yearDataGroup = group.MapGroup("/year-data");
        yearDataGroup.MapPost("/set-subjects", SetSubjectsTaught.Delegate);
        return group;
    }

    private static RouteGroupBuilder MapDevelopmentEndpoints(this RouteGroupBuilder group)
    {
        var devGroup = group.MapGroup("/");
        devGroup.MapPost("/add-test-resources", CreateResourceDataForTesting.Delegate);
        return group;
    }
}