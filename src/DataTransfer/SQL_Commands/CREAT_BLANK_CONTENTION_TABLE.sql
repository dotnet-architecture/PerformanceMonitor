Use [PerformanceData]
GO
DROP TABLE IF EXISTS Contention
CREATE TABLE Contention (
"type" VARCHAR(MAX), 
"timestamp" DATETIME PRIMARY KEY,
"appId" int,
"id" uniqueidentifier,
CONSTRAINT [FK_Session_ID] FOREIGN KEY ("appID") REFERENCES Session(id) ON DELETE CASCADE
)
