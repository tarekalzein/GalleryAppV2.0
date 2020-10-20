namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editedMediaFile : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MediaFiles", "Album_AlbumID", "dbo.Albums");
            DropIndex("dbo.MediaFiles", new[] { "Album_AlbumID" });
            AddColumn("dbo.MediaFiles", "Album_AlbumID1", c => c.Int());
            AlterColumn("dbo.MediaFiles", "Album_AlbumID", c => c.Int(nullable: false));
            CreateIndex("dbo.MediaFiles", "Album_AlbumID1");
            AddForeignKey("dbo.MediaFiles", "Album_AlbumID1", "dbo.Albums", "AlbumID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaFiles", "Album_AlbumID1", "dbo.Albums");
            DropIndex("dbo.MediaFiles", new[] { "Album_AlbumID1" });
            AlterColumn("dbo.MediaFiles", "Album_AlbumID", c => c.Int());
            DropColumn("dbo.MediaFiles", "Album_AlbumID1");
            CreateIndex("dbo.MediaFiles", "Album_AlbumID");
            AddForeignKey("dbo.MediaFiles", "Album_AlbumID", "dbo.Albums", "AlbumID");
        }
    }
}
