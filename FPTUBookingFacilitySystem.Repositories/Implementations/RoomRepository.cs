using FPTUBookingFacilitySystem.Repositories.Context;
using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTUBookingFacilitySystem.Repositories.Implementations
{
    public class RoomRepository : IRoomRepository
    {
        private readonly FPTUBookingFacilityDbContext _context;

        public RoomRepository(FPTUBookingFacilityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .Include(r => r.Campus)
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomId)
                // .OrderBy(r => r.RoomName)
                .ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Campus)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomId == id);
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            room.CreatedAt = DateTime.UtcNow;
            room.UpdatedAt = DateTime.UtcNow;
            
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            
            // Reload with related entities
            return await _context.Rooms
                .Include(r => r.Campus)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomId == room.RoomId) ?? room;
        }

        public async Task<Room?> UpdateRoomAsync(int id, Room room)
        {
            var existingRoom = await _context.Rooms
                .Include(r => r.Campus)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomId == id);
            if (existingRoom == null)
            {
                return null;
            }

            existingRoom.RoomName = room.RoomName;
            existingRoom.RoomTypeId = room.RoomTypeId;
            existingRoom.Capacity = room.Capacity;
            existingRoom.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingRoom;
        }

        public async Task<bool> UpdateRoomStatusAsync(int id, string status)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return false;
            }

            room.RoomStatus = status;
            room.UpdatedAt = DateTime.Now;

            _context.Entry(room).Property(r => r.RoomStatus).IsModified = true;
            _context.Entry(room).Property(r => r.UpdatedAt).IsModified = true;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return false;
            }

            if (_context.Bookings.Any(b => b.RoomId == id))
            {
                throw new InvalidOperationException("Cannot delete room with existing bookings.");
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

