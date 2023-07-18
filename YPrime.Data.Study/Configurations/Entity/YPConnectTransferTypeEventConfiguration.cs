using System;
using System.Data.Entity.Migrations;
using YPrime.Data.Study.Models.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class YPConnectTransferTypeEventConfiguration
    {
        public static void Seed(StudyDbContext context)
        {
            context.YPConnectTransferTypeEvents.AddOrUpdate(x => x.Id, new []
            {
                new YPConnectTransferTypeEvent()
                {
                    Id = new Guid("8DA7101E-AD26-4C35-B7F7-0DF3B4FF008A"),
                    YPConnectTransferTypeId = 1
                },
                new YPConnectTransferTypeEvent()
                {
                    Id = new Guid("802DC67E-F283-48D6-9377-4DCF0A3B1333"),
                    YPConnectTransferTypeId = 17
                },
                new YPConnectTransferTypeEvent()
                {
                    Id = new Guid("E3EB10F0-D558-4BDC-8787-8681453FC855"),
                    YPConnectTransferTypeId = 18
                },
                new YPConnectTransferTypeEvent()
                {
                    Id = new Guid("7B77C522-AA4A-4EBF-BA8D-BAA234E5656C"),
                    YPConnectTransferTypeId = 20
                },
                new YPConnectTransferTypeEvent()
                {
                    Id = new Guid("3A97EC23-CAA4-4500-96F1-D58DB6CAB5C9"),
                    YPConnectTransferTypeId = 24
                },
                new YPConnectTransferTypeEvent()
                {
                    Id = new Guid("036B8336-AA59-4CAF-B35D-EDFD7EA153CF"),
                    YPConnectTransferTypeId = 22
                },
            });
        }
    }
}