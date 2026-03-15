--Retrieves All projects of a User 
CREATE PROCEDURE GetProjectByContributor @User int AS 
DECLARE @projectid INT;
SELECT @projectid=projectId FROM Users WHERE id=@User; 
SELECT Projects.Id FROM Projects WHERE id=@projectid;
GO;

--Retrieves All projects with the spec
