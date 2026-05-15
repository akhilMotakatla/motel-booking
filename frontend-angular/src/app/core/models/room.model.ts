export interface Room {
  id: number;
  name: string;
  description: string;
  pricePerNight: number;
  maxOccupancy: number;
  floorNumber: number;
  roomNumber: string;
  sizeInSqFt: number;
  status: RoomStatus;
  thumbnailUrl?: string;
  averageRating: number;
  totalReviews: number;
  isFeatured: boolean;
  roomTypeName: string;
  roomTypeId: number;
  imageUrls: string[];
  amenities: Amenity[];
}

export interface Amenity {
  id: number;
  name: string;
  icon?: string;
  category?: string;
}

export interface RoomFilter {
  search?: string;
  roomTypeId?: number;
  locationId?: number;
  state?: string;
  city?: string;
  minPrice?: number;
  maxPrice?: number;
  maxOccupancy?: number;
  checkInDate?: string;
  checkOutDate?: string;
  status?: string;
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export type RoomStatus = 'Available' | 'Occupied' | 'Maintenance' | 'Reserved' | 'OutOfService';

export interface CreateRoom {
  name: string;
  description: string;
  pricePerNight: number;
  maxOccupancy: number;
  floorNumber: number;
  roomNumber: string;
  sizeInSqFt: number;
  roomTypeId: number;
  isFeatured: boolean;
  amenityIds: number[];
  imageUrls: string[];
  thumbnailUrl?: string;
}
