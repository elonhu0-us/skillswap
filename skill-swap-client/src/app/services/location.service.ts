import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private apiUrl = '/api/location';

  constructor(private http: HttpClient) {}

  updateLocation(lat: number, lng: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/update`, { lat, lng });
  }

  getBrowserLocation(): Promise<{ lat: number, lng: number }> {
    return new Promise((resolve, reject) => {
      navigator.geolocation.getCurrentPosition(
        pos => resolve({ lat: pos.coords.latitude, lng: pos.coords.longitude }),
        err => reject(err)
      );
    });
  }
}
