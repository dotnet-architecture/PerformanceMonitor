USE PerformanceData
GO
DROP TABLE IF EXISTS CPU_Usage
CREATE TABLE CPU_Usage (
"usage" FLOAT, 
"timestamp" DATETIME PRIMARY KEY,
"appId" int,
CONSTRAINT [FK_CPU_Session_ID] FOREIGN KEY ("appID") REFERENCES Session(id) 
)