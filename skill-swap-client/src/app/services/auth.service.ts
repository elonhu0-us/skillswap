import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

interface AuthResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
    private apiUrl = '/api/auth';;

    constructor(private http: HttpClient) { }

    register(data: {email:string; displayName:string; passwordHash:string}): Observable<any> {
        return this.http.post(`${this.apiUrl}/register`, data);
    }
    login(data: {email:string; passwordHash:string}): Observable<AuthResponse> {
        return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data);
    }
    setToken(token: string): void {
        localStorage.setItem('jwt', token);
    }
    getToken(): string | null {
        return localStorage.getItem('jwt');
    }
    logout(): void {
        localStorage.removeItem('jwt');
    }
    isLoggedIn(): boolean {
        return this.getToken() !== null;
    }
}