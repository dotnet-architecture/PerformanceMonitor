USE PerformanceData
GO
DROP TABLE IF EXISTS Exception_Data
CREATE TABLE Exception_Data (
"type" VARCHAR(MAX), 
"timestamp" DATETIME,
"appId" int,
CONSTRAINT [FK_EXCEPTION_Session_ID] FOREIGN KEY ("appID") REFERENCES Session(id) ON DELETE CASCADE
)