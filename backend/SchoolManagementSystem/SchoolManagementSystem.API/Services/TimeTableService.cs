using SchoolManagementSystem.API.Dtos;
using SchoolManagementSystem.API.Models;
using SchoolManagementSystem.API.Repository;

namespace SchoolManagementSystem.API.Services
{
    public class TimeTableService : ITimeTableService
    {
        private readonly ITimeTableRepository _timeTableRepository;
        private readonly IClassRepository _classRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ILogger<TimeTableService> _logger;

        public TimeTableService(
            ITimeTableRepository timeTableRepository,
            IClassRepository classRepository,
            ICourseRepository courseRepository,
            ITeacherRepository teacherRepository,
            ILogger<TimeTableService> logger)
        {
            _timeTableRepository = timeTableRepository;
            _classRepository = classRepository;
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _logger = logger;
        }

        public async Task<(bool Succeeded, string Message, Guid? TimeTableId)> CreateTimeTableAsync(
            TimeTableCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
              
                if (!await _classRepository.ExistAsync(dto.ClassId, cancellationToken))
                {
                    return (false, "Class not found.", null);
                }

                
                if (!await _courseRepository.ExistsAsync(dto.CourseId, cancellationToken))
                {
                    return (false, "Course not found.", null);
                }

             
                if (!await _teacherRepository.ExistsAsync(dto.TeacherId, cancellationToken))
                {
                    return (false, "Teacher not found.", null);
                }

          
                if (dto.EndTime <= dto.StartTime)
                {
                    return (false, "End time must be after start time.", null);
                }

            
                if (await _timeTableRepository.HasConflictAsync(
                    dto.ClassId, dto.Day, dto.StartTime, dto.EndTime, null, cancellationToken))
                {
                    return (false, "This time slot conflicts with another class for this class.", null);
                }

             
                if (await _timeTableRepository.TeacherHasConflictAsync(
                    dto.TeacherId, dto.Day, dto.StartTime, dto.EndTime, null, cancellationToken))
                {
                    return (false, "This time slot conflicts with another class for this teacher.", null);
                }

              
                if (await _timeTableRepository.RoomHasConflictAsync(
                    dto.RoomNumber, dto.Day, dto.StartTime, dto.EndTime, null, cancellationToken))
                {
                    return (false, "This room is already booked for this time slot.", null);
                }

                var timeTable = new TimeTable
                {
                    Id = Guid.NewGuid(),
                    ClassId = dto.ClassId,
                    CourseId = dto.CourseId,
                    TeacherId = dto.TeacherId,
                    Day = dto.Day,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    RoomNumber = dto.RoomNumber
                };

                var created = await _timeTableRepository.CreateAsync(timeTable, cancellationToken);

                return (true, "Timetable entry created successfully.", created.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating timetable entry");
                return (false, "An error occurred while creating the timetable entry.", null);
            }
        }

        public async Task<(bool Succeeded, string Message)> UpdateTimeTableAsync(
            Guid id,
            TimeTableUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var timeTable = await _timeTableRepository.GetByIdAsync(id, cancellationToken);
                if (timeTable == null)
                {
                    return (false, "Timetable entry not found.");
                }

          
                if (!await _teacherRepository.ExistsAsync(dto.TeacherId, cancellationToken))
                {
                    return (false, "Teacher not found.");
                }

               
                if (dto.EndTime <= dto.StartTime)
                {
                    return (false, "End time must be after start time.");
                }

                if (await _timeTableRepository.HasConflictAsync(
                    timeTable.ClassId, dto.Day, dto.StartTime, dto.EndTime, id, cancellationToken))
                {
                    return (false, "This time slot conflicts with another class for this class.");
                }

                
                if (await _timeTableRepository.TeacherHasConflictAsync(
                    dto.TeacherId, dto.Day, dto.StartTime, dto.EndTime, id, cancellationToken))
                {
                    return (false, "This time slot conflicts with another class for this teacher.");
                }

         
                if (await _timeTableRepository.RoomHasConflictAsync(
                    dto.RoomNumber, dto.Day, dto.StartTime, dto.EndTime, id, cancellationToken))
                {
                    return (false, "This room is already booked for this time slot.");
                }

                timeTable.TeacherId = dto.TeacherId;
                timeTable.Day = dto.Day;
                timeTable.StartTime = dto.StartTime;
                timeTable.EndTime = dto.EndTime;
                timeTable.RoomNumber = dto.RoomNumber;

                await _timeTableRepository.UpdateAsync(timeTable, cancellationToken);

                return (true, "Timetable entry updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating timetable {TimeTableId}", id);
                return (false, "An error occurred while updating the timetable entry.");
            }
        }

