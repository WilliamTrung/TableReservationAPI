using ApplicationService.Models.FeedbackModels;
using ApplicationService.Models.UserModels;
using ApplicationService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services.Implementation
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task GetFeedbackByReservation(int reservationId, AuthorizedModel requester)
        {
            throw new NotImplementedException();
        }

        public Task GetFeedbacks()
        {
            throw new NotImplementedException();
        }

        public Task SendFeedback(FeedbackModel feedback)
        {
            throw new NotImplementedException();
        }
    }
}
