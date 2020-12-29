IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'IntValuesType')
DROP TYPE [dbo].[IntValuesType];

CREATE TYPE [dbo].[IntValuesType] AS TABLE
(
     [Value] [int] NOT NULL
);
