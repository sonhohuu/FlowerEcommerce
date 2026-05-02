export const MOCK_PRODUCTS = [
  { id: 1, name: "Áo thun Unisex Basic", category: "Áo", price: 250000, stock: 142, status: "active", image: "👕" },
  { id: 2, name: "Quần Jogger Slim", category: "Quần", price: 420000, stock: 87, status: "active", image: "👖" },
  { id: 3, name: "Giày Sneaker Air", category: "Giày", price: 1200000, stock: 23, status: "active", image: "👟" },
  { id: 4, name: "Túi Tote Canvas", category: "Phụ kiện", price: 180000, stock: 0, status: "out_of_stock", image: "👜" },
  { id: 5, name: "Mũ Bucket Retro", category: "Phụ kiện", price: 150000, stock: 65, status: "active", image: "🧢" },
  { id: 6, name: "Áo khoác Denim", category: "Áo", price: 750000, stock: 12, status: "low_stock", image: "🧥" },
  { id: 7, name: "Váy Floral Midi", category: "Váy", price: 380000, stock: 44, status: "active", image: "👗" },
  { id: 8, name: "Kính Mắt Vintage", category: "Phụ kiện", price: 320000, stock: 0, status: "out_of_stock", image: "🕶️" },
];
 
export const MOCK_CATEGORIES = [
  { id: 1, name: "Áo", slug: "ao", productCount: 24, icon: "👕", color: "#6366f1" },
  { id: 2, name: "Quần", slug: "quan", productCount: 18, icon: "👖", color: "#8b5cf6" },
  { id: 3, name: "Giày", slug: "giay", productCount: 12, icon: "👟", color: "#ec4899" },
  { id: 4, name: "Phụ kiện", slug: "phu-kien", productCount: 35, icon: "👜", color: "#f59e0b" },
  { id: 5, name: "Váy", slug: "vay", productCount: 9, icon: "👗", color: "#10b981" },
  { id: 6, name: "Đồ thể thao", slug: "the-thao", productCount: 16, icon: "🏃", color: "#3b82f6" },
];
 
export const MOCK_ORDERS = [
  { id: "#ORD-2847", customer: "Nguyễn Minh Tuấn", total: 1450000, items: 3, status: "delivered", date: "02/05/2026", payment: "VNPay" },
  { id: "#ORD-2846", customer: "Trần Thị Lan", total: 250000, items: 1, status: "processing", date: "02/05/2026", payment: "COD" },
  { id: "#ORD-2845", customer: "Lê Hoàng Nam", total: 2100000, items: 4, status: "shipped", date: "01/05/2026", payment: "MoMo" },
  { id: "#ORD-2844", customer: "Phạm Thu Hà", total: 380000, items: 2, status: "pending", date: "01/05/2026", payment: "COD" },
  { id: "#ORD-2843", customer: "Hoàng Văn Bình", total: 900000, items: 2, status: "cancelled", date: "30/04/2026", payment: "VNPay" },
  { id: "#ORD-2842", customer: "Đinh Thị Mai", total: 570000, items: 3, status: "delivered", date: "30/04/2026", payment: "MoMo" },
  { id: "#ORD-2841", customer: "Vũ Đức Anh", total: 1800000, items: 5, status: "processing", date: "29/04/2026", payment: "COD" },
];
 
export const MOCK_USERS = [
  { id: 1, name: "Nguyễn Minh Tuấn", email: "tuan.nguyen@email.com", role: "customer", orders: 12, spent: 8500000, joined: "01/2025", status: "active", avatar: "N" },
  { id: 2, name: "Trần Thị Lan", email: "lan.tran@email.com", role: "customer", orders: 5, spent: 2100000, joined: "03/2025", status: "active", avatar: "T" },
  { id: 3, name: "Admin Chính", email: "admin@store.com", role: "admin", orders: 0, spent: 0, joined: "01/2024", status: "active", avatar: "A" },
  { id: 4, name: "Lê Hoàng Nam", email: "nam.le@email.com", role: "customer", orders: 8, spent: 5400000, joined: "06/2025", status: "active", avatar: "L" },
  { id: 5, name: "Phạm Thu Hà", email: "ha.pham@email.com", role: "customer", orders: 3, spent: 980000, joined: "09/2025", status: "inactive", avatar: "P" },
  { id: 6, name: "Mod Hỗ Trợ", email: "mod@store.com", role: "moderator", orders: 0, spent: 0, joined: "04/2024", status: "active", avatar: "M" },
  { id: 7, name: "Vũ Đức Anh", email: "anh.vu@email.com", role: "customer", orders: 19, spent: 15200000, joined: "11/2024", status: "active", avatar: "V" },
];