import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SkillPost {
  id: number;
  ownerId: number;
  title: string;
  description: string;
  type: string;
}

@Injectable({
  providedIn: 'root'
})
export class MarketplaceService {
  private apiUrl = '/api/skills';

  constructor(private http: HttpClient) {}

  getNearbySkills(lat: number, lng: number, radius: number = 50): Observable<SkillPost[]> {
    return this.http.get<SkillPost[]>(
      `${this.apiUrl}/nearby?lat=${lat}&lng=${lng}&radius=${radius}`
    );
  }

  searchSkills(query: string): Observable<SkillPost[]> {
    return this.http.get<SkillPost[]>(`${this.apiUrl}/search?query=${query}`);
  }
}
