using ApplicationCore.Entities;
using ApplicationService.Models;
using ApplicationService.Models.TableModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Role, RoleModel>().ReverseMap();            
            CreateMap<TableStatus, TableStatusModel>().ReverseMap();
            CreateMap<TableType, TableTypeModel>().ReverseMap();
        }
    }
}
