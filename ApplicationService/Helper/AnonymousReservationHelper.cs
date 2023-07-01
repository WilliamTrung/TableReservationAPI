using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Helper
{
    public class AnonymousReservationHelper
    {
        public static string[] SplitNoteToPhone_Note(string note)
        {
            string[] result = new string[2];
            var temp = note.Split('-');
            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = temp[i];
            }
            return result;

        }
        public static string MergePhone_Note(string phone, string? note)
        {
            if (note.IsNullOrEmpty())
            {
                return phone;
            }
            return phone + "-" + note;
        }
    }
}
