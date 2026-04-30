using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowerEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductAddProductDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "PublicUrl",
                table: "FileAttachments");

            migrationBuilder.RenameColumn(
                name: "ThumbnailPath",
                table: "FileAttachments",
                newName: "AltText");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "FileAttachments",
                newName: "Bytes");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "FileAttachments",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "FileAttachments",
                newName: "SecureUrl");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Categories",
                newName: "Slug");

            migrationBuilder.AddColumn<bool>(
                name: "IsContactPrice",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOutOfStock",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductDetailId",
                table: "Products",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "FileAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "FileAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "FileAttachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "FileAttachments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LastModifierId",
                table: "FileAttachments",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "FileAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "FileAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "FileAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductDetail",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSizePrices",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductDetailId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    CreatorId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSizePrices_ProductDetail_ProductDetailId",
                        column: x => x.ProductDetailId,
                        principalTable: "ProductDetail",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductDetailId",
                table: "Products",
                column: "ProductDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizePrices_ProductDetailId",
                table: "ProductSizePrices",
                column: "ProductDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductDetail_ProductDetailId",
                table: "Products",
                column: "ProductDetailId",
                principalTable: "ProductDetail",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductDetail_ProductDetailId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductSizePrices");

            migrationBuilder.DropTable(
                name: "ProductDetail");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductDetailId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsContactPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsOutOfStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductDetailId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "FileAttachments");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "FileAttachments",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "SecureUrl",
                table: "FileAttachments",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "Bytes",
                table: "FileAttachments",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "AltText",
                table: "FileAttachments",
                newName: "ThumbnailPath");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Categories",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "FileAttachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicUrl",
                table: "FileAttachments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
