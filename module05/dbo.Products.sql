CREATE TABLE [dbo].[Products] (
    [Product_ID]                 INT NOT NULL,
    [Article_ID]                 INT NOT NULL,
    [BlogArticleEntityArticleId] INT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Product_ID] ASC, [Article_ID] ASC),
    CONSTRAINT [FK_Products_Articles_BlogArticleEntityArticleId] FOREIGN KEY ([BlogArticleEntityArticleId]) REFERENCES [dbo].[Articles] ([Article_id])  ON DELETE CASCADE,
);


GO
CREATE NONCLUSTERED INDEX [IX_Products_BlogArticleEntityArticleId]
    ON [dbo].[Products]([BlogArticleEntityArticleId] ASC);

