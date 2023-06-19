using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationRepository.Repository;
using ApplicationRepository.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposedValue;
        private readonly TableReservationContext _context;
        public UnitOfWork(TableReservationContext context)
        {
            _context = context;
        }
        public IGenericRepository<Rate> RateRepository
        {
            get
            {
                if(RateRepository == null)
                {
                    return new GenericRepository<Rate>(_context);
                }
                return RateRepository;
            }
        }

        public IGenericRepository<Reservation> ReservationRepository
        {
            get
            {
                if(ReservationRepository == null)
                {
                    return new GenericRepository<Reservation>(_context);
                }
                return ReservationRepository;
            }
        }

        public IGenericRepository<ReservationStatus> ReservationStatusRepository
        {
            get
            {
                if(ReservationStatusRepository == null)
                {
                    return new GenericRepository<ReservationStatus>(_context);
                }
                return ReservationStatusRepository;
            }
        }

        public IGenericRepository<Review> ReviewRepository
        {
            get
            {
                if(ReviewRepository == null)
                {
                    return new GenericRepository<Review>(_context);
                }
                return ReviewRepository;
            }
        }

        public IGenericRepository<ReviewRating> ReviewRatingRepository
        {
            get
            {
                if(ReviewRatingRepository == null)
                {
                    return new GenericRepository<ReviewRating>(_context);
                }
                return ReviewRatingRepository;
            }
        }

        public IGenericRepository<Role> RoleRepository
        {
            get
            {
                if(RoleRepository == null)
                {
                    return new GenericRepository<Role>(_context);
                }
                return RoleRepository;
            }
        }

        public IGenericRepository<Table> TableRepository
        {
            get
            {
                if(TableRepository == null)
                {
                    return new GenericRepository<Table>(_context);
                }
                return TableRepository;
            }
        }

        public IGenericRepository<TableStatus> TableStatusRepository
        {
            get
            {
                if(TableStatusRepository == null)
                {
                    return new GenericRepository<TableStatus>(_context);
                }
                return TableStatusRepository;
            }
        }

        public IGenericRepository<TableType> TableTypeRepository
        {
            get
            {
                if(TableTypeRepository == null)
                {
                    return new GenericRepository<TableType>(_context);
                }
                return TableTypeRepository;
            }
        }

        public IGenericRepository<User> UserRepository
        {
            get
            {
                if(UserRepository == null)
                {
                    return new GenericRepository<User>(_context);
                }
                return UserRepository;
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
