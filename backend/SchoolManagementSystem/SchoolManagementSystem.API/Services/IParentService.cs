using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Services
{
    public interface IParentService
    {
     
        Task<(bool Succeeded, string Message, Guid? ParentId)> RegisterParentAsync(ParentRegistrationDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UpdateParentAsync(Guid id, ParentUpdateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> DeleteParentAsync(Guid id, CancellationToken cancellationToken = default);

      
        Task<ParentResponseDto?> GetParentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ParentDetailDto?> GetParentDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ParentResponseDto>> GetAllParentsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ParentResponseDto>> GetParentsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

       
        Task<(bool Succeeded, string Message)> LinkStudentToParentAsync(Guid parentId, Guid studentId, string relationship, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> LinkMultipleStudentsAsync(Guid parentId, List<StudentRelationshipDto> students, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UpdateRelationshipAsync(Guid parentId, Guid studentId, string relationship, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UnlinkStudentFromParentAsync(Guid parentId, Guid studentId, CancellationToken cancellationToken = default);
    }
}
