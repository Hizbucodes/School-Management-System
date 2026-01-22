using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Repository
{
    public interface IStudentRepository
    {
        Task<(bool Succeeded, string Message, Guid? StudentId)> RegisterStudentAsync(StudentRegistrationDto dto);
    }
}
