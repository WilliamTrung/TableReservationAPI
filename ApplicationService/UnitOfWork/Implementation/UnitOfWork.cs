using ApplicationContext;
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

        private GenericRepository<Rate>? _rateRepos;
        private GenericRepository<Reservation>? _reservationRepos;
        private GenericRepository<Review>? _reviewRepos;
        private GenericRepository<ReviewRating>? _reviewRatingRepos;
        private GenericRepository<Role>? _roleRepos;
        private GenericRepository<Table>?    _tableRepos;
        private GenericRepository<User>? _userRepos;
        private GenericRepository<TableType>? _tableTypeRepos;

        public UnitOfWork(TableReservationContext context)
        {
            _context = context;
        }
        public IGenericRepository<Rate> RateRepository
        {
            get
            {
                if(_rateRepos == null)
                {
                    _rateRepos = new GenericRepository<Rate>(_context);
                }
                return _rateRepos;
            }
        }

        public IGenericRepository<Reservation> ReservationRepository
        {
            get
            {
                if(_reservationRepos == null)
                {
                    _reservationRepos = new GenericRepository<Reservation>(_context);
                }
                return _reservationRepos;
            }
        }

        public IGenericRepository<Review> ReviewRepository
        {
            get
            {
                if(_reviewRepos == null)
                {
                    _reviewRepos = new GenericRepository<Review>(_context);
                }
                return _reviewRepos;
            }
        }

        public IGenericRepository<ReviewRating> ReviewRatingRepository
        {
            get
            {
                if(_reviewRatingRepos == null)
                {
                    _reviewRatingRepos = new GenericRepository<ReviewRating>(_context);
                }
                return _reviewRatingRepos;
            }
        }

        public IGenericRepository<Role> RoleRepository
        {
            get
            {
                if(_roleRepos == null)
                {
                    _roleRepos = new GenericRepository<Role>(_context);
                }
                return _roleRepos;
            }
        }

        public IGenericRepository<Table> TableRepository
        {
            get
            {
                if(_tableRepos == null)
                {
                    _tableRepos = new GenericRepository<Table>(_context);
                }
                return _tableRepos;
            }
        }


        public IGenericRepository<TableType> TableTypeRepository
        {
            get
            {
                if(_tableTypeRepos == null)
                {
                    _tableTypeRepos = new GenericRepository<TableType>(_context);
                }
                return _tableTypeRepos;
            }
        }

        public IGenericRepository<User> UserRepository
        {
            get
            {
                if(_userRepos == null)
                {
                    _userRepos = new GenericRepository<User>(_context);
                }
                return _userRepos;
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
