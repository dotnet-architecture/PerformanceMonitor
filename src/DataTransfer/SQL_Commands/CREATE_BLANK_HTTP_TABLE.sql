USE PerformanceData
GO
DROP TABLE IF EXISTS Http_Request
CREATE TABLE Http_Request (
"type" VARCHAR(MAX), 
"method" VARCHAR(MAX), 
"path" VARCHAR(MAX),
"id" UNIQUEIDENTIFIER,
"timestamp" DATETIME PRIMARY KEY,
"appId" int,
CONSTRAINT [FK_HTTP_Session_ID] FOREIGN KEY ("appID") REFERENCES Session(id)
)
