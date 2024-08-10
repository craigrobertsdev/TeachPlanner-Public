using System.Text;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Curriculum;
using UglyToad.PdfPig;

namespace TeachPlanner.Api.Services.CurriculumParser.SACurriculum;

public class LanguageParser : BaseParser
{
    private static readonly char[] _contentDescriptionEndings = ['^'];
    public LanguageParser(string subjectName) : base(subjectName, _contentDescriptionEndings) { }

    protected override YearLevel ParseLearningStandard(PdfDocument document, ref string description)
    {
        var page = document.GetPage(_currentPageNum);
        var descriptionBuilder = new StringBuilder();
        var words = page.Text.Split(' ');
        var idx = 0;

        var found = false;
        do
        {
            var word = words[idx];
            if (word == "Languages")
            { // Handles the first occurrence of the subject name which is in the footer of the page
                if (found)
                {
                    break;
                }

                found = true;
            }
        } while (++idx < words.Length);

        // Get the subject description to pass up to the subject
        do
        {
            idx++;
            if (words[idx] == "Learning")
            {
                idx++;
                if (words[idx] == "Standard")
                {
                    break;
                }
                else
                {
                    descriptionBuilder.Append("Learning");
                }
            }

            if (words[idx] == string.Empty)
            {
                descriptionBuilder.Append("\n\n");
            }
            else
            {
                descriptionBuilder.Append(words[idx]);
                descriptionBuilder.Append(' ');
            }
        } while (idx < words.Length);

        YearLevelValue yearLevelValue;
        var desc = descriptionBuilder.ToString();
        var wordIdx = desc.Length - 7; // The shortest YearLevelValue is 6 so start here.
        do
        {
            if (wordIdx == 0)
            {
                throw new Exception("Year Level not found");
            }
            try
            {
                yearLevelValue = Enum.Parse<YearLevelValue>(desc[wordIdx..].Replace(" ", string.Empty));
                break;
            }
            catch
            {
                wordIdx--;
                continue;
            }
        } while (true);

        description = desc[0..wordIdx].Trim();

        var learningStandardBuilder = new StringBuilder();
        idx++;
        var prev = words[idx]; // to handle multiple consecutive empty strings
        do
        {
            var word = words[idx];
            if (word == "UNDERSTAND:")
            {
                break;
            }

            if (word == "Students") // the documents follow a consistent pattern at the start of every paragraph
            {
                learningStandardBuilder.Append("\n\n");
            }

            learningStandardBuilder.Append(word);
            learningStandardBuilder.Append(' ');
            prev = word;
            idx++;
        } while (idx < words.Length);

        var learningStandard = learningStandardBuilder.ToString().Trim();
        _currentPageNum++;

        return YearLevel.Create(yearLevelValue, learningStandard);
    }


    protected override bool IsContentDescriptionEnd(string text)
    {
        return text.Contains("AC9") || _contentDescriptionEndings.Any(text.Contains);
    }
}
