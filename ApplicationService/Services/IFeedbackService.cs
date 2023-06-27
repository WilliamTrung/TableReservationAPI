using ApplicationService.Models.FeedbackModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IFeedbackService
    {
        Task SendFeedback(FeedbackModel feedback);
        Task GetFeedbacks();
    }
}
