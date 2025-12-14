using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;

namespace FPTUBookingFacilitySystem.Services.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomResponse>> GetAllRoomsAsync();
        Task<RoomResponse?> GetRoomByIdAsync(int id);
        Task<RoomResponse> CreateRoomAsync(CreateRoomRequest request);
        Task<RoomResponse?> UpdateRoomAsync(int id, UpdateRoomRequest request);
        Task<bool> UpdateRoomStatusAsync(int id, UpdateRoomStatusRequest request);
        Task<bool> DeleteRoomAsync(int id);
    }
}

