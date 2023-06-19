using ApplicationCore.Entities;
using ApplicationService.Models.TableModels;
using ApplicationService.UnitOfWork;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services.Implementation
{
    public class TableManagement : ITableManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TableManagement(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        /// <summary>
        /// Add table
        /// Throw MissingMemberException: Status does not exist
        /// Throw MissingMemberException: Private and Seat set does not exist
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="MissingMemberException"></exception>
        public Task AddTable(ModifiedTableModel table)
        {
            var checkStatus = _unitOfWork.TableStatusRepository.Get(filter: status => status.Description == table.TableStatus).FirstOrDefault();
            if(checkStatus == null)
            {
                throw new MissingMemberException("Selected status does not exist!");
            }
            var checkType = _unitOfWork.TableTypeRepository.Get(filter: type => type.Private == table.Private && type.Seat == table.Seat).FirstOrDefault();
            if (checkType == null)
            {
                throw new MissingMemberException("Selected type does not exist!");
            }
            var added = new Table
            {
                Description = table.TableDescription,
                StatusId = checkStatus.Id,
                TypeId = checkType.Id
            };
            _unitOfWork.TableRepository.Create(added);
            _unitOfWork.Commit();
            
            return Task.CompletedTask;
        }

        public Task DisableTable(int id)
        {
            _unitOfWork.TableRepository.Delete(id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ModifiedTableModel>> GetTables()
        {
            var list = _unitOfWork.TableRepository.Get(filter: null, orderBy: null, includeProperties: "Status,Type");
            var temp = new List<ModifiedTableModel>();
            foreach (var item in list)
            {
                temp.Add(ModifiedTableModel.Converter(item));
            }   
            var result = (IEnumerable<ModifiedTableModel>) temp;
            return Task.FromResult(result);
        }

        public Task<IEnumerable<TableStatusModel>> GetTableStatuses()
        {
            var list = _unitOfWork.TableStatusRepository.Get();
            var result = _mapper.Map<IEnumerable<TableStatusModel>>(list);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<TableTypeModel>> GetTableTypes()
        {
            var list = _unitOfWork.TableTypeRepository.Get();
            var result = _mapper.Map<IEnumerable<TableTypeModel>>(list);
            return Task.FromResult(result);
        }

        public Task UpdateTable(ModifiedTableModel table)
        {
            var checkStatus = _unitOfWork.TableStatusRepository.Get(filter: status => status.Description == table.TableStatus).FirstOrDefault();
            if (checkStatus == null)
            {
                throw new MissingMethodException("Selected status does not exist!");
            }
            var checkType = _unitOfWork.TableTypeRepository.Get(filter: type => type.Private == table.Private && type.Seat == table.Seat).FirstOrDefault();
            if (checkType == null)
            {
                throw new MissingMethodException("Selected type does not exist!");
            }
            var updated = new Table
            {
                Id = table.Id,
                Description = table.TableDescription,
                StatusId = checkStatus.Id,
                TypeId = checkType.Id,
                IsDeleted = table.IsDeleted
            };
            _unitOfWork.TableRepository.Update(updated, updated.Id);
            _unitOfWork.Commit();
            return Task.CompletedTask;
        }
    }
}
