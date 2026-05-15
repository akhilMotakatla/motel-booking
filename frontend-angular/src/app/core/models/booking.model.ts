export interface Booking {
  id: number;
  userId: number;
  userName: string;
  userEmail: string;
  roomId: number;
  roomName: string;
  roomNumber: string;
  roomThumbnail?: string;
  checkInDate: string;
  checkOutDate: string;
  nightsCount: number;
  numberOfGuests: number;
  totalAmount: number;
  taxAmount: number;
  discountAmount: number;
  status: BookingStatus;
  specialRequests?: string;
  confirmationNumber?: string;
  createdAt: string;
  paymentStatus?: string;
}

export interface CreateBooking {
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
  specialRequests?: string;
  locationId?: number;
}

export type BookingStatus = 'Pending' | 'Confirmed' | 'CheckedIn' | 'CheckedOut' | 'Cancelled' | 'NoShow';
