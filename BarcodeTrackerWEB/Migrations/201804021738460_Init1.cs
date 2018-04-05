namespace BarcodeTrackerWEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderDetails", "startSequence", c => c.String());
            AlterColumn("dbo.OrderDetails", "endSequence", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderDetails", "endSequence", c => c.Long(nullable: false));
            AlterColumn("dbo.OrderDetails", "startSequence", c => c.Long(nullable: false));
        }
    }
}
