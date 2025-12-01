import { Component } from "@angular/core";
import { AuthService } from "../../services/auth.service";
import { FormsModule } from '@angular/forms';
@Component({
    selector: "app-register",
    templateUrl: "./register.component.html",
    styleUrls: ["./register.component.scss"],
    standalone: true,
    imports: [
        FormsModule
    ]})
export class RegisterComponent {
    email: string = "";
    displayName: string = "";
    password: string = ""
    constructor(private authService: AuthService) {}
    submit(){
        this.authService.register({
            email: this.email,
            displayName: this.displayName,
            passwordHash: this.password
        }).subscribe({
            next: (res) => {
                console.log("Registration successful", res);
            },
            error: (err) => {
                console.error("Registration failed", err);
            }
        });
    }
}