namespace Assignment4_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoredProcedure : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "dbo.Customer_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 100),
                        DateOfBirth = p.DateTime(),
                        Amount = p.Decimal(precision: 18, scale: 2),
                    },
                body:
                    @"INSERT [dbo].[Customers]([Name], [DateOfBirth], [Amount])
                      VALUES (@Name, @DateOfBirth, @Amount)
                      
                      DECLARE @ID int
                      SELECT @ID = [ID]
                      FROM [dbo].[Customers]
                      WHERE @@ROWCOUNT > 0 AND [ID] = scope_identity()
                      
                      SELECT t0.[ID], t0.[RowVersion]
                      FROM [dbo].[Customers] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ID] = @ID"
            );
            
            CreateStoredProcedure(
                "dbo.Customer_Update",
                p => new
                    {
                        ID = p.Int(),
                        Name = p.String(maxLength: 100),
                        DateOfBirth = p.DateTime(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                        Amount = p.Decimal(precision: 18, scale: 2),
                    },
                body:
                    @"UPDATE [dbo].[Customers]
                      SET [Name] = @Name, [DateOfBirth] = @DateOfBirth, [Amount] = @Amount
                      WHERE (([ID] = @ID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))
                      
                      SELECT t0.[RowVersion]
                      FROM [dbo].[Customers] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ID] = @ID"
            );
            
            CreateStoredProcedure(
                "dbo.Customer_Delete",
                p => new
                    {
                        ID = p.Int(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                    },
                body:
                    @"DELETE [dbo].[Customers]
                      WHERE (([ID] = @ID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.Customer_Delete");
            DropStoredProcedure("dbo.Customer_Update");
            DropStoredProcedure("dbo.Customer_Insert");
        }
    }
}
