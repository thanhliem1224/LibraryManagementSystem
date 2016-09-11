namespace LibraryManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChuDe",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TenChuDe = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Sach",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ChuDeID = c.Int(nullable: false),
                        SachID = c.String(nullable: false),
                        TenSach = c.String(nullable: false),
                        SoLuong = c.Int(nullable: false),
                        TrangThai = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ChuDe", t => t.ChuDeID, cascadeDelete: true)
                .Index(t => t.ChuDeID);
            
            CreateTable(
                "dbo.MuonTraSach",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SachID = c.Int(nullable: false),
                        HocSinhID = c.Int(nullable: false),
                        NgayMuon = c.DateTime(nullable: false),
                        HanTra = c.DateTime(nullable: false),
                        NgayTra = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HocSinh", t => t.HocSinhID, cascadeDelete: true)
                .ForeignKey("dbo.Sach", t => t.SachID, cascadeDelete: true)
                .Index(t => t.SachID)
                .Index(t => t.HocSinhID);
            
            CreateTable(
                "dbo.HocSinh",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TenHS = c.String(nullable: false),
                        Lop = c.String(nullable: false),
                        NgaySinh = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ThanhLy",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SachID = c.Int(nullable: false),
                        Ngay = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Sach", t => t.SachID, cascadeDelete: true)
                .Index(t => t.SachID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThanhLy", "SachID", "dbo.Sach");
            DropForeignKey("dbo.MuonTraSach", "SachID", "dbo.Sach");
            DropForeignKey("dbo.MuonTraSach", "HocSinhID", "dbo.HocSinh");
            DropForeignKey("dbo.Sach", "ChuDeID", "dbo.ChuDe");
            DropIndex("dbo.ThanhLy", new[] { "SachID" });
            DropIndex("dbo.MuonTraSach", new[] { "HocSinhID" });
            DropIndex("dbo.MuonTraSach", new[] { "SachID" });
            DropIndex("dbo.Sach", new[] { "ChuDeID" });
            DropTable("dbo.ThanhLy");
            DropTable("dbo.HocSinh");
            DropTable("dbo.MuonTraSach");
            DropTable("dbo.Sach");
            DropTable("dbo.ChuDe");
        }
    }
}
