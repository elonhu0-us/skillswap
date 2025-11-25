import { Component } from "@angular/core";
import { AuthService } from "../../services/auth.service";
import { Router } from "@angular/router";

@Component({
    selector: "app-login",
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.scss"]})
export class LoginComponent {
    email: string = "";
    password: string = "";
    constructor(private authService: AuthService, private router: Router) {}
    submit(){
        this.authService.login({
            email: this.email,
            passwordHash: this.password}).subscribe({
            next: (res) => {
                this.authService.setToken(res.token);
                alert("Login successful");
                this.router.navigate(["/"]);//homepage
            },
            error: (err) => {
                console.error("Login failed", err);
                alert("Login failed: " + err.error.message);
            }
        });
    }
}