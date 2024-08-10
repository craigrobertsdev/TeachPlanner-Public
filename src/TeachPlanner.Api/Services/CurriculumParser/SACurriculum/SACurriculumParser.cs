using TeachPlanner.Shared.Common.Interfaces.Curriculum;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Api.Services.CurriculumParser.SACurriculum;

public class SACurriculumParser : ICurriculumParser
{
    public List<CurriculumSubject> ParseCurriculum()
    {
        var curriculum = new List<CurriculumSubject>();

        var subjectDirectory = @"C:\Users\craig\source\repos\TeachPlanner\src\TeachPlanner.Curriculum Files";
        var files = Directory.GetFiles(subjectDirectory, "*.pdf");

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            CurriculumSubject subject = default!;
            if (fileName.StartsWith("The_Arts"))
            {
                var subjectName = fileName.Split('-')[1][..^4]; // remove the "The_Arts-" and ".pdf" extension
                subject = new ArtsParser(subjectName).ParseFile(file);
            }
            else if (fileName.Equals("English.pdf"))
            {
                subject = new EnglishParser().ParseFile(file);
            }
            else if (fileName.Equals("Mathematics.pdf"))
            {
                subject = new MathematicsParser().ParseFile(file);
            }
            else if (fileName.StartsWith("Language"))
            {
                var subjectName = fileName.Split('-')[1][..^4]; // remove the "Language-" and ".pdf" extension
                subject = new LanguageParser(subjectName).ParseFile(file);
            }
            else
            {
                throw new NotSupportedException(file);
            }

            curriculum.Add(subject);
        }

        return curriculum;
    }
}
