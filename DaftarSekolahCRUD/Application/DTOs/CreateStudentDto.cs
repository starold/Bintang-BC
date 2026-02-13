namespace DaftarSekolahCRUD.Application.DTOs
{
    public class CreateStudentDto
    {
    public string NIS {get; set;}
    public string Name{get; set;}
    public Guid ClassRoomId{get;set;}
    public string Address{get; set;}
    public string ParentName{get; set;}
    public List<Guid> ExtracurricularIds{get; set;}
    }
    
}