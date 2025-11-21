import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { DestinationService } from '@proxy/destinations'; 
import { CityDto, CitySearchRequestDto } from '@proxy/application/contracts/destinations/models'; 

@Component({
  selector: 'app-destinations-list',
  standalone: false,
  templateUrl: './destinations-list.component.html',
  styleUrls: ['./destinations-list.component.scss']
})
export class DestinationsListComponent implements OnInit {
   
  cities: CityDto[] = [];
  isLoading = false;

  searchParams = {
    query: '',
    country: ''
  };

  constructor(private destinationService: DestinationService) {}

  ngOnInit(): void {}

  onSearch(): void {
    if (!this.searchParams.query && !this.searchParams.country) return;

    this.isLoading = true;
    this.cities = [];

    const request: CitySearchRequestDto = {
      partialName: this.searchParams.query,
      limit: 10,
      countryCode: this.searchParams.country || undefined
    };

    this.destinationService.searchCities(request)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (result: any) => {
          
          this.cities = result.cities || result.items || [];
        },
        error: (err) => {
          console.error('Error:', err);
          this.cities = [];
        }
      });
  }

  clearSearch(): void {
    this.searchParams.query = '';
    this.searchParams.country = '';
    this.cities = [];
  }

  formatCoordinates(lat?: string, long?: string): string {
    if (!lat || !long) return 'N/A';
    const latNum = parseFloat(lat);
    const longNum = parseFloat(long);
    if (isNaN(latNum) || isNaN(longNum)) return `${lat}, ${long}`;
    return `${latNum.toFixed(4)}, ${longNum.toFixed(4)}`;
  }

  openInMaps(city: CityDto): void {
    if (city.latitude && city.longitude) {
      const url = `https://www.google.com/maps/search/?api=1&query=${city.latitude},${city.longitude}`;
      window.open(url, '_blank');
    }
  }
}