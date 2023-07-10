using ApplicationCore.Entities;
using ApplicationCore.Enum;
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
        public Task AddTable(NewTableModel table)
        {
            bool checkStatus = Enum.IsDefined(typeof(IEnum.TableStatus), table.Status);
            
            if(!checkStatus)
            {
                throw new MissingMemberException("Selected status does not exist!");
            }
            var checkType = _unitOfWork.TableTypeRepository.Get(filter: type => type.Private == table.Private && type.Seat == table.Seat).Result.FirstOrDefault();
            if (checkType == null)
            {
                throw new MissingMemberException("Selected type does not exist!");
            }
            var added = new Table
            {
                Description = table.TableDescription,
                Status = table.Status,
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
            var list = _unitOfWork.TableRepository.Get(filter: null, orderBy: null, includeProperties: "Type").Result;
            var temp = new List<ModifiedTableModel>();
            foreach (var item in list)
            {
                temp.Add(ModifiedTableModel.Converter(item));
            }   
            var result = (IEnumerable<ModifiedTableModel>) temp;
            return Task.FromResult(result);
        }

        public Task<IEnumerable<string>> GetTableStatuses()
        {
            List<string> enumValues = Enum.GetValues(typeof(IEnum.TableStatus))
                              .Cast<IEnum.TableStatus>()
                              .Select(c => c.ToString())
                              .ToList();
            return Task.FromResult((IEnumerable<string>)enumValues);
        }

        public Task<IEnumerable<TableTypeModel>> GetTableTypes()
        {
            var tableTypeRepos = _unitOfWork.TableTypeRepository;
            var list = tableTypeRepos.Get();
            var result = _mapper.Map<IEnumerable<TableTypeModel>>(list.Result);
            return Task.FromResult(result);
        }

        public Task UpdateTable(ModifiedTableModel table)
        {
            //var checkStatus = _unitOfWork.TableStatusRepository.Get(filter: status => status.Description.ToUpper() == table.TableStatus.ToUpper()).Result.FirstOrDefault();
            bool checkStatus = Enum.IsDefined(typeof(IEnum.TableStatus), table.Status);

            if (!checkStatus)
            {
                throw new MissingMethodException("Selected status does not exist!");
            }
            var checkType = _unitOfWork.TableTypeRepository.Get(filter: type => type.Private == table.Private && type.Seat == table.Seat).Result.FirstOrDefault();
            if (checkType == null)
            {
                throw new MissingMethodException("Selected type does not exist!");
            }
            var updated = new Table
            {
                Id = table.Id,
                Description = table.TableDescription,
                Status = table.Status,
                TypeId = checkType.Id,
                IsDeleted = table.IsDeleted
            };
            _unitOfWork.TableRepository.Update(updated, updated.Id);
            _unitOfWork.Commit();
            return Task.CompletedTask;
        }
    }
}
