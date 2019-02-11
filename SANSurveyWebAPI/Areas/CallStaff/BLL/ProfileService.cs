using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Areas.CallStaff.BLL
{
    public class ProfileService : IDisposable
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ProfileService()
        {
        }


        public IEnumerable<ProfileDto> GetProfiles()
        {

            var profiles = _unitOfWork.ProfileRespository.Get();

            //var profileDtos = profiles
            //                   .Select(b => ObjectMapper.GetProfileDto(b))
            //                   .ToList().AsQueryable<ProfileDto>();

            var profileDtos = profiles
                               .Select(b => ObjectMapper.GetProfileDto(b))
                               .ToList().AsQueryable<ProfileDto>();

            if (profileDtos != null)
                return profileDtos;

            return null;
        }


        public ProfileDto GetProfileById(int id)
        {
            var e = _unitOfWork.ProfileRespository.GetByID(id);

            if (e != null)
            {
                var dto = ObjectMapper.GetProfileDto(e);

                if (dto != null)
                    return dto;
            }
            return null;
        }




        public void Dispose()
        {
            //_unitOfWork.Dispose();
        }
    }
}