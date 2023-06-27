using ApplicationCore.Entities;
using ApplicationRepository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Reservation> ReservationRepository { get; }
        IGenericRepository<Feedback> ReviewRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<Table> TableRepository { get; }
        IGenericRepository<TableType> TableTypeRepository { get; }
        IGenericRepository<User> UserRepository { get; }

        void Commit();
        
    }
}
