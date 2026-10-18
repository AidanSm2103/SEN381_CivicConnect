using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CivicConnect.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceNoSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DEF-008: database sequence for reference numbers. Starts after the highest existing number
            // so existing references stay valid and no new one can clash with them.
            migrationBuilder.Sql(@"
                DECLARE @start bigint = 1 + ISNULL((
                    SELECT MAX(TRY_CAST(SUBSTRING(ReferenceNo, 9, 20) AS bigint))
                    FROM dbo.ServiceRequests
                    WHERE ReferenceNo LIKE 'CC-____-%'), 0);
                DECLARE @sql nvarchar(200) = N'CREATE SEQUENCE dbo.ServiceRequestRefSeq AS bigint START WITH '
                    + CAST(@start AS nvarchar(20)) + N' INCREMENT BY 1;';
                EXEC sp_executesql @sql;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS dbo.ServiceRequestRefSeq;");
        }
    }
}
