namespace Rentals.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        OpeningBalance = c.Decimal(nullable: false, precision: 10, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        PayeeId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 10, scale: 2),
                        Taxable = c.Boolean(nullable: false),
                        Reference = c.String(maxLength: 30),
                        Memo = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Payees", t => t.PayeeId)
                .Index(t => t.AccountId)
                .Index(t => t.PayeeId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        DefaultCategoryId = c.Int(nullable: false),
                        Memo = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.DefaultCategoryId)
                .Index(t => t.DefaultCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "PayeeId", "dbo.Payees");
            DropForeignKey("dbo.Payees", "DefaultCategoryId", "dbo.Categories");
            DropForeignKey("dbo.Transactions", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Transactions", "AccountId", "dbo.Accounts");
            DropIndex("dbo.Payees", new[] { "DefaultCategoryId" });
            DropIndex("dbo.Transactions", new[] { "CategoryId" });
            DropIndex("dbo.Transactions", new[] { "PayeeId" });
            DropIndex("dbo.Transactions", new[] { "AccountId" });
            DropTable("dbo.Payees");
            DropTable("dbo.Categories");
            DropTable("dbo.Transactions");
            DropTable("dbo.Accounts");
        }
    }
}
