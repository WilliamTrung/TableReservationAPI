using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
namespace Validator
{
    public class TimeValidator : ValidationAttribute
    {
        public string OclockErrorMessage { get; set; } = "The time must be 'o'clock'.";
        public string BetweenErrorMessage { get; set; } = "The time must be between " + GlobalValidation.START_TIME + "h and " + GlobalValidation.END_TIME + "h!";

        public override bool IsValid(object value)
        {
            if (value is DateTime time)
            {
                if (IsOclock(time))
                {
                    // Time is "o'clock"
                    return true;
                }

                if (IsBetween(time, new TimeSpan(GlobalValidation.START_TIME, 0, 0), new TimeSpan(GlobalValidation.END_TIME, 0, 0)))
                {
                    // Time is between 8 AM and 6 PM
                    return true;
                }
            }

            return false;
        }

        private bool IsOclock(DateTime time)
        {
            return time.Minute == 0 && time.Second == 0;
        }

        private bool IsBetween(DateTime time, TimeSpan startTime, TimeSpan endTime)
        {
            TimeSpan currentTime = time.TimeOfDay;
            return currentTime >= startTime && currentTime <= endTime;
        }

        public override string FormatErrorMessage(string name)
        {
            if (IsOclockErrorMessageApplicable() && !IsBetweenErrorMessageApplicable())
            {
                return OclockErrorMessage;
            }
            else if (!IsOclockErrorMessageApplicable() && IsBetweenErrorMessageApplicable())
            {
                return BetweenErrorMessage;
            }
            else
            {
                return base.FormatErrorMessage(name);
            }
        }

        private bool IsOclockErrorMessageApplicable()
        {
            return !string.IsNullOrEmpty(OclockErrorMessage);
        }

        private bool IsBetweenErrorMessageApplicable()
        {
            return !string.IsNullOrEmpty(BetweenErrorMessage);
        }
    }


}
