using System;
using YPrime.StudyPortal.Constants;

namespace YPrime.StudyPortal.Attributes
{
    public class MenuGroupAttribute : Attribute
    {
        public MenuGroupAttribute(MenuGroupType menuGroupType)
        {
            Type = menuGroupType;
        }

        public MenuGroupType Type { get; set; }
    }
}