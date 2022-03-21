CREATE TABLE [dbo].[Articles] (
    [Article_id]       INT            IDENTITY (1, 1) NOT NULL,
    [Article_Title]    NVARCHAR (MAX) NULL,
    [Article_Text]     NVARCHAR (MAX) NULL,
    [Publication_Date] DATETIME2 (7)  NOT NULL,
    [Employee_id]      INT            NOT NULL,
    CONSTRAINT [PK_Articles] PRIMARY KEY CLUSTERED ([Article_id] ASC)
);

