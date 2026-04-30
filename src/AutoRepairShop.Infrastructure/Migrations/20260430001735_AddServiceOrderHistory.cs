using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRepairShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceOrderHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceOrderHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrderHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrderHistories_ServiceOrders_ServiceOrderId",
                        column: x => x.ServiceOrderId,
                        principalTable: "ServiceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderHistories_CreatedAt",
                table: "ServiceOrderHistories",
                column: "CreatedAt"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrderHistories_ServiceOrderId",
                table: "ServiceOrderHistories",
                column: "ServiceOrderId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ServiceOrderHistories");
        }
    }
}
