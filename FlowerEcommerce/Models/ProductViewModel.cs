namespace FlowerEcommerce.View.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? DiscountPercent { get; set; }
        public bool IsContactPrice { get; set; }
        public string Category { get; set; } = "";
        public string CategorySlug { get; set; } = "";
    }

    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Sku { get; set; } = "";
        public List<string> Images { get; set; } = new();
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public bool IsContactPrice { get; set; }
        public bool IsOutOfStock { get; set; }
        public string Category { get; set; } = "";
        public string CategorySlug { get; set; } = "";
        public string Description { get; set; } = "";
        public List<SizePrice> SizePrices { get; set; } = new();
    }

    public class SizePrice
    {
        public string Label { get; set; } = "";
        public decimal Price { get; set; }
    }

    public static class FishingProductsData
    {
        public static readonly List<ProductDetailViewModel> All = new()
        {
            // ── CẦN CÂU TAY ──────────────────────────────────────────────────
            new()
            {
                Id = 1,
                Name = "Cần tay King Kong Transformers 5H-540",
                Sku = "KK-TF-540",
                Images = new() { "/images/can-tay/kingkong-tf540-1.jpg", "/images/can-tay/kingkong-tf540-2.jpg", "/images/can-tay/kingkong-tf540-3.jpg", "/images/can-tay/kingkong-tf540-4.jpg" },
                Price = 1_950_000,
                OriginalPrice = 2_200_000,
                IsContactPrice = false,
                IsOutOfStock = false,
                Category = "Cần Câu Tay",
                CategorySlug = "can-cau-tay",
                Description = @"<b>Cần tay chuyên săn hàng King Kong Transformers 5H-540</b><br>
Mã sản phẩm: <b>King Kong</b><br>
Chất liệu: Carbon<br>
Chiều dài: 5M4<br>
Đường kính đọt: 1.8mm<br>
Đường kính gốc: 20.5mm<br>
Độ cứng: 5H<br>
Số lóng: 5<br>
Trọng lượng cần: 289g<br>
Phân bổ lực: 28<br>
Tải tĩnh an toàn: 3500g<br>
Lực tải động: 10–50Kg<br>
<b>Thương hiệu</b>: Transformers<br>
<b>Sản xuất</b>: Trung Quốc",
                SizePrices = new()
                {
                    new() { Label = "4M5 - 5H", Price = 1_850_000 },
                    new() { Label = "5M4 - 5H", Price = 1_950_000 },
                    new() { Label = "6M3 - 5H", Price = 2_950_000 },
                    new() { Label = "7M2 - 5H", Price = 3_050_000 },
                }
            },
            new()
            {
                Id = 2,
                Name = "Cần tay Daiwa Ninja 4H-600",
                Sku = "DW-NJ-600",
                Images = new() { "/images/can-tay/daiwa-ninja-600-1.jpg", "/images/can-tay/daiwa-ninja-600-2.jpg" },
                Price = 2_450_000,
                OriginalPrice = 2_800_000,
                IsContactPrice = false,
                IsOutOfStock = false,
                Category = "Cần Câu Tay",
                CategorySlug = "can-cau-tay",
                Description = @"<b>Cần tay Daiwa Ninja 4H-600</b><br>
Chất liệu: Carbon HVF<br>
Chiều dài: 6M0<br>
Độ cứng: 4H<br>
Số lóng: 6<br>
Trọng lượng: 265g<br>
Tải động: 8–40Kg<br>
<b>Thương hiệu</b>: Daiwa<br>
<b>Sản xuất</b>: Nhật Bản",
                SizePrices = new()
                {
                    new() { Label = "4M5 - 4H", Price = 1_950_000 },
                    new() { Label = "5M4 - 4H", Price = 2_150_000 },
                    new() { Label = "6M0 - 4H", Price = 2_450_000 },
                }
            },
            new()
            {
                Id = 3,
                Name = "Cần tay Shimano FX XT 3H-450",
                Sku = "SHM-FXXT-450",
                Images = new() { "/images/can-tay/shimano-fx-450-1.jpg" },
                Price = 0,
                IsContactPrice = true,
                IsOutOfStock = false,
                Category = "Cần Câu Tay",
                CategorySlug = "can-cau-tay",
                Description = @"<b>Cần tay Shimano FX XT 3H-450</b><br>
Chất liệu: XT Carbon<br>
Chiều dài: 4M5<br>
Độ cứng: 3H<br>
Số lóng: 5<br>
Trọng lượng: 240g<br>
<b>Thương hiệu</b>: Shimano<br>
<b>Sản xuất</b>: Nhật Bản",
                SizePrices = new()
            },
            new()
            {
                Id = 4,
                Name = "Cần tay Carbon Ultra Light 2H-360",
                Sku = "CUL-2H-360",
                Images = new() { "/images/can-tay/ultra-light-360-1.jpg", "/images/can-tay/ultra-light-360-2.jpg" },
                Price = 850_000,
                OriginalPrice = 1_050_000,
                IsContactPrice = false,
                IsOutOfStock = false,
                Category = "Cần Câu Tay",
                CategorySlug = "can-cau-tay",
                Description = @"<b>Cần tay Carbon Ultra Light 2H-360</b><br>
Chất liệu: Carbon T800<br>
Chiều dài: 3M6<br>
Độ cứng: 2H<br>
Số lóng: 4<br>
Trọng lượng: 168g<br>
Tải động: 3–15Kg<br>
<b>Sản xuất</b>: Trung Quốc",
                SizePrices = new()
                {
                    new() { Label = "2M7 - 2H", Price = 650_000 },
                    new() { Label = "3M6 - 2H", Price = 850_000 },
                    new() { Label = "4M5 - 2H", Price = 1_050_000 },
                }
            },
 
            // ── CẦN CÂU MÁY ─────────────────────────────────────────────────
            new()
            {
                Id = 5,
                Name = "Máy câu ngang Shimano Sedona 2500",
                Sku = "SHM-SDN-2500",
                Images = new() { "/images/can-may/shimano-sedona-2500-1.jpg", "/images/can-may/shimano-sedona-2500-2.jpg" },
                Price = 1_650_000,
                OriginalPrice = 1_950_000,
                IsContactPrice = false,
                IsOutOfStock = false,
                Category = "Cần Câu Máy",
                CategorySlug = "can-cau-may",
                Description = @"<b>Máy câu ngang Shimano Sedona 2500</b><br>
Số bi: 5+1<br>
Tỉ số truyền: 5.0:1<br>
Dung lượng dây: 0.25mm/140m<br>
Trọng lượng: 240g<br>
Lực kéo tối đa: 8Kg<br>
<b>Thương hiệu</b>: Shimano<br>
<b>Sản xuất</b>: Nhật Bản",
                SizePrices = new()
            },
            new()
            {
                Id = 6,
                Name = "Máy câu ngang Daiwa Freams LT 3000",
                Sku = "DW-FRMS-3000",
                Images = new() { "/images/can-may/daiwa-freams-3000-1.jpg" },
                Price = 2_150_000,
                IsContactPrice = false,
                IsOutOfStock = true,
                OriginalPrice = 2_450_000,
                Category = "Cần Câu Máy",
                CategorySlug = "can-cau-may",
                Description = @"<b>Máy câu ngang Daiwa Freams LT 3000</b><br>
Số bi: 6+1<br>
Tỉ số truyền: 5.2:1<br>
Dung lượng dây: 0.3mm/150m<br>
Trọng lượng: 220g<br>
Lực kéo tối đa: 10Kg<br>
Công nghệ: Zaion Air Rotor<br>
<b>Thương hiệu</b>: Daiwa<br>
<b>Sản xuất</b>: Nhật Bản",
                SizePrices = new()
            },
            new()
            {
                Id = 7,
                Name = "Máy câu đứng Abu Garcia Revo SX",
                Sku = "ABG-RVSX-6",
                Images = new() { "/images/can-may/abugarcia-revo-sx-1.jpg", "/images/can-may/abugarcia-revo-sx-2.jpg" },
                Price = 3_200_000,
                IsContactPrice = false,
                IsOutOfStock = false,
                Category = "Cần Câu Máy",
                CategorySlug = "can-cau-may",
                Description = @"<b>Máy câu đứng (Baitcaster) Abu Garcia Revo SX</b><br>
Số bi: 9+1<br>
Tỉ số truyền: 6.6:1<br>
Dung lượng dây: 0.30mm/120m<br>
Trọng lượng: 195g<br>
Lực phanh: Infini II Magnetic Brake<br>
<b>Thương hiệu</b>: Abu Garcia<br>
<b>Sản xuất</b>: Thụy Điển",
                SizePrices = new()
            },
            new()
            {
                Id = 8,
                Name = "Combo cần + máy câu Okuma Celilo",
                Sku = "OKM-CEL-COMBO",
                Images = new() { "/images/can-may/okuma-celilo-combo-1.jpg" },
                Price = 0,
                IsContactPrice = true,
                IsOutOfStock = false,
                Category = "Cần Câu Máy",
                CategorySlug = "can-cau-may",
                Description = @"<b>Combo cần + máy câu Okuma Celilo</b><br>
Bộ combo trọn gói tiện lợi cho người mới bắt đầu.<br>
Cần: Carbon tổng hợp, 1M98<br>
Máy: Okuma Celilo 30, 5+1 bi, tỉ số 5.0:1<br>
<b>Thương hiệu</b>: Okuma<br>
<b>Sản xuất</b>: Đài Loan",
                SizePrices = new()
            },
 
            // ── CẦN CÂU LURE ─────────────────────────────────────────────────
            new()
            {
                Id = 9,
                Name = "Cần lure Majorcraft Crostage 2-8g",
                Sku = "MC-CRS-2S",
                Images = new() { "/images/can-lure/majorcraft-crostage-1.jpg", "/images/can-lure/majorcraft-crostage-2.jpg" },
                Price = 1_850_000,
                OriginalPrice = 2_100_000,
                IsContactPrice = false,
                IsOutOfStock = false,
                Category = "Cần Câu Lure",
                CategorySlug = "can-cau-lure",
                Description = @"<b>Cần lure Majorcraft Crostage 2-8g</b><br>
Chất liệu: Carbon 40T<br>
Chiều dài: 1M98 (2 khúc)<br>
Lure weight: 2–8g<br>
Line: PE #0.3–0.8<br>
Action: Fast<br>
Trọng lượng cần: 86g<br>
<b>Thương hiệu</b>: MajorCraft<br>
<b>Sản xuất</b>: Nhật Bản",
                SizePrices = new()
                {
                    new() { Label = "1M98 - 2-8g", Price = 1_850_000 },
                    new() { Label = "2M10 - 3-10g", Price = 2_050_000 },
                }
            },
            new()
            {
                Id = 10,
                Name = "Cần lure Yamaga Blanks Blue Current 72",
                Sku = "YMG-BC-72",
                Images = new() { "/images/can-lure/yamaga-bc72-1.jpg" },
                Price = 4_500_000,
                IsContactPrice = false,
                IsOutOfStock = false,
                Category = "Cần Câu Lure",
                CategorySlug = "can-cau-lure",
                Description = @"<b>Cần lure Yamaga Blanks Blue Current 72</b><br>
Chất liệu: Carbon Nano Alloy<br>
Chiều dài: 2M18 (1 khúc)<br>
Lure weight: 1–10g<br>
Line: PE #0.2–0.6<br>
Action: Moderate Fast<br>
Trọng lượng cần: 74g<br>
<b>Thương hiệu</b>: Yamaga Blanks<br>
<b>Sản xuất</b>: Nhật Bản",
                SizePrices = new()
            },
            new()
            {
                Id = 11,
                Name = "Cần lure jigging Xzoga Taka-S 200g",
                Sku = "XZG-TAKA-200",
                Images = new() { "/images/can-lure/xzoga-taka-200-1.jpg", "/images/can-lure/xzoga-taka-200-2.jpg" },
                Price = 0,
                IsContactPrice = true,
                IsOutOfStock = false,
                Category = "Cần Câu Lure",
                CategorySlug = "can-cau-lure",
                Description = @"<b>Cần lure jigging Xzoga Taka-S 200g</b><br>
Chất liệu: Carbon 46T<br>
Chiều dài: 1M83<br>
Jig weight: up to 200g<br>
Line: PE #2–4<br>
Action: Regular Fast<br>
Ứng dụng: câu jigging ngoài khơi, cá lớn<br>
<b>Thương hiệu</b>: Xzoga<br>
<b>Sản xuất</b>: Nhật Bản",
                SizePrices = new()
                {
                    new() { Label = "Taka-S 150g", Price = 3_800_000 },
                    new() { Label = "Taka-S 200g", Price = 4_200_000 },
                    new() { Label = "Taka-S 300g", Price = 4_800_000 },
                }
            },
            new()
            {
                Id = 12,
                Name = "Cần lure bass Evergreen Poseidon 6'6\"",
                Sku = "EVG-PSD-66",
                Images = new() { "/images/can-lure/evergreen-poseidon-1.jpg" },
                Price = 3_750_000,
                OriginalPrice = 4_200_000,
                IsContactPrice = false,
                IsOutOfStock = false,
                Category = "Cần Câu Lure",
                CategorySlug = "can-cau-lure",
                Description = @"<b>Cần lure bass Evergreen Poseidon 6'6""</b><br>
Chất liệu: Nhật carbon blend<br>
Chiều dài: 1M98<br>
Lure weight: 7–21g<br>
Line: 10–20lb mono<br>
Action: Fast<br>
Ứng dụng: cá bass, snakehead (cá lóc)<br>
<b>Thương hiệu</b>: Evergreen<br>
<b>Sản xuất</b>: Nhật Bản",
                SizePrices = new()
            },
        };
    }
}
