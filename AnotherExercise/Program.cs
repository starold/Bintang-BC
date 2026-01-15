class Student
{
    public string Name {get; set;}
    public int Score {get; set;}
    public string GetGrade()
    {
        if (Score >= 85) return "A";
        
    }
}