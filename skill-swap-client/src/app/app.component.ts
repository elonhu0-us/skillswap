import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LocationService } from './services/location.service';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet], 
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent { 
  constructor(private locationService: LocationService) {}

  ngOnInit() {
    navigator.geolocation.getCurrentPosition(pos => {
      this.locationService.updateLocation(
        pos.coords.latitude,
        pos.coords.longitude
      ).subscribe();
    });
  }
}