        public async Task<(bool Succeeded, string Message)> DeleteTimeTableAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _timeTableRepository.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    return (false, "Timetable entry not found.");
                }

                return (true, "Timetable entry deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting timetable {TimeTableId}", id);
                return (false, "An error occurred while deleting the timetable entry.");
            }
        }

        public async Task<TimeTableResponseDto?> GetTimeTableByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var timeTable = await _timeTableRepository.GetByIdAsync(id, cancellationToken);
            if (timeTable == null)
                return null;

            return MapToResponseDto(timeTable);
        }

        public async Task<IEnumerable<TimeTableResponseDto>> GetAllTimeTablesAsync(
            CancellationToken cancellationToken = default)
        {
            var timeTables = await _timeTableRepository.GetAllAsync(cancellationToken);
            return timeTables.Select(MapToResponseDto);
        }

        public async Task<WeeklyScheduleDto?> GetClassWeeklyScheduleAsync(
            Guid classId,
            CancellationToken cancellationToken = default)
        {
            var classEntity = await _classRepository.GetClassByIdAsync(classId, cancellationToken);
            if (classEntity == null)
                return null;

            var timeTables = await _timeTableRepository.GetByClassIdAsync(classId, cancellationToken);

            var schedule = new WeeklyScheduleDto
            {
                ClassId = classId,
                ClassName = classEntity.Name
            };

     
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                schedule.Schedule[day] = new List<TimeTableResponseDto>();
            }

         
            foreach (var timeTable in timeTables)
            {
                schedule.Schedule[timeTable.Day].Add(MapToResponseDto(timeTable));
            }

            return schedule;
        }

        public async Task<TeacherScheduleDto?> GetTeacherWeeklyScheduleAsync(
            Guid teacherId,
            CancellationToken cancellationToken = default)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId, cancellationToken);
            if (teacher == null)
                return null;

            var timeTables = await _timeTableRepository.GetByTeacherIdAsync(teacherId, cancellationToken);

            var schedule = new TeacherScheduleDto
            {
                TeacherId = teacherId,
                TeacherName = teacher.FullName
            };

        
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                schedule.Schedule[day] = new List<TimeTableResponseDto>();
            }

           
            foreach (var timeTable in timeTables)
            {
                schedule.Schedule[timeTable.Day].Add(MapToResponseDto(timeTable));
            }

            return schedule;
        }

        public async Task<DailyScheduleDto> GetClassDailyScheduleAsync(
            Guid classId,
            DayOfWeek day,
            CancellationToken cancellationToken = default)
        {
            var timeTables = await _timeTableRepository.GetByClassAndDayAsync(classId, day, cancellationToken);

            return new DailyScheduleDto
            {
                Day = day,
                DayName = day.ToString(),
                Slots = timeTables.Select(MapToResponseDto).ToList()
            };
        }

        public async Task<DailyScheduleDto> GetTeacherDailyScheduleAsync(
            Guid teacherId,
            DayOfWeek day,
            CancellationToken cancellationToken = default)
        {
            var timeTables = await _timeTableRepository.GetByTeacherAndDayAsync(teacherId, day, cancellationToken);

            return new DailyScheduleDto
            {
                Day = day,
                DayName = day.ToString(),
                Slots = timeTables.Select(MapToResponseDto).ToList()
            };
        }

        private TimeTableResponseDto MapToResponseDto(TimeTable timeTable)
        {
            return new TimeTableResponseDto
            {
                Id = timeTable.Id,
                ClassId = timeTable.ClassId,
                ClassName = timeTable.Class?.Name ?? "N/A",
                CourseId = timeTable.CourseId,
                CourseName = timeTable.Course?.Name ?? "N/A",
                CourseCode = timeTable.Course?.Code ?? "N/A",
                TeacherId = timeTable.TeacherId,
                TeacherName = timeTable.Teacher?.FullName ?? "N/A",
                Day = timeTable.Day,
                DayName = timeTable.Day.ToString(),
                StartTime = timeTable.StartTime,
                EndTime = timeTable.EndTime,
                RoomNumber = timeTable.RoomNumber,
                Duration = (int)(timeTable.EndTime - timeTable.StartTime).TotalMinutes
            };
        }
    }
}
