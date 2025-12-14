using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;
using FPTUBookingFacilitySystem.Services.Interfaces;

namespace FPTUBookingFacilitySystem.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<RoomResponse>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllRoomsAsync();
            return rooms.Select(r => MapToResponse(r));
        }

        public async Task<RoomResponse?> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
                return null;

            return MapToResponse(room);
        }

        public async Task<RoomResponse> CreateRoomAsync(CreateRoomRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RoomName))
                throw new ArgumentException("Room name is required.");

            if (string.IsNullOrWhiteSpace(request.RoomStatus))
                throw new ArgumentException("Room status is required.");

            if (!_validStatuses.Contains(request.RoomStatus.ToLower()))
                throw new ArgumentException("Invalid room status.");

            if (request.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0.");

            var room = new Room
            {
                RoomName = request.RoomName,
                CampusId = request.CampusId,
                RoomTypeId = request.RoomTypeId,
                RoomStatus = request.RoomStatus,
                Capacity = request.Capacity
            };

            var createdRoom = await _roomRepository.CreateRoomAsync(room);
            return MapToResponse(createdRoom);
        }

        public async Task<RoomResponse?> UpdateRoomAsync(int id, UpdateRoomRequest request)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
                return null;

            if (string.IsNullOrWhiteSpace(request.RoomName))
                throw new ArgumentException("Room name is required.");

            if (request.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0.");

            room.RoomName = request.RoomName;
            room.Capacity = request.Capacity;
            room.RoomTypeId = request.RoomTypeId;
            // room.UpdatedAt = DateTime.Now;

            await _roomRepository.UpdateRoomAsync(id, room);
            return MapToResponse(room);
        }


        private readonly List<string> _validStatuses = new()
        {
            "available",
            "under_maintenance",
            "unavailable"
        };

        public async Task<bool> UpdateRoomStatusAsync(int id, UpdateRoomStatusRequest request)
        {
            var status = request.RoomStatus.ToLower();

            if (!_validStatuses.Contains(status))
                throw new ArgumentException("Invalid room status.");

            return await _roomRepository.UpdateRoomStatusAsync(id, status);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            return await _roomRepository.DeleteRoomAsync(id);
        }

        private static RoomResponse MapToResponse(Room room)
        {
            return new RoomResponse
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                CampusId = room.CampusId,
                CampusName = room.Campus?.CampusName,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType?.TypeName,
                RoomStatus = room.RoomStatus,
                Capacity = room.Capacity,
                CreatedAt = room.CreatedAt,
                UpdatedAt = room.UpdatedAt
            };
        }
    }
}

