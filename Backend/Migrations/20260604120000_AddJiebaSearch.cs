using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class AddJiebaSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_jieba;");

            migrationBuilder.Sql("""
                ALTER TABLE "SystemRules"
                    ADD COLUMN IF NOT EXISTS "SearchVector" tsvector;
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS idx_systemrules_search_vector
                    ON "SystemRules" USING GIN ("SearchVector");
                """);

            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION update_systemrules_search_vector()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW."SearchVector" := to_tsvector('jiebacfg',
                        COALESCE(NEW."SystemName", '') || ' ' ||
                        COALESCE(NEW."System", '') || ' ' ||
                        COALESCE(NEW."RuleDescription", '')
                    );
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql("""
                DROP TRIGGER IF EXISTS systemrules_search_vector_trigger ON "SystemRules";
                CREATE TRIGGER systemrules_search_vector_trigger
                    BEFORE INSERT OR UPDATE ON "SystemRules"
                    FOR EACH ROW EXECUTE FUNCTION update_systemrules_search_vector();
                """);

            migrationBuilder.Sql("""
                UPDATE "SystemRules"
                SET "SearchVector" = to_tsvector('jiebacfg',
                    COALESCE("SystemName", '') || ' ' ||
                    COALESCE("System", '') || ' ' ||
                    COALESCE("RuleDescription", '')
                );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP TRIGGER IF EXISTS systemrules_search_vector_trigger ON "SystemRules";""");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_systemrules_search_vector();");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS idx_systemrules_search_vector;""");
            migrationBuilder.Sql("""ALTER TABLE "SystemRules" DROP COLUMN IF EXISTS "SearchVector";""");
        }
    }
}
