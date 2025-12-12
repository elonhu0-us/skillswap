import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MarketplaceService, SkillPost } from '../../services/marketplace.service';
import { LocationService } from '../../services/location.service';

@Component({
  selector: 'app-marketplace',
  templateUrl: './marketplace.component.html',
  styleUrls: ['./marketplace.component.scss'],
  standalone: true,
  imports: [CommonModule]
})

export class MarketplaceComponent implements OnInit {

  skills: SkillPost[] = [];
  loading = true;
  error = '';

  constructor(
    private marketplaceService: MarketplaceService,
    private locationService: LocationService
  ) {}

  async ngOnInit() {
    try {
      const loc = await this.locationService.getBrowserLocation();

      // update backend user cache
      await this.locationService.updateLocation(loc.lat, loc.lng).toPromise();

      // fetch radius-filtered skills
      this.marketplaceService.getNearbySkills(loc.lat, loc.lng, 50)
        .subscribe({
          next: data => {
            this.skills = data;
            this.loading = false;
          },
          error: err => {
            this.error = 'Failed to load skills';
            this.loading = false;
          }
        });

    } catch (err) {
      this.error = 'Could not access location.';
      this.loading = false;
    }
  }
}
