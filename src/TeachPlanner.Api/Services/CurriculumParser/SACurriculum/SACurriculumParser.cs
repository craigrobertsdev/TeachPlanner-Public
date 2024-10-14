using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Interfaces.Curriculum;

namespace TeachPlanner.Api.Services.CurriculumParser.SACurriculum;

public class SACurriculumParser : ICurriculumParser
{
    public async Task<List<CurriculumSubject>> ParseCurriculum()
    {
        var curriculum = new List<CurriculumSubject>();

        // var subjectDirectory = @"C:\Users\craig\source\repos\TeachPlanner\src\TeachPlanner.Curriculum Files";
        var subjectDirectory = @"/home/craig/source/TeachPlanner/src/TeachPlanner.Curriculum Files";
        var files = Directory.GetFiles(subjectDirectory, "*.pdf");

        List<Task<CurriculumSubject>> tasks = [];

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            CurriculumSubject subject;
            if (fileName.StartsWith("The_Arts"))
            {
                var subjectName = fileName.Split('-')[1][..^4]; // remove the "The_Arts-" and ".pdf" extension
                tasks.Add(Task.Run(() => new ArtsParser(subjectName).ParseFile(file)));
            }
            else if (fileName.Equals("English.pdf"))
            {
                tasks.Add(Task.Run(() => new EnglishParser().ParseFile(file)));
            }
            else if (fileName.Equals("Mathematics.pdf"))
            {
                tasks.Add(Task.Run(() => new MathematicsParser().ParseFile(file)));
            }
            else if (fileName.StartsWith("Language"))
            {
                var subjectName = fileName.Split('-')[1][..^4]; // remove the "Language-" and ".pdf" extension
                tasks.Add(Task.Run(() => new LanguageParser(subjectName).ParseFile(file)));
            }
            else
            {
                throw new NotSupportedException(file);
            }

            var subjects = await Task.WhenAll(tasks);

            curriculum.AddRange(subjects);
        }

        return curriculum;
    }
}