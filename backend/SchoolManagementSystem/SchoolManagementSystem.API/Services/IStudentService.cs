using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Services
{
    public interface IStudentService
    {
     
        Task<(bool Succeeded, string Message, Guid? StudentId)> RegisterStudentAsync(StudentRegistrationDto dto, CancellationToken cancellationToken = default);

        
        Task<(bool Succeeded, string Message)> UpdateStudentAsync(Guid id, StudentUpdateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default);


        Task<StudentResponseDto?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<StudentDetailDto?> GetStudentDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<StudentResponseDto>> GetStudentsByClassAsync(Guid classId, CancellationToken cancellationToken = default);
    }
}
