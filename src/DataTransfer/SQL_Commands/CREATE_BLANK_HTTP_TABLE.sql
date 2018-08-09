USE PerformanceData
GO
DROP TABLE IF EXISTS Http_Request
CREATE TABLE Http_Request (
"type" VARCHAR(MAX), 
"method" VARCHAR(MAX), 
"path" VARCHAR(MAX),
"activityID" UNIQUEIDENTIFIER,
"timestamp" DATETIME,
"appId" int,
"id" int IDENTITY(1,1) not null,
CONSTRAINT [FK_HTTP_Session_ID] FOREIGN KEY ("appId") REFERENCES Session(id) ON DELETE CASCADE
)
