USE PerformanceData
GO
DROP TABLE IF EXISTS GC
CREATE TABLE GC(
"type" VARCHAR(MAX), 
"id" INTEGER,
"timestamp" DATETIME,
"appId" int,
CONSTRAINT [FK_GC_Session_ID] FOREIGN KEY ("appID") REFERENCES Session(id)
)
