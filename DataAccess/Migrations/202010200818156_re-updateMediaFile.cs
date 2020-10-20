namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reupdateMediaFile : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MediaFiles", "Album_AlbumID1", "dbo.Albums");
            DropIndex("dbo.MediaFiles", new[] { "Album_AlbumID1" });
            RenameColumn(table: "dbo.MediaFiles", name: "Album_AlbumID1", newName: "AlbumID");
            AlterColumn("dbo.MediaFiles", "AlbumID", c => c.Int(nullable: false));
            CreateIndex("dbo.MediaFiles", "AlbumID");
            AddForeignKey("dbo.MediaFiles", "AlbumID", "dbo.Albums", "AlbumID", cascadeDelete: true);
            DropColumn("dbo.MediaFiles", "Album_AlbumID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MediaFiles", "Album_AlbumID", c => c.Int(nullable: false));
            DropForeignKey("dbo.MediaFiles", "AlbumID", "dbo.Albums");
            DropIndex("dbo.MediaFiles", new[] { "AlbumID" });
            AlterColumn("dbo.MediaFiles", "AlbumID", c => c.Int());
            RenameColumn(table: "dbo.MediaFiles", name: "AlbumID", newName: "Album_AlbumID1");
            CreateIndex("dbo.MediaFiles", "Album_AlbumID1");
            AddForeignKey("dbo.MediaFiles", "Album_AlbumID1", "dbo.Albums", "AlbumID");
        }
    }
}
