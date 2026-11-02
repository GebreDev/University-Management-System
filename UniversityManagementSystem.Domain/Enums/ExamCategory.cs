namespace UniversityManagementSystem.Domain.Enums
{
    /// <summary>
    /// The category of an exam (what kind of assessment it is).
    /// Question types (MCQ, Essay, ...) are tracked separately on Question.
    /// </summary>
    public enum ExamCategory
    {
        Midterm = 1,
        Final = 2,
        Quiz = 3,
        Assignment = 4
    }
}
