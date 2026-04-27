using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRepairShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceAndSupplyTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert test Services
            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Name", "Description", "Price" },
                values: new object[,]
                {
                    {
                        new Guid("33333333-3333-3333-3333-333333333333"),
                        "Troca de Óleo",
                        "Troca de óleo e filtro do motor",
                        150.00m,
                    },
                    {
                        new Guid("44444444-4444-4444-4444-444444444444"),
                        "Alinhamento",
                        "Alinhamento e balanceamento de rodas",
                        200.00m,
                    },
                    {
                        new Guid("55555555-5555-5555-5555-555555555555"),
                        "Revisão Completa",
                        "Revisão completa do veículo",
                        500.00m,
                    },
                    {
                        new Guid("66666666-6666-6666-6666-666666666666"),
                        "Troca de Pneus",
                        "Troca de pneus do veículo",
                        300.00m,
                    },
                    {
                        new Guid("77777777-7777-7777-7777-777777777777"),
                        "Reparo de Freios",
                        "Reparo e manutenção do sistema de freios",
                        400.00m,
                    },
                }
            );

            // Insert test Supplies
            migrationBuilder.InsertData(
                table: "Supplies",
                columns: new[] { "Id", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888888"), "Óleo 5W30", 45.00m, 50 },
                    {
                        new Guid("99999999-9999-9999-9999-999999999999"),
                        "Filtro de Óleo",
                        25.00m,
                        100,
                    },
                    {
                        new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                        "Pastilha de Freio",
                        80.00m,
                        30,
                    },
                    {
                        new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                        "Pneu Aro 17",
                        350.00m,
                        20,
                    },
                    {
                        new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                        "Bateria 60Ah",
                        400.00m,
                        15,
                    },
                    {
                        new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                        "Vela de Ignição",
                        15.00m,
                        200,
                    },
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete Services
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333")
            );

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444")
            );

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555")
            );

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666")
            );

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777")
            );

            // Delete Supplies
            migrationBuilder.DeleteData(
                table: "Supplies",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888")
            );

            migrationBuilder.DeleteData(
                table: "Supplies",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999")
            );

            migrationBuilder.DeleteData(
                table: "Supplies",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
            );

            migrationBuilder.DeleteData(
                table: "Supplies",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            );

            migrationBuilder.DeleteData(
                table: "Supplies",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc")
            );

            migrationBuilder.DeleteData(
                table: "Supplies",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd")
            );
        }
    }
}
