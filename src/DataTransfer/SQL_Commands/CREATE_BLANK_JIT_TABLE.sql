Use [PerformanceData]
GO
DROP TABLE IF EXISTS Jit
CREATE TABLE Jit (
"method" VARCHAR(MAX), 
"timestamp" DATETIME,
"appId" int,
CONSTRAINT [FK_JIT_Session_ID] FOREIGN KEY ("appID") REFERENCES Session(id) ON DELETE CASCADE
)
