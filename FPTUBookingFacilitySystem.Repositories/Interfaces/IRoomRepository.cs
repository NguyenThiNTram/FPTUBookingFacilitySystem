using FPTUBookingFacilitySystem.Repositories.Entities;

namespace FPTUBookingFacilitySystem.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
        Task<Room> CreateRoomAsync(Room room);
        Task<Room?> UpdateRoomAsync(int id, Room room);
        Task<bool> UpdateRoomStatusAsync(int id, string status);
        Task<bool> DeleteRoomAsync(int id);
    }
}

