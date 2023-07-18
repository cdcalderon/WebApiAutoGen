using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.TestObjects
{
    public abstract class RepositoryTestBase
    {

        protected YPrimeSession YPrimeSession = new YPrimeSession();

        protected void SetupSession()
        {
            HttpContext.Current = GetMockedHttpContext();
        }

        protected HttpContext GetMockedHttpContext()
        {
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(id => id.Name).Returns("test");

            var context = new HttpContext(
                new HttpRequest(string.Empty, "http://tempuri.org", string.Empty),
                new HttpResponse(new StringWriter()));

            context.User = user.Object;

            var sessionContainer = new HttpSessionStateContainer(
                "id",
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10,
                true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc,
                false);

            SessionStateUtility.AddHttpSessionStateToContext(context, sessionContainer);

            context.Session.Add("YPrimeSessionInstance", YPrimeSession);

            return context;
        }

        protected Mock<IStudySettingService> BuildBaseMockStudySettingService(
            List<StudySettingModel> baseModels = null)
        {
            if (baseModels == null)
            {
                baseModels = new List<StudySettingModel>
                {
                    new StudySettingModel
                    {
                        Properties = new StudySettingProperties
                        {
                            Key = "PatientNumberLength"
                        },
                        Value = "3"
                    },
                    new StudySettingModel
                    {
                        Properties = new StudySettingProperties
                        {
                            Key = "PatientNumberIncludeSiteId"
                        },
                        Value = "1"
                    },
                    new StudySettingModel
                    {
                        Properties = new StudySettingProperties
                        {
                            Key = "PatientNumberPrefix"
                        },
                        Value = "S"
                    },
                    new StudySettingModel
                    {
                        Properties = new StudySettingProperties
                        {
                            Key = "PatientNumberPrefixSiteSeparator"
                        },
                        Value = "-"
                    },
                    new StudySettingModel
                    {
                        Properties = new StudySettingProperties
                        {
                            Key = "PatientNumberSiteSubjectNumberSeparator"
                        },
                        Value = "-"
                    },
                    new StudySettingModel
                    {
                        Properties = new StudySettingProperties
                        {
                            Key = "PatientNumberIsStudyWide"
                        },
                        Value = "false"
                    },
                    new StudySettingModel
                    {
                        Properties = new StudySettingProperties
                        {
                            Key = "PatientPINLength"
                        },
                        Value = "4"
                    }
                };
            }

            var studyCustoms = new List<StudyCustomModel>();

            foreach (var studySetting in baseModels)
            {
                studyCustoms.Add(new StudyCustomModel
                {
                    Key = studySetting.Key,
                    Value = studySetting.Value
                });
            }

            var mockService = new Mock<IStudySettingService>();

            mockService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(baseModels);

            mockService
                .Setup(s => s.GetAllStudyCustoms(It.IsAny<Guid?>()))
                .ReturnsAsync(studyCustoms);

            foreach (var model in baseModels)
            {
                mockService
                    .Setup(s => s.Get(It.Is<string>(key => key == model.Key), It.IsAny<Guid?>()))
                    .ReturnsAsync(model);

                mockService
                    .Setup(s => s.GetStringValue(It.Is<string>(key => key == model.Key), It.IsAny<Guid?>()))
                    .ReturnsAsync(model.Value);

                if (int.TryParse(model.Value, out var parsedInt))
                {
                    mockService
                        .Setup(s => s.GetIntValue(It.Is<string>(key => key == model.Key), 
                            It.Is<int>(val => val == 0),
                            It.IsAny<Guid?>()))
                        .ReturnsAsync(parsedInt);
                }

                if (Guid.TryParse(model.Value, out var parsedGuid))
                {
                    mockService
                        .Setup(s => s.GetGuidValue(It.Is<string>(key => key == model.Key), It.IsAny<Guid?>()))
                        .ReturnsAsync(parsedGuid);
                }
            }

            return mockService;
        }
    }
}
