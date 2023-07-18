using System.Collections.Generic;

namespace YPrime.eCOA.DTOLibrary.ViewModel
{
    public class ApiRequestResultViewModel
    {
        public ApiRequestResultViewModel()
        {
            Errors = new List<string>();
        }

        public bool WasSuccessful { get; set; }
        public IList<string> Errors { get; set; }
    }
}