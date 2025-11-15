using EntreLaunch.Elastic;
using EntreLaunch.Entities;

namespace EntreLaunch.Elastic.Migrations;

[ElasticMigration("20230131211337_ReIndexDomain")]
public class ReIndexDomain : ElasticMigration
{
    public override async Task EntreLaunch(ElasticDbContext context)
    {
        await ElasticHelper.MigrateIndex(context, typeof(Domain));
    }
}
