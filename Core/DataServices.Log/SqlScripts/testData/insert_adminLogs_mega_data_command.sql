DECLARE @i int = 0
DECLARE @the_date as datetimeoffset
WHILE @i < 1000000 
BEGIN
	SET @the_date = DATEADD(ns ,FLOOR(RAND()*10000) ,SYSDATETIMEOFFSET())
	INSERT INTO dbo.AdminLogs values(@the_date,NEWID(),NEWID(),'127.0.0.1','path','get','Description','Exception','StackTrace','ServerName','Connection: keep-alive
Content-Type: application/json
Accept: */*
Accept-Encoding: gzip, deflate, br
Cookie: .AspNetCore.Antiforgery.gvRURe431Yg=CfDJ8N5fTH4fC_RIt09lM4n6AyP7oKpsKUaMvSZtNM2qIUpQ-3hs7fRxB6gV92oN8SVfTHIQFvTGe7jlYIBXlfHomc5c0NIPNAMCxd5LW_lyf5_EuDC0mMWjLZyS8ETN59fJxnm8lSWkW0jH4yO4s1MYr6o; X-CSRF-TOKEN=CfDJ8N5fTH4fC_RIt09lM4n6AyNkFiDDlgYeSKNZBTUgfZ3smQfmBTjwZQzDkvJ__jDGHwMgTjox3YtEd5kmjvU1wAjC2CUJlqTY1JpiweMn5ztwrrWwrHLM_eRm7fupROe5WTBStAiS2buHWqliq21RcFJpl3kQiB61FpAp6uWW2SyiwMjxEYwW4zI-QAuP4ROrCg; .AspNetCore.Cookies=CfDJ8N5fTH4fC_RIt09lM4n6AyP4osiYsHa6FnUhvAqk9JEKUNsaII1KFhyOtKx8m0i_AvLTRQ5HNjACtgneJNZTU88lyQteg7RsAzIW-2O9xGoS71rMVIxyHbXmpcy3mIRIo2slaHED8vJoNiYY2TgYBC9lBPWGMprQtc9VgUnS-qF_nDz_LQ4V7bBGMFPjfQ-jEqtDjLNg5fdpW4zPRwghL_WWvf-jSMZ-kZ6lYz2kOR25ur8lYc1yMzJY_UMLuY57iaV3QpTYbVbNQb0FfgyixriRXqUmUnVV4zDWxNHFHKvLzMi5NZVVwSldZwTYzM5qjSBCNmilqxny4VZLT3pFk12ZBKcGbjBL08robFpGeTbaFedWHABq-wWIipPfstVEYZFVE-IfEJL3U66UeOHYpBkkKqsOOwQ1sQvlxu3NUQ2a9uwF-yxQqKkQzjqWMdp9ji1MhqB1cDoippblhS6f3ws
Host: localhost:5001
User-Agent: PostmanRuntime/7.26.5
Content-Length: 171
Postman-Token: f50c57a1-5e6e-4c3a-9ae5-7095cf624d07','{
    "from": "2020-10-21T00:00:00",
    "to": "2020-10-22T00:00:00",
    "pageSize":"10",
    "pageNumber": "1",
    "sortBy": "recordedOn",
    "isDesc": "true"
}','','{"totalCount":0,"items":[],"valid":true,"message":"","errors":[]}',200,5)
    SET @i = @i + 1
END