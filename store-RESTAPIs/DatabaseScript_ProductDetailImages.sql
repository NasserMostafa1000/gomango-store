-- =============================================
-- Script to create ProductDetailImages table
-- =============================================

-- Create ProductDetailImages table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductDetailImages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductDetailImages] (
        [ProductDetailImageId] INT IDENTITY(1,1) NOT NULL,
        [ProductDetailsId] INT NOT NULL,
        [ImageUrl] VARCHAR(500) NOT NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_ProductDetailImages] PRIMARY KEY CLUSTERED ([ProductDetailImageId] ASC),
        CONSTRAINT [FK_ProductDetailImages_ProductsDetails] FOREIGN KEY ([ProductDetailsId])
            REFERENCES [dbo].[ProductsDetails] ([ProductDetailsId])
            ON DELETE CASCADE
    );

    -- Create index for better query performance
    CREATE NONCLUSTERED INDEX [IX_ProductDetailImages_ProductDetailsId]
        ON [dbo].[ProductDetailImages] ([ProductDetailsId] ASC);

    PRINT 'Table ProductDetailImages created successfully.';
END
ELSE
BEGIN
    PRINT 'Table ProductDetailImages already exists.';
END
GO

