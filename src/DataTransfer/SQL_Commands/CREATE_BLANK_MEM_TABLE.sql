Use [PerformanceData]
GO
DROP TABLE IF EXISTS MEM_Data
CREATE TABLE MEM_Data (
"usage" BIGINT,
"timestamp" DATETIME PRIMARY KEY,
"appId" int,
CONSTRAINT [FK_MEM_Session_ID] FOREIGN KEY ("appID") REFERENCES Session(id)
)