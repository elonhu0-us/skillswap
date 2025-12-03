import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MarketplaceService {
  private baseUrl = `${environment.apiUrl}/skills`;

  constructor(private http: HttpClient) {}

  getSkills(lat: number, lng: number) {
    return this.http.get<any[]>(
      `${this.baseUrl}?userLat=${lat}&userLng=${lng}&radiusMiles=50`
    );
  }

  searchSkills(query: string) {
    return this.http.get<any[]>(
      `${this.baseUrl}/search?query=${query}`
    );
  }
}
