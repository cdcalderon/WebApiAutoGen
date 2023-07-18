using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class EmailContentConfiguration
    {
        public EmailContentConfiguration(EntityTypeConfiguration<EmailContent> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Data Type Seeding'");

            context.EmailContents.AddOrUpdate(dt => dt.Id,
                new EmailContent
                {
                    Id = Guid.Parse("112B1D12-19AC-40CE-A694-1D43724000AF"), Name = "Outdated Visit Alert",
                    TranslationKey = null, IsBlinded = true, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br /><h2>Outdated Visit Alert<br /><=Sponsor=> <=Protocol=><br /><br /></h2></div><div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br /><table style=\"width: 100%; background-color: #f5f6f7;\"><tbody><tr><td><div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div></td><td><table style=\"float: right;\" align=\"right\"><tbody><tr><td style=\"color: #6c769b;\">User Name:</td><td><=UserName=></td></tr><tr><td style=\"color: #6c769b;\">System Transaction Date:</td><td><=SystemDate=></td></tr></tbody></table></td></tr></tbody></table></div><div style=\"clear: both; text-align: center;\"><table style=\"margin: 0 auto; width: 700px;\" cellspacing=\"5\" cellpadding=\"4\"><tbody><tr><td style=\"color: #6c769b; text-align: right;\">Site Investigator Name:</td><td style=\"text-align: left;\"><=Investigator=></td></tr><tr><td style=\"color: #6c769b; text-align: right;\">&nbsp;Site Number:</td><td style=\"text-align: left;\"><=SiteNumber=></td></tr><tr><td style=\"color: #6c769b; text-align: right;\">Visit Name:</td><td style=\"text-align: left;\"><=Name=></td></tr><tr><td style=\"color: #6c769b; text-align: right;\"><=PatientLabel=> Number:</td><td style=\"text-align: left;\"><=PatientNumber=></td></tr><tr><td style=\"color: #6c769b; text-align: right;\">Window Type:</td><td style=\"text-align: left;\"><=VisitWindowType=></td></tr><tr><td style=\"color: #6c769b; text-align: right;\">Visit Recorded:</td><td style=\"text-align: left;\"><=VisitRecorded=></td></tr><tr><td style=\"color: #6c769b; text-align: right;\">Alert&nbsp;Date:</td><td style=\"text-align: left;\"><=DateCurrent=></td></tr></tbody></table><br /><strong><br />Note: </strong>The <=PatientLabel=> number listed in this alert has had a visit attempted outside the allowable visit window.<hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div><div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br /><div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div></div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=>&nbsp;-&nbsp;Outdated Visit Alert - Site Number&nbsp;<=SiteNumber=>- <=PatientLabel=>&nbsp;<=PatientNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("E61C4427-7285-48DF-B749-3F4237B809E0"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("35C35965-0DBD-4D3E-AE99-7024FBEF7605"),
                    Name = "Confirmation of <=PatientLabel=> Unblind (Unblinded)", TranslationKey = null,
                    IsBlinded = false, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br /> <h2 style=\"text-align: center;\">Confirmation of <span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> Unblind (Unblinded)<br /><span class=\"keyword\" contenteditable=\"false\"><=Sponsor=></span> <span class=\"keyword\" contenteditable=\"false\"><=Protocol=></span><br /><br /></h2> </div> <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br /> <table style=\"width: 100%; background-color: #f5f6f7;\"> <tbody> <tr> <td> <div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div> </td> <td> <table style=\"float: right;\" align=\"right\"> <tbody> <tr> <td style=\"color: #6c769b;\">User Name:</td> <td><span class=\"keyword\" contenteditable=\"false\"><=UserName=></span></td> </tr> <tr> <td style=\"color: #6c769b;\">System Transaction Date:</td> <td><span class=\"keyword\" contenteditable=\"false\"><=SystemDate=></span></td> </tr> <tr> <td style=\"color: #6c769b;\">Site Transaction Date:</td> <td><span class=\"keyword\" contenteditable=\"false\"><=LocalDate=></span></td> </tr> </tbody> </table> </td> </tr> </tbody> </table> </div> <div style=\"clear: both; text-align: center;\"> <table style=\"margin: 0 auto; width: 700px;\" cellspacing=\"5\" cellpadding=\"4\"> <tbody> <tr> <td style=\"color: #6c769b; text-align: right;\">Site Number:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=SiteID=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Site Investigator Name:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=Investigator=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\"><span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> Number:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=SubjectNumber=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\"><span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> Status:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=PatientStatus=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Date of Birth:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=keyDOB=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Gender:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=keyGender=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\"><span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> Initials:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=keyPatientInitials=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Treatment Arm:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=UNBLINDEDTreatmentArm=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Unblinded Date:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=DateCurrent=></span></td> </tr> </tbody> </table> <strong><br /><br />Note: </strong>This <span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> has been successfully unblinded.<hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div> <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br /> <div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div> </div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of <=PatientLabel=> Unblind (Unblinded) - Site <=SiteID=>&nbsp;- <=PatientLabel=>&nbsp;<=SubjectNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = true, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("AA535E16-6782-4F83-B5F5-72DA767297A0"),
                    Name = "Confirmation of <=PatientLabel=> Unblind (Blinded)", TranslationKey = null,
                    IsBlinded = true, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br /> <h2 style=\"text-align: center;\">Confirmation of <span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> Unblind (Blinded)<br /><span class=\"keyword\" contenteditable=\"false\"><=Sponsor=></span> <span class=\"keyword\" contenteditable=\"false\"><=Protocol=></span><br /><br /></h2> </div> <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br /> <table style=\"width: 100%; background-color: #f5f6f7;\"> <tbody> <tr> <td> <div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div> </td> <td> <table style=\"float: right;\" align=\"right\"> <tbody> <tr> <td style=\"color: #6c769b;\">User Name:</td> <td><span class=\"keyword\" contenteditable=\"false\"><=UserName=></span></td> </tr> <tr> <td style=\"color: #6c769b;\">System Transaction Date:</td> <td><span class=\"keyword\" contenteditable=\"false\"><=SystemDate=></span></td> </tr> <tr> <td style=\"color: #6c769b;\">Site Transaction Date:</td> <td><span class=\"keyword\" contenteditable=\"false\"><=LocalDate=></span></td> </tr> </tbody> </table> </td> </tr> </tbody> </table> </div> <div style=\"clear: both; text-align: center;\"> <table style=\"margin: 0 auto; width: 700px;\" cellspacing=\"5\" cellpadding=\"4\"> <tbody> <tr> <td style=\"color: #6c769b; text-align: right;\">Site Number:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=SiteID=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Site Investigator Name:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=Investigator=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\"><span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> Number:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=SubjectNumber=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\"><span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> Status:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=PatientStatus=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Date of Birth:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=keyDOB=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Gender:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=keyGender=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\"><span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> Initials:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=keyPatientInitials=></span></td> </tr> <tr> <td style=\"color: #6c769b; text-align: right;\">Unblinded Date:</td> <td style=\"text-align: left;\"><span class=\"keyword\" contenteditable=\"false\"><=DateCurrent=></span></td> </tr> </tbody> </table> <br /><strong><br />Note: </strong>This <span class=\"keyword\" contenteditable=\"false\"><=PatientLabel=></span> has been successfully unblinded.<hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div> <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br /> <div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div> </div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of <=PatientLabel=> Unblind (Blinded) - Site <=SiteID=>&nbsp;- <=PatientLabel=>&nbsp;<=SubjectNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = true, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("FAA312D7-212A-4E73-A22D-77F606AEC0D5"), Name = "Confirmation of Site Management",
                    TranslationKey = null, IsBlinded = true, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"> <br /> <h2 style=\"text-align: center;\">Confirmation of Site Management<br /> <=Sponsor=> <=Protocol=> <br /> <br /> </h2> </div> <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"> <br /> <table style=\"width: 100%; background-color: #f5f6f7;\"> <tbody> <tr> <td> <div style=\"margin-left: 15px;\"> <img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /> </div> </td> <td> <table style=\"float: right;\" align=\"right\"> <tbody> <tr> <td style=\"color: #6c769b;\">User Name:</td> <td> <=UserName=> </td> </tr> <tr> <td style=\"color: #6c769b;\">System Transaction Date:</td> <td> <=SystemDate=> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> </div> <div style=\"clear: both; text-align: center;\"> <table style=\"margin: 0 auto; width: 700px;\" cellspacing=\"5\" cellpadding=\"4\"> <tbody> <tr style=\"height: 18px;\"> <td style=\"color: #6c769b; height: 18px; text-align: right;\">Site Name:</td> <td style=\"height: 18px; text-align: left;\"> <=Name=> </td> </tr> <tr style=\"height: 18px;\"> <td style=\"color: #6c769b; height: 18px; text-align: right;\">Site Number:</td> <td style=\"height: 18px; text-align: left;\"> <=SiteNumber=> </td> </tr> <tr style=\"height: 18px;\"> <td style=\"color: #6c769b; height: 18px; text-align: right;\">Site Investigator Name:</td> <td style=\"height: 18px; text-align: left;\"> <=Investigator=> </td> </tr> <tr style=\"height: 12px;\"> <td style=\"color: #6c769b; height: 12px; text-align: right;\">Primary Contact:</td> <td style=\"height: 12px; text-align: left;\"> <=PrimaryContact=>&nbsp;</td> </tr> <tr style=\"height: 12px;\"> <td style=\"color: #6c769b; height: 12px; text-align: right;\">Shipping Address:</td> <td style=\"height: 12px; text-align: left;\"><=DisplayAddress=> </td> </tr> <tr style=\"height: 12px;\"> <td style=\"color: #6c769b; height: 12px; text-align: right;\">City, State:</td> <td style=\"height: 12px; text-align: left;\"> <=City=>,&nbsp;<=State=> </td> </tr> <tr style=\"height: 12px;\"> <td style=\"color: #6c769b; height: 12px; text-align: right;\">Zip Code:</td> <td style=\"height: 12px; text-align: left;\"><=Zip=> </td> </tr> <tr style=\"height: 12px;\"> <td style=\"color: #6c769b; height: 12px; text-align: right;\">Site Country:</td> <td style=\"height: 12px; text-align: left;\"> <=SiteCountry=> </td> </tr> <tr style=\"height: 12px;\"> <td style=\"color: #6c769b; height: 12px; text-align: right;\">Phone Number:</td> <td style=\"height: 12px; text-align: left;\"><=PhoneNumber=> </td> </tr> <tr style=\"height: 12px;\"> <td style=\"color: #6c769b; height: 12px; text-align: right;\">Fax&nbsp;Number:</td> <td style=\"height: 12px; text-align: left;\"> <=FaxNumber=>&nbsp;</td> </tr> <tr style=\"height: 12px;\"> <td style=\"color: #6c769b; height: 12px; text-align: right;\">Time Zone:</td> <td style=\"height: 12px; text-align: left;\"> <=TimeZone=>&nbsp;</td> </tr> <tr style=\"height: 18px;\"> <td style=\"color: #6c769b; height: 18px; text-align: right;\">Site Active:</td> <td style=\"height: 18px; text-align: left;\"><=IsActive=> </td> </tr> </tbody> </table> <br /> <strong>Note: </strong>The site details have been updated successfully.<strong> <br /> <br /> </strong> </div> <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"> <br /> <div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a> <br /> <br /> </div> </div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Site Management - Site&nbsp;<=SiteNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("18901012-7DF9-4481-BF20-797BAEC63AFE"), Name = "Confirmation of Early Termination",
                    TranslationKey = null, IsBlinded = true, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br /><h2 style=\"text-align: center;\">Confirmation of&nbsp;Early Termination<br /><=Sponsor=> <=Protocol=><br /><br /></h2></div><div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br /><table style=\"width: 100%; background-color: #f5f6f7;\"><tbody><tr><td><div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div></td><td><table style=\"float: right;\" align=\"right\"><tbody><tr><td style=\"color: #6c769b;\">User Name:</td><td><=UserName=></td></tr><tr><td style=\"color: #6c769b;\">System Transaction Date:</td><td><=SystemDate=></td></tr><tr><td style=\"color: #6c769b;\">Site Transaction Date:</td><td style=\"text-align: left;\"><=LocalDate=></td></tr></tbody></table></td></tr></tbody></table></div><div style=\"clear: both; text-align: center;\"><table style=\"margin: 0 auto; width: 700px;\" cellspacing=\"5\" cellpadding=\"4\"><tbody><=VisitQuestionnaireConfirmation=></tbody></table><br /><strong><br />Note: </strong>This <=PatientLabel=> has been successfully discontinued from the study.<hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div><div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br /><div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div></div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Early Termination - Site&nbsp;<=SiteID=>&nbsp;- <=PatientLabel=>&nbsp;<=SubjectNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = true,
                    IsSiteSpecific = true, PatientStatusTypeId = 4
                },
                new EmailContent
                {
                    Id = Guid.Parse("8A658560-987A-4171-A6E5-BE1DD1269751"), Name = "Confirmation of Screening",
                    TranslationKey = null, IsBlinded = true, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br />  <h2 style=\"text-align: center;\">Confirmation of Screening<br /><=Sponsor=> <=Protocol=><br /><br /></h2>  </div>  <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br />  <table style=\"width: 100%; background-color: #f5f6f7;\">  <tbody>  <tr>  <td>  <div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div>  </td>  <td>  <table style=\"float: right;\" align=\"right\">  <tbody>  <tr>  <td style=\"color: #6c769b;\">User Name:</td>  <td><=UserName=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">System Transaction Date:</td>  <td><=SystemDate=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">Site Transaction Date:</td>  <td><=LocalDate=></td>  </tr>  </tbody>  </table>  </td>  </tr>  </tbody>  </table>  </div>  <div style=\"clear: both; text-align: center;\"><=VisitQuestionnaireConfirmation=><br /><strong><br />Note: </strong>This subject&nbsp;has been successfully screened.<hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div>  <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br />  <div>yprime &copy; 2017 - For yprime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:primesupport@yprime.com\">primesupport@yprime.com</a><br /><br /></div>  </div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Screening - Site <=SiteID=>&nbsp;- <=PatientLabel=>&nbsp;<=SubjectNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = true,
                    IsSiteSpecific = true, PatientStatusTypeId = 1
                },
                new EmailContent
                {
                    Id = Guid.Parse("352471B1-6D18-4470-8282-D90BD03C816F"), Name = "Site Activation Alert",
                    TranslationKey = null, IsBlinded = true, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br /><h2>Site Activation Alert<br /><=Sponsor=> <=Protocol=><br /><br /></h2></div><div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br /><table style=\"width: 100%; background-color: #f5f6f7;\"><tbody><tr><td><div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div></td><td><table style=\"float: right;\" align=\"right\"><tbody><tr><td style=\"color: #6c769b;\">User Name:</td><td><=UserName=></td></tr><tr><td style=\"color: #6c769b;\">System Transaction Date:</td><td><=SystemDate=></td></tr></tbody></table></td></tr></tbody></table></div><div style=\"clear: both; text-align: center;\"><table style=\"margin: 0 auto; width: 700px;\" cellspacing=\"5\" cellpadding=\"4\"><tbody><tr><td style=\"color: #6c769b; text-align: right;\">Site Investigator Name:</td><td style=\"text-align: left;\"><=Investigator=></td></tr><tr><td style=\"color: #6c769b; text-align: right;\">&nbsp;Site Number:</td><td style=\"text-align: left;\"><=SiteNumber=></td></tr><tr><td style=\"color: #6c769b; text-align: right;\">&nbsp;Site Activation Date:</td><td style=\"text-align: left;\"><=DateCurrent=></td></tr></tbody></table><br /><strong><br />Note: </strong>The site noted above has been activated.<hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div><div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br /><div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div></div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=>&nbsp;- Site Activation Alert&nbsp;- Site Number: <=SiteNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("E61C4427-7285-48DF-B749-3F4237B809E0"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("78266C1F-BC86-4DFC-A9F5-F93456DB30A0"), Name = "Confirmation of Study Completion",
                    TranslationKey = null, IsBlinded = true, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br /><h2 style=\"text-align: center;\">Confirmation of Study Completion<br /><=Sponsor=> <=Protocol=><br /><br /></h2></div><div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br /><table style=\"width: 100%; background-color: #f5f6f7;\"><tbody><tr><td><div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div></td><td><table style=\"float: right;\" align=\"right\"><tbody><tr><td style=\"color: #6c769b;\">User Name:</td><td><=UserName=></td></tr><tr><td style=\"color: #6c769b;\">System Transaction Date:</td><td><=SystemDate=></td></tr><tr><td style=\"color: #6c769b;\">Site Transaction Date:</td><td><=LocalDate=></td></tr></tbody></table></td></tr></tbody></table></div><div style=\"clear: both; text-align: center;\"><table style=\"margin: 0 auto; width: 700px;\" cellspacing=\"5\" cellpadding=\"4\"><tbody><=VisitQuestionnaireConfirmation=></tbody></table><br /><strong><br />Note: </strong>This <=PatientLabel=> visit has been recorded successfully.<hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div><div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br /><div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div></div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Study Completion - Site <=SiteID=>&nbsp;- <=PatientLabel=>&nbsp;<=SubjectNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = true,
                    IsSiteSpecific = true, PatientStatusTypeId = 8
                },
                new EmailContent
                {
                    Id = Guid.Parse("B04D7B3B-6347-4F5C-9FFE-F9817F46133A"), Name = "Confirmation of Enrollment",
                    TranslationKey = null, IsBlinded = true, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br /><h2 style=\"text-align: center;\">Confirmation of&nbsp;Enrollment<br /><=Sponsor=> <=Protocol=><br /><br /></h2></div><div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br /><table style=\"width: 100%; background-color: #f5f6f7;\"><tbody><tr><td><div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div></td><td><table style=\"float: right;\" align=\"right\"><tbody><tr><td style=\"color: #6c769b;\">User Name:</td><td><=UserName=></td></tr><tr><td style=\"color: #6c769b;\">System Transaction Date:</td><td><=SystemDate=></td></tr><tr><td style=\"color: #6c769b;\">Site Transaction Date:</td><td><=LocalDate=></td></tr></tbody></table></td></tr></tbody></table></div><div style=\"clear: both; text-align: center;\"><=VisitQuestionnaireConfirmation=><br /><strong><br />Note: </strong>This <=PatientLabel=>'s has been successfully recorded.<hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div><div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br /><div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div></div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Randomization Visit- Site&nbsp;<=SiteID=>&nbsp;- <=PatientLabel=>&nbsp;<=SubjectNumber=>",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = true, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("9C6E5203-83D9-48BE-96C8-0253F2B664BA"),
                    Name = "Confirmation of Data Correction Form (Approved)", TranslationKey = null, IsBlinded = false,
                    Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br />  <h2 style=\"text-align: center;\">Confirmation of Data Correction Form<br /><=Sponsor=> <=Protocol=><br /><br /></h2>  </div>  <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br />  <table style=\"width: 100%; background-color: #f5f6f7;\">  <tbody>  <tr>  <td>  <div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div>  </td>  <td>  <table style=\"float: right;\" align=\"right\">  <tbody>  <tr>  <td style=\"color: #6c769b;\">User Name:</td>  <td><=UserName=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">System Transaction Date:</td>  <td><=SystemDate=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">Site Transaction Date:</td>  <td><=LocalDate=></td>  </tr>  </tbody>  </table>  </td>  </tr>  </tbody>  </table>  </div>  <div style=\"clear: both; text-align: center;\"><br /><=VisitQuestionnaireConfirmation=><br /><hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div>  <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br />  <div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div>  </div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Data Correction Form (Approved)",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = Guid.Parse("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("4C4D58DE-44FD-4AC5-8990-6BF1FAD00AB2"),
                    Name = "Confirmation of Data Correction Form (Rejected)", TranslationKey = null, IsBlinded = false,
                    Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br />  <h2 style=\"text-align: center;\">Confirmation of Data Correction Form<br /><=Sponsor=> <=Protocol=><br /><br /></h2>  </div>  <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br />  <table style=\"width: 100%; background-color: #f5f6f7;\">  <tbody>  <tr>  <td>  <div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div>  </td>  <td>  <table style=\"float: right;\" align=\"right\">  <tbody>  <tr>  <td style=\"color: #6c769b;\">User Name:</td>  <td><=UserName=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">System Transaction Date:</td>  <td><=SystemDate=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">Site Transaction Date:</td>  <td><=LocalDate=></td>  </tr>  </tbody>  </table>  </td>  </tr>  </tbody>  </table>  </div>  <div style=\"clear: both; text-align: center;\"><br /><=VisitQuestionnaireConfirmation=><br /><hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div>  <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br />  <div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div>  </div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Data Correction Form (Rejected)",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = Guid.Parse("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("21480126-512F-4848-80D5-40BB2E7D4F3B"),
                    Name = "Confirmation of Data Correction Form (Need More Information)", TranslationKey = null,
                    IsBlinded = false, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br />  <h2 style=\"text-align: center;\">Confirmation of Data Correction Form<br /><=Sponsor=> <=Protocol=><br /><br /></h2>  </div>  <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br />  <table style=\"width: 100%; background-color: #f5f6f7;\">  <tbody>  <tr>  <td>  <div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div>  </td>  <td>  <table style=\"float: right;\" align=\"right\">  <tbody>  <tr>  <td style=\"color: #6c769b;\">User Name:</td>  <td><=UserName=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">System Transaction Date:</td>  <td><=SystemDate=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">Site Transaction Date:</td>  <td><=LocalDate=></td>  </tr>  </tbody>  </table>  </td>  </tr>  </tbody>  </table>  </div>  <div style=\"clear: both; text-align: center;\"><br /><=VisitQuestionnaireConfirmation=><br /><hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div>  <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br />  <div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div>  </div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Data Correction Form (Need More Information)",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = Guid.Parse("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("2730FE90-6191-4E49-8D89-6A82232B4DFF"),
                    Name = "Confirmation of Data Correction Form (Pending Approval)", TranslationKey = null,
                    IsBlinded = false, Notes = null, LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\"><br />  <h2 style=\"text-align: center;\">Confirmation of Data Correction Form<br /><=Sponsor=> <=Protocol=><br /><br /></h2>  </div>  <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br />  <table style=\"width: 100%; background-color: #f5f6f7;\">  <tbody>  <tr>  <td>  <div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div>  </td>  <td>  <table style=\"float: right;\" align=\"right\">  <tbody>  <tr>  <td style=\"color: #6c769b;\">User Name:</td>  <td><=UserName=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">System Transaction Date:</td>  <td><=SystemDate=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">Site Transaction Date:</td>  <td><=LocalDate=></td>  </tr>  </tbody>  </table>  </td>  </tr>  </tbody>  </table>  </div>  <div style=\"clear: both; text-align: center;\"><br /><=VisitQuestionnaireConfirmation=><br /><hr style=\"border-width: 1px 0px 0px 0px; border-style: inset;\" /></div>  <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br />  <div>YPrime &copy; <=YYYY=> - For YPrime support, contact us at +1-(844)-385-2352 or via email at: <a href=\"mailto:yprimesupport@yprime.com\" style=\"color:#33ceff\">yprimesupport@yprime.com</a><br /><br /></div>  </div>",
                    SubjectLineTemplate =
                        "<=Sponsor=>&nbsp;<=Protocol=> Confirmation of Data Correction Form (Pending Approval)",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = Guid.Parse("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("0BDED019-6F77-4F06-8617-3D396C2906DA"), Name = "Web backup subject e-mail",
                    TranslationKey = null, IsBlinded = false, Notes = null, LastUpdate = null,
                    BodyTemplate = "{{Translation:WebBackupBodyKey}}",
                    SubjectLineTemplate = "{{Translation:WebBackupTitleKey}}'", IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = Guid.Parse("8F9502CA-CADA-4713-881C-B327DAAADB5F"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("DB3D18BA-E8A8-4736-8793-D15B47526A99"),
                    Name = "Confirmation of Bulk Site Activation", TranslationKey = null, IsBlinded = true,
                    Notes = "Confirmation of Bulk Site Activation", LastUpdate = null,
                    BodyTemplate =
                        "<div style=\"background-color: #314158; text-align: center; color: white;\">  <h2 style=\"text-align: center;\">Confirmation of&nbsp;Bulk Site Management<br /><=Sponsor=>&nbsp;<=Protocol=></h2>  </div>  <div style=\"border-bottom: 4px solid #5c6b7d; background-color: #f5f6f7;\"><br />  <table style=\"width: 100%; background-color: #f5f6f7;\">  <tbody>  <tr style=\"height: 54.375px;\">  <td style=\"height: 54.375px;\">  <div style=\"margin-left: 15px;\"><img src=\"https://ypstorageprd.blob.core.windows.net/shared-assets/Images/YPrimeLogo-EmailContent.png\" alt=\"Logo\" width=\"110\" height=\"50\" /></div>  </td>  <td style=\"height: 54.375px;\">  <table style=\"float: right;\" align=\"right\">  <tbody>  <tr>  <td style=\"color: #6c769b;\">User Name:</td>  <td><=UserName=></td>  </tr>  <tr>  <td style=\"color: #6c769b;\">System Transaction Date:</td>  <td><=SystemDate=></td>  </tr>  </tbody>  </table>  </td>  </tr>  </tbody>  </table>  </div>  <div style=\"clear: both; text-align: center;\"><br /><center><=SiteActivationTable=></center><br /><strong>Note:&nbsp;</strong>The Site Management changes have been updated successfully.<strong>&nbsp;<br /><br /></strong></div>  <div style=\"background-color: #5c6b7d; color: white; text-align: center;\"><br />  <div>yprime &copy; <=YYYY=> - For yprime support, contact us at +1-(844)-385-2352 or via email at:&nbsp;<a href=\"mailto:yprimesupport@yprime.com\">yprimesupport@yprime.com</a></div>  </div>",
                    SubjectLineTemplate = "<=Sponsor=>&nbsp;<=Protocol=>&nbsp;Confirmation of Bulk Site Management",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = Guid.Parse("E61C4427-7285-48DF-B749-3F4237B809E0"), DisplayOnScreen = false,
                    IsSiteSpecific = false, PatientStatusTypeId = null
                },
                new EmailContent
                {
                    Id = Guid.Parse("B04D7B3B-6347-4F5C-9FFE-F9817F4613BA"),
                    Name = "BYOD Confirmation Email",
                    TranslationKey = null,
                    IsBlinded = true,
                    Notes = null,
                    LastUpdate = null,
                    BodyTemplate =
                        "Placeholder for BYOD confirmation email body",
                    SubjectLineTemplate =
                        "Placeholder for BYOD confirmation email subject",
                    IsEmailSentToPerformingUser = false,
                    EmailContentTypeId = new Guid("8F9502CA-CADA-4713-881C-B327DAAADB5F"),
                    DisplayOnScreen = false,
                    IsSiteSpecific = true,
                    PatientStatusTypeId = null
                }
            );

            context.SaveChanges();
        }
    }
}