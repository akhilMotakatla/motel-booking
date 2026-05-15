import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Room, RoomFilter, PagedResult, CreateRoom } from '../models/room.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RoomService {
  private readonly apiUrl = `${environment.apiUrl}/rooms`;

  constructor(private http: HttpClient) {}

  getRooms(filter: Partial<RoomFilter> = {}): Observable<PagedResult<Room>> {
    let params = new HttpParams();
    Object.entries(filter).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '')
        params = params.set(key, String(value));
    });
    return this.http.get<PagedResult<Room>>(this.apiUrl, { params });
  }

  getFeaturedRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.apiUrl}/featured`);
  }

  getRoom(id: number): Observable<Room> {
    return this.http.get<Room>(`${this.apiUrl}/${id}`);
  }

  checkAvailability(id: number, checkIn: string, checkOut: string): Observable<{ isAvailable: boolean }> {
    const params = new HttpParams()
      .set('checkIn', checkIn)
      .set('checkOut', checkOut);
    return this.http.get<{ isAvailable: boolean }>(`${this.apiUrl}/${id}/availability`, { params });
  }

  getAvailableRooms(checkIn: string, checkOut: string, guests = 1): Observable<Room[]> {
    const params = new HttpParams()
      .set('checkIn', checkIn)
      .set('checkOut', checkOut)
      .set('guests', guests);
    return this.http.get<Room[]>(`${this.apiUrl}/available`, { params });
  }

  createRoom(dto: CreateRoom): Observable<Room> {
    return this.http.post<Room>(this.apiUrl, dto);
  }

  updateRoom(id: number, dto: Partial<CreateRoom>): Observable<Room> {
    return this.http.put<Room>(`${this.apiUrl}/${id}`, dto);
  }

  deleteRoom(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
