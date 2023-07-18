using System;

namespace YPrime.eCOA.DTOLibrary
{
    public class StudyInformation
    {
        public Guid Id { get; set; }
        public string Sponsor { get; set; }
        public string Protocol { get; set; }

        public string DatabaseConnection { get; set; }
        public string PortalUrl { get; set; }
        public string APIUrl { get; set; }
        public string SSOUrl { get; set; }
        public string InventoryUrl { get; set; }
        public string QuestionBuilderUrl { get; set; }

        public string StudyType { get; set; }
    }
}