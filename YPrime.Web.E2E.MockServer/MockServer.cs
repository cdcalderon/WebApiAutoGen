using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Web.E2E.MockServer
{
    public class MockServer
    {
        private readonly string wireMockServerUrl = @"https://web-wiremock.azurewebsites.net";
        public const int mockServerPort = 3030;
        private const string StudySettingsDefaultMappingFile = "StudySettingEndpoint.json";
        private const string TranslationsEndpointMappingFile = "TranslationEndpoint.json";
        private const string SingleTranslationEndpointMappingFile = "TranslationSingleEndpoint.json";
        private const string TranslationWithLanguageIdEndpointMappingFile = "TranslationWithLanguageIdEndpoint.json";
        private const string LanguageEndpointMappingFile = "LanguageEndpoint.json";

        private const string NotificationScheduleCancelEndPointFile = "NotificationScheduleCancelEndPoint.json";
        private const string NotificationScheduleUpdateStatusEndPointFile = "NotificationScheduleUpdateStatusEndPoint.json";

        private const string MappingGuidPlaceholder = "<MAPPINGGUID>";
        private const string MappingResourceKeyPlaceholder = "<RESOURCEKEY>";
        private const string MappingResourceValuePlaceholder = "<RESOURCEVALUE>";
        private const string MappingLanguageIdPlaceholder = "<LANGUAGEID>";

        // Endpoints
        public List<Mapping> Mappings { get; set; } = new List<Mapping>
        {
            new Mapping("AllDeepLoadedQuestionsEndpoint.json"),
            new Mapping("ApproverGroupEndpoint.json"),
            new Mapping("BusinessRuleEndpoint.json"),
            new Mapping("CareGiverTypeEndpoint.json"),
            new Mapping("CorrectionTypeEndpoint.json"),
            new Mapping("CorrectionWorkflowEndpoint.json"),
            new Mapping("CorrectionWorkflowEndpointAddPaperQues.json"),
            new Mapping("CorrectionWorkflowEndpointChangeQuesInfo.json"),
            new Mapping("CorrectionWorkflowEndpointChangeQuesResponse.json"),
            new Mapping("CorrectionWorkflowEndpointChangeSubjectVisit.json"),
            new Mapping("CorrectionWorkflowEndpointMergeSubject.json"),
            new Mapping("CorrectionWorkflowEndpointRemoveSubject.json"),
            new Mapping("CountryEndpoint.json", 
                "Country/{ID}"),
            new Mapping("DeepLoadedQuestionniareFormsQuestionnaireEndpoint.json"),
            new Mapping("DeepLoadedTemperatureQuestionnaireEndpoint.json"),
            new Mapping("DeepLoadedMultiSelectQuestionnaireEndpoint.json"),
            new Mapping("DeepLoadedFreeTextQuestionnaireEndpoint.json"),
            new Mapping("LanguageEndpoint.json"),
            new Mapping("LanguageGermanEndpoint.json"),
            new Mapping("LanguageEnglishEndpoint.json"),
            new Mapping("PatientStatusTypeEndpoint.json"),
            new Mapping("QuestionnairePagesEndpoint.json"),
            new Mapping("QuestionsEndpoint.json", 
                "Questionnaire/{ID}/questions",
                new List<string>(){"DeepLoadedQuestionniareFormsQuestionnaireEndpoint.json",
                    "DeepLoadedTemperatureQuestionnaireEndpoint.json" }, ".pages-questions"),

              new Mapping("QuestionsEndpoint.json",
                "Questionnaire/{ID}/questions",
                new List<string>(){"DeepLoadedQuestionniareFormsQuestionnaireEndpoint.json",
                    "DeepLoadedMultiSelectQuestionnaireEndpoint.json" }, ".pages-questions"),

              new Mapping("QuestionsEndpoint.json",
                "Questionnaire/{ID}/questions",
                new List<string>(){"DeepLoadedQuestionniareFormsQuestionnaireEndpoint.json",
                    "DeepLoadedFreeTextQuestionnaireEndpoint.json" }, ".pages-questions"),

            new Mapping("GetAllQuestionnaires.json"),
            new Mapping("StudyRoleEndpoint.json"),
            new Mapping("StudySettingEndpoint.json"),
            new Mapping("StudySettingStudyCustomEndpoint.json"),
            new Mapping("SubjectInformationEndpoint.json"),
            new Mapping(TranslationsEndpointMappingFile),
            new Mapping("VersionsEndpoint.json"),
            new Mapping("VisitEndpoint.json"),
            new Mapping("VisitIdEndpoint.json"),
            new Mapping("VisitIdEndpointEnrollment.json"),
            new Mapping("VisitIdEndpointTreatment.json")
        };

        public Dictionary<string, Guid> SingleTranslationGuidMappings { get; set; } = new Dictionary<string, Guid>();

        private static readonly HttpClient client = new HttpClient();
        private readonly Assembly assembly = typeof(MockServer).GetTypeInfo().Assembly;

        private string MappingUrl => $@"{wireMockServerUrl}/__admin/mappings";
        private string RequestUrl => $@"{wireMockServerUrl}/__admin/requests";

        public MockServer() { }

        public async Task SetupInitialMappings()
        {
            foreach (var mapping in Mappings)
            {
                await PostMapping(mapping);

                if (!string.IsNullOrWhiteSpace(mapping.PatternFormat))
                {
                    if (mapping.LookupEndpointForBody == null)
                    {
                        await PostSingleGetMappings(mapping);
                    }
                    else
                    {
                        await PostGroupGetMappings(mapping);
                    }
                }
            }
        }

        private async Task PostGroupGetMappings(Mapping mapping)
        {
            foreach (var lookupFile in mapping.LookupEndpointForBody)
            {
                var newMapping = new Mapping("BaseGetTemplateEndpoint.json");
                var mockMapping = await ReadMappingFile(newMapping);
                var lookupMapping = new Mapping(lookupFile);
                var lookUpMappingContents = await ReadMappingFile(lookupMapping);
                var lookUpMappingJObject = JObject.Parse(lookUpMappingContents);

                var nodeData = mapping.NodeToPullDataFrom.Split('-');

                var id = lookUpMappingJObject.SelectToken("Response.BodyAsJson.id");
                var children = lookUpMappingJObject.SelectTokens("Response.BodyAsJson" + nodeData[0]).Children();

                StringBuilder builder = new StringBuilder();

                string idReplacement = mapping.PatternFormat.Replace("{ID}", id.ToString());

                foreach (var cToken in children)
                {
                    var data = cToken;
                    if (nodeData != null && nodeData.Count() == 2)
                    {
                        data = cToken.SelectToken(nodeData[1])[0];
                    }

                    var value = data.ToString();
                    builder = builder.Append(value + ",");
                }

                mockMapping = mockMapping.Replace("<FORMAT>", idReplacement);

                newMapping.Id = Guid.NewGuid().ToString();
                mockMapping = mockMapping.Replace("<BODYREPLACEMENT>", "[" + builder.ToString().TrimEnd(',') + "]");
                await PostMapping(newMapping, overrideMappingFileContents: mockMapping);
            }
        }

        private async Task PostSingleGetMappings(Mapping mapping)
        {
            var newMapping = new Mapping("BaseGetTemplateEndpoint.json");
            var mockMapping = await ReadMappingFile(newMapping);

            string mappingContents = await ReadMappingFile(mapping);
            var map = JObject.Parse(mappingContents);

            var childrenBodyTokens = map.SelectTokens("Response.BodyAsJson").Children();

            foreach (var token in childrenBodyTokens)
            {
                var data = mockMapping;
                var id = token.SelectToken("id").ToString();
                string idReplacement = mapping.PatternFormat.Replace("{ID}", id);
                data = data.Replace("<FORMAT>", idReplacement);

                newMapping.Id = Guid.NewGuid().ToString();
                data = data.ToString().Replace("<BODYREPLACEMENT>", token.ToString());
                await PostMapping(newMapping, overrideMappingFileContents: data);
            }
        }

        public async Task CleanupNotificationMapping()
        {
            await GetAndDeleteMatchingMappings("/notificationScheduler/cancel");
            await GetAndDeleteMatchingMappings("/notificationScheduler/updatepatientalarmstatus");
        }

        public async Task PostMapping(
            Mapping mapping,
            bool updateMapping = false,
            HttpStatusCode? updateHttpStatusCode = null,
            string overrideMappingFileContents = null)
        {
            var mockMapping = overrideMappingFileContents != null ? overrideMappingFileContents : await ReadMappingFile(mapping);

            mockMapping = mockMapping.Replace(MappingGuidPlaceholder, mapping.Id);
            var endpointMapping = JsonConvert.DeserializeObject<MappingModel>(mockMapping);

            if (!string.IsNullOrEmpty(mapping.ResponseBody))
            {
                var responseBodyJson = await new StreamReader(
                        assembly.GetManifestResourceStream(
                            assembly.GetManifestResourceNames().FirstOrDefault(i => i.EndsWith(mapping.ResponseBody)))
                        ).ReadToEndAsync();

                endpointMapping.Response.BodyAsJson = JToken.Parse(responseBodyJson);
            }

            if (updateHttpStatusCode != null)
            {
                endpointMapping.Response.StatusCode = (int)updateHttpStatusCode;
            }

            var content = new StringContent(JsonConvert.SerializeObject(endpointMapping), Encoding.UTF8, "application/json");

            if (updateMapping)
            {
                await client.DeleteAsync($"{MappingUrl}/{endpointMapping.Guid}");
            }

            await client.PostAsync(MappingUrl, content);

            if (mapping.MappingFile == TranslationsEndpointMappingFile)
            {
                await PostTranslations(endpointMapping, updateMapping);
            }
        }

        private async Task<string> ReadMappingFile(Mapping mapping)
        {
            var resourceStream = assembly.GetManifestResourceStream(
                assembly.GetManifestResourceNames().FirstOrDefault(i => i.EndsWith(mapping.MappingFile)));

            var mappingJson = await new StreamReader(resourceStream)
                .ReadToEndAsync();

            return mappingJson;
        }

        private async Task PostTranslations(MappingModel allTranslationsMapping, bool updateMapping = false)
        {
            var baseSingleTranslationMapping = new Mapping(SingleTranslationEndpointMappingFile);
            var baseTranslationWithLanguageIdMapping = new Mapping(TranslationWithLanguageIdEndpointMappingFile);

            var baseSingleTranslationJson = await ReadMappingFile(baseSingleTranslationMapping);
            var baseTranslationWithLanguageIdJson = await ReadMappingFile(baseTranslationWithLanguageIdMapping);

            var allTranslations = JsonConvert.DeserializeObject<List<TranslationModel>>(allTranslationsMapping.Response.BodyAsJson.ToString());

            var newMappings = new List<MappingModel>();

            foreach (var translation in allTranslations)
            {
                var singleTranslationMappingGuid = Guid.NewGuid();
                var translationWithLanguageIdMappingGuid = Guid.NewGuid();

                if (updateMapping && SingleTranslationGuidMappings.ContainsKey(translation.Id))
                {
                    singleTranslationMappingGuid = SingleTranslationGuidMappings[translation.Id];
                }

                var serializedTranslation = JsonConvert.SerializeObject(translation);

                var newSingleMappingJson = baseSingleTranslationJson.Replace(MappingGuidPlaceholder, singleTranslationMappingGuid.ToString());
                newSingleMappingJson = newSingleMappingJson.Replace(MappingResourceKeyPlaceholder, translation.Id);
                newSingleMappingJson = newSingleMappingJson.Replace(MappingResourceValuePlaceholder, serializedTranslation);

                var newTranslationWithLanguageIdMappingJson = baseTranslationWithLanguageIdJson.Replace(MappingGuidPlaceholder, translationWithLanguageIdMappingGuid.ToString());
                newTranslationWithLanguageIdMappingJson = newTranslationWithLanguageIdMappingJson.Replace(MappingResourceKeyPlaceholder, translation.Id);
                newTranslationWithLanguageIdMappingJson = newTranslationWithLanguageIdMappingJson.Replace(MappingResourceValuePlaceholder, serializedTranslation);
                newTranslationWithLanguageIdMappingJson = newTranslationWithLanguageIdMappingJson.Replace(MappingLanguageIdPlaceholder, translation.LanguageKey.ToString());

                var singleEndpointMapping = JsonConvert.DeserializeObject<MappingModel>(newSingleMappingJson);
                var multipleEndpointMapping = JsonConvert.DeserializeObject<MappingModel>(newTranslationWithLanguageIdMappingJson);

                newMappings.Add(singleEndpointMapping);
                newMappings.Add(multipleEndpointMapping);
            }

            var content = new StringContent(
                JsonConvert.SerializeObject(newMappings),
                Encoding.UTF8,
                "application/json");

            if (updateMapping)
            {
                await client.PutAsync(MappingUrl, content);
            }
            else
            {
                await client.PostAsync(MappingUrl, content);
            }
        }

        public async Task StudySettingEndpoint(string responseFile)
        {
            var mapping = Mappings.SingleOrDefault(m => m.MappingFile == StudySettingsDefaultMappingFile);
            mapping.ResponseBody = responseFile;

            await PostMapping(mapping, true);
        }


        public async Task LanguageEndpoint(string responseFile)
        {
            var mapping = Mappings.SingleOrDefault(m => m.MappingFile == LanguageEndpointMappingFile);
            mapping.ResponseBody = responseFile;

            await PostMapping(mapping, true);
        }

        public async Task UpdateStudyCustomValue(string key, string value)
        {
            var mapping = await GetAndDeleteMatchingMappings("/StudySetting/StudyCustom");

            if (mapping == null)
            {
                throw new NotImplementedException();
            }

            var studyCustoms = JsonConvert.DeserializeObject<List<StudyCustomModel>>(mapping.Response.BodyAsJson.ToString());

            var matchingStudyCustom = studyCustoms.FirstOrDefault(sc => sc.Key == key);
            if (matchingStudyCustom == null)
            {
                matchingStudyCustom = new StudyCustomModel()
                {
                    Key = key,
                    Group = "Added Via E2E Tests",
                    Notes = null
                };

                studyCustoms.Add(matchingStudyCustom);
            }

            matchingStudyCustom.Value = value;

            mapping.Response.BodyAsJson = studyCustoms;

            var serializedMapping = JsonConvert.SerializeObject(mapping);

            var content = new StringContent(serializedMapping, Encoding.UTF8, "application/json");

            await client.PostAsync($"{MappingUrl}", content);
        }

        public async Task UpdateQuestionnaireMockedValues(string newValue, string lookUpValue, string question, string property, string mockToUpdate, string questionnaireId)
        {
            MappingModel mapping = await GetAndDeleteMatchingMappings($"*/Questionnaire/deepLoadedQuestionnaire/{questionnaireId}*");

            if (mapping == null)
            {
                throw new NotImplementedException();
            }

            var value = JsonConvert.DeserializeObject<QuestionnaireModel>(mapping.Response.BodyAsJson.ToString());

            switch (mockToUpdate.ToLower())
            {
                case "choice":
                         
                    var choices = value.Pages.SelectMany(x => x.Questions.Select(y => y.Choices)).SelectMany(c => c);
                    Guid questionId = Guid.Parse(question);
                    var choiceToUpdate = choices.FirstOrDefault(x => x.HtmlFreeDisplayText == lookUpValue && x.QuestionId == questionId);

                    switch(property.ToLower())
                    {
                        case "clear other responses":
                            choiceToUpdate.ClearOtherResponses = Convert.ToBoolean(newValue);
                            break;
                    }          

                    break;

                case "maxtext":                    
                    var questions = value.Pages.SelectMany(x => x.Questions);
                    questionId = Guid.Parse(question);
                    var questionToUpdate = questions.FirstOrDefault(x => x.Id == questionId);          
                    questionToUpdate.QuestionSettings.MaxValue = newValue;              

                    break;

                default:
                    break;
            }


            mapping.Response.BodyAsJson = value;

            var serializedMapping = JsonConvert.SerializeObject(mapping);

            var content = new StringContent(serializedMapping, Encoding.UTF8, "application/json");

            await client.PostAsync($"{MappingUrl}", content);
        }

        public async Task UpdateNotificationStatusCodeValue(int value)
        {
            List<string> notificationMappings = new List<string>() { "NotificationScheduleCancelEndPoint.json", "NotificationScheduleUpdateStatusEndPoint.json" };

            foreach (var mappings in notificationMappings)
            {
                var mapping = new Mapping(mappings);
                var mockMapping = await ReadMappingFile(mapping);

                mockMapping = mockMapping.Replace(MappingGuidPlaceholder, mapping.Id);
                var endpointMapping = JsonConvert.DeserializeObject<MappingModel>(mockMapping);

                endpointMapping.Response.StatusCode = value.ToString();

                var serializedMapping = JsonConvert.SerializeObject(endpointMapping);

                var content = new StringContent(serializedMapping, Encoding.UTF8, "application/json");

                await client.PostAsync($"{MappingUrl}", content);
            }
        }

        private async Task<MappingModel> GetAndDeleteMatchingMappings(string expectedPattern)
        {
            var mappingsJson = await client.GetAsync(MappingUrl);
            var allMappings = JsonConvert.DeserializeObject<IList<MappingModel>>(await mappingsJson.Content.ReadAsStringAsync());

            var matchingMappings = new List<MappingModel>();

            foreach (var mapping in allMappings)
            {
                var path = JsonConvert.DeserializeObject<PathModel>(mapping.Request.Path.ToString());

                foreach (var matcher in path.Matchers)
                {
                    var pattern = matcher.Pattern.ToString();
                    if (pattern.EndsWith(expectedPattern))
                    {
                        matchingMappings.Add(mapping);
                    }
                }
            }

            foreach (var matchingMapping in matchingMappings)
            {
                await client.DeleteAsync($"{MappingUrl}/{matchingMapping.Guid}");
            }

            return matchingMappings.FirstOrDefault();
        }

        public async Task ClearMockResources()
        {
            var emptyContent = new StringContent(string.Empty);
            await client.PostAsync($"{MappingUrl}/reset", emptyContent);
            await client.PostAsync($"{RequestUrl}/reset", emptyContent);
        }

        public string GetLocalIPAddress()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            return localIP ?? throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public async Task<LogRequestModel> GetLatestCancelNotificationsRequest()
        {
            // update patient temp cancels the alarms on the server so they could be restarted if went back to active status. 
            var allMockRequests = await GetAllRequestLogs("notificationScheduler/cancel");
            var latestRequest = allMockRequests
                .FirstOrDefault();

            return latestRequest;
        }

        public async Task<LogRequestModel> GetLatestUpdateStatusNotificationsRequest()
        {
            // update patient temp cancels the alarms on the server so they could be restarted if went back to active status. 
            var allMockRequests = await GetAllRequestLogs("notificationScheduler/updatepatientalarmstatus");
            var latestRequest = allMockRequests
                .FirstOrDefault();

            return latestRequest;
        }

        private async Task<List<LogRequestModel>> GetAllRequestLogs(
            string endPointRequestPath)
        {
            var requestsRequest = await client.GetAsync(RequestUrl);
            var requestsBody = await requestsRequest.Content.ReadAsStringAsync();
            var allMockRequests = JsonConvert.DeserializeObject<List<LogEntryModel>>(requestsBody);

            var logModels = allMockRequests
                .Where(mr =>
                    mr?.Request != null &&
                    mr.Request.Path.EndsWith(endPointRequestPath))
                .OrderByDescending(mr => mr.Request.DateTime)
                .Select(mr => mr.Request)
                .ToList();

            return logModels;
        }
    }
}
