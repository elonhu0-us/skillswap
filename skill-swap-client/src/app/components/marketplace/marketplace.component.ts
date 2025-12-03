import { Component, OnInit } from '@angular/core';
import { MarketplaceService } from '../../services/marketplace.service';

@Component({
  selector: 'app-marketplace',
  templateUrl: './marketplace.component.html',
  styleUrls: ['./marketplace.component.scss']
})
export class MarketplaceComponent implements OnInit {
  skills: any[] = [];
  loading = false;
  searchQuery = '';
  userLat: number | null = null;
  userLng: number | null = null;

  constructor(private marketplaceService: MarketplaceService) {}

  ngOnInit(): void {
    this.getUserLocation();
  }

  getUserLocation() {
    this.loading = true;

    if (!navigator.geolocation) {
      alert("Geolocation not supported.");
      this.loading = false;
      return;
    }

    navigator.geolocation.getCurrentPosition(
      (pos) => {
        this.userLat = pos.coords.latitude;
        this.userLng = pos.coords.longitude;

        // Fetch marketplace skills
        this.loadSkills();
      },
      (err) => {
        console.error(err);
        alert("Unable to get your location.");
        this.loading = false;
      }
    );
  }

  loadSkills() {
    if (this.userLat == null || this.userLng == null) return;

    this.marketplaceService
      .getSkills(this.userLat, this.userLng)
      .subscribe({
        next: (res) => {
          this.skills = res;
          this.loading = false;
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      });
  }

  searchSkills() {
    if (!this.searchQuery.trim()) {
      this.loadSkills();
      return;
    }

    this.loading = true;

    this.marketplaceService.searchSkills(this.searchQuery).subscribe({
      next: (res) => {
        this.skills = res;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }
}
