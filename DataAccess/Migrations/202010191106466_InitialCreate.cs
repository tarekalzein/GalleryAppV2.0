namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        AlbumID = c.Int(nullable: false, identity: true),
                        AlbumTitle = c.String(),
                        AlbumDescription = c.String(),
                        AlbumImage = c.String(),
                    })
                .PrimaryKey(t => t.AlbumID);
            
            CreateTable(
                "dbo.MediaFiles",
                c => new
                    {
                        FileID = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Description = c.String(),
                        FilePath = c.String(),
                        FileThumbnail = c.String(),
                        Time = c.Int(nullable: false),
                        PlayEnabled = c.Boolean(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Album_AlbumID = c.Int(),
                    })
                .PrimaryKey(t => t.FileID)
                .ForeignKey("dbo.Albums", t => t.Album_AlbumID)
                .Index(t => t.Album_AlbumID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaFiles", "Album_AlbumID", "dbo.Albums");
            DropIndex("dbo.MediaFiles", new[] { "Album_AlbumID" });
            DropTable("dbo.MediaFiles");
            DropTable("dbo.Albums");
        }
    }
}
