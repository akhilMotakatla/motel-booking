export interface User {
  id: number;
  fullName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  profileImageUrl?: string;
  role: string;
  isEmailVerified: boolean;
  createdAt: string;
  totalBookings: number;
}

export interface UpdateUser {
  fullName: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  profileImageUrl?: string;
}

export interface DashboardStats {
  totalRooms: number;
  availableRooms: number;
  occupiedRooms: number;
  totalBookings: number;
  pendingBookings: number;
  confirmedBookings: number;
  totalUsers: number;
  totalRevenue: number;
  monthlyRevenue: number;
  weeklyRevenue: number;
  occupancyRate: number;
  averageRating: number;
  revenueChart: RevenueChartData[];
  bookingStatusChart: BookingStatusData[];
}

export interface RevenueChartData {
  label: string;
  amount: number;
}

export interface BookingStatusData {
  status: string;
  count: number;
}
