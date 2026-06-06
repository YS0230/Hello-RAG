using System.Globalization;

namespace Backend.Queries;

public static class RuleSearchQuery
{
    public static FormattableString Build(float[] floats, string queryText, int topN)
    {
        var vecLiteral = "[" + string.Join(",",
            floats.Select(f => f.ToString("G", CultureInfo.InvariantCulture))) + "]";

        return $"""
            WITH vector_hits AS (
                SELECT "Id",
                       ROW_NUMBER() OVER (ORDER BY "Embedding" <=> {vecLiteral}::vector) AS rank,
                       1.0 - ("Embedding" <=> {vecLiteral}::vector) AS similarity
                FROM "SystemRules"
                WHERE "Embedding" IS NOT NULL
                  AND "Embedding" <=> {vecLiteral}::vector < 0.35
                LIMIT 40
            ),
            text_hits AS (
                SELECT "Id",
                       ROW_NUMBER() OVER (
                           ORDER BY ts_rank_cd("SearchVector", plainto_tsquery('jiebacfg', {queryText})) DESC
                       ) AS rank,
                       ts_rank_cd("SearchVector", plainto_tsquery('jiebacfg', {queryText})) AS text_score
                FROM "SystemRules"
                WHERE "SearchVector" IS NOT NULL
                  AND "SearchVector" @@ plainto_tsquery('jiebacfg', {queryText})
                LIMIT 40
            ),
            combined AS (
                SELECT
                    COALESCE(v."Id", t."Id") AS "Id",
                    COALESCE(v.similarity, 0.0) AS vector_similarity,
                    COALESCE(t.text_score, 0.0) AS text_score,
                    CASE
                        WHEN v.rank IS NOT NULL AND t.rank IS NOT NULL THEN
                            (1.0 / (60.0 + v.rank)) * v.similarity * 0.4 +
                            (1.0 / (60.0 + t.rank)) * t.text_score * 0.6 +
                            0.005
                        WHEN v.rank IS NOT NULL THEN
                            (1.0 / (60.0 + v.rank)) * v.similarity
                        WHEN t.rank IS NOT NULL THEN
                            (1.0 / (60.0 + t.rank)) * t.text_score
                    END AS rrf_score
                FROM vector_hits v
                FULL JOIN text_hits t ON t."Id" = v."Id"
            )
            SELECT r."Id", r."SystemName", r."SerialNumber", r."System",
                   r."RuleDescription", r."Recorder", r."CreatedAt",
                   CAST(c.rrf_score AS float8) AS "Similarity",
                   CAST(c.vector_similarity AS float8) AS "VectorSimilarity",
                   CAST(c.text_score AS float8) AS "TextScore",
                   CAST(r."SearchVector" AS text) AS "SearchVector"
            FROM combined c
            JOIN "SystemRules" r ON r."Id" = c."Id"
            WHERE c.rrf_score > 0.002
            ORDER BY c.rrf_score DESC
            LIMIT {topN}
            """;
    }
}
