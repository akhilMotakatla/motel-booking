import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StateInfo, CityInfo, Branch } from '../models/location.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LocationService {
  private readonly apiUrl = `${environment.apiUrl}/locations`;

  constructor(private http: HttpClient) {}

  getStates(): Observable<StateInfo[]> {
    return this.http.get<StateInfo[]>(`${this.apiUrl}/states`);
  }

  getCities(state: string): Observable<CityInfo[]> {
    const params = new HttpParams().set('state', state);
    return this.http.get<CityInfo[]>(`${this.apiUrl}/cities`, { params });
  }

  getBranches(state: string, city: string): Observable<Branch[]> {
    const params = new HttpParams().set('state', state).set('city', city);
    return this.http.get<Branch[]>(`${this.apiUrl}/branches`, { params });
  }

  getBranch(id: number): Observable<Branch> {
    return this.http.get<Branch>(`${this.apiUrl}/${id}`);
  }
}
