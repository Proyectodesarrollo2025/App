import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ToasterService } from '@abp/ng.theme.shared';

import { DestinationService, FavoriteDestinationService } from '@proxy/destinations';
import { CityDto, CitySearchRequestDto, DestinationDto } from '@proxy/application/contracts/destinations/models';

@Component({
  selector: 'app-destinations-list',
  standalone: false,
  templateUrl: './destinations-list.component.html',
  styleUrls: ['./destinations-list.component.scss']
})
export class DestinationsListComponent implements OnInit {

  cities: any[] = [];
  savedDestinations: DestinationDto[] = [];
  isLoading = false;
  showFilters = false;
  selectedCity: any = null;
  showModal = false;
  private cityImageCache = new Map<string, any>();

  searchParams = {
    query: '',
    country: '',
    region: '',
    minPopulation: null as number | null
  };

  private searchSubject = new Subject<string>();

  constructor(
    private destinationService: DestinationService,
    private favoriteService: FavoriteDestinationService,
    private toasterService: ToasterService
  ) { }

  ngOnInit(): void {
    this.loadSavedDestinations();

    this.searchSubject.pipe(
      debounceTime(1200),
      distinctUntilChanged()
    ).subscribe((searchTerm) => {
      this.executeSearch(searchTerm);
    });
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchParams.query);
  }

  onSearch(): void {
    this.executeSearch(this.searchParams.query);
  }

  private executeSearch(query: string): void {
    if (this.isLoading) return;
    if (!query && !this.searchParams.country && !this.searchParams.region) return;

    this.isLoading = true;

    const request: CitySearchRequestDto = {
      partialName: query,
      limit: 10,
      countryCode: this.searchParams.country || undefined,
      regionCode: this.searchParams.region || undefined,
      minPopulation: this.searchParams.minPopulation || undefined
    };

    this.destinationService.searchCities(request)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (result: any) => {
          this.cities = result.cities || result.items || [];
          this.fetchUnsplashImages();
        },
        error: (err) => {
          console.error('Error:', err);
          this.toasterService.error('Error al buscar ciudades', 'Error');
          this.cities = [];
        }
      });
  }

  private fetchUnsplashImages() {
    // Lista curada de IDs de fotos premium de Unsplash (Viajes, Arquitectura, Ciudades)
    // Estos IDs son 100% estables y de alta resolución.
    const curatedPhotoIds = [
      '1502602898657-3e91760cbb34', // Paris style
      '1449156001566-35957096fb91', // Tokyo style
      '1477959858617-67f85cf4f1df', // NYC style
      '1513635269975-59663e0ac1ad', // London style
      '1501594907352-0dfc58eb36fe', // San Francisco style
      '1552832230-019623e618aa', // Rome style
      '1523482580672-f109ba8cb9be', // Sydney style
      '1520117147647-0349b22ef104', // Berlin style
      '1493333858332-df7ed0ec419e', // Barcelona style
      '1512100353987-0b19280d4607', // Santorini style
      '1533105079780-92b9be482077', // Venice style
      '1464822759023-fed622ff2c3b', // Mountains/Nature
      '1503389158882-9366144bea98', // Modern City
      '1480714378408-67cf0d13bc1b', // Chicago style
      '1518684079-3c830dcef090', // Dubai style
      '1529156069912-ab0023a73e1c', // European street
      '1507525428034-b723cf961d3e', // Tropical Beach
      '1519501025264-65ba15a82390', // Cityscape
      '1534447677768-be436bb09401', // Hill city
      '1517154421773-0529f29ea451'  // Seoul style
    ];

    for (let i = 0; i < this.cities.length; i++) {
      const city = this.cities[i];
      // Usamos el nombre de la ciudad para generar un índice consistente
      const charSum = city.name.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0);
      const photoId = curatedPhotoIds[charSum % curatedPhotoIds.length];

      city.wikiImage = `https://images.unsplash.com/photo-${photoId}?auto=format&fit=crop&w=800&q=80`;
    }
  }

  async openCityDetails(city: any) {
    this.selectedCity = city;
    this.showModal = true;

    if (city.description) return;

    try {
      // Usamos términos que obliguen a Wikipedia a buscar el lugar geográfico
      const searchTerms = [
        `${city.name}, ${city.country} (localidad)`,
        `${city.name}, ${city.country} (municipio)`,
        `${city.name} ${city.country}`
      ];

      const BANNED = /enfermedad|político|partido|médico|virus|biografía|nacido en|elecciones|mosquito|protesta/i;
      const REQUIRED = /ciudad|municipio|localidad|población|situado|clima|turismo|historia/i;

      let foundExtract = '';

      for (const term of searchTerms) {
        const url = `https://es.wikipedia.org/w/api.php?action=query&format=json&generator=search&gsrsearch=${encodeURIComponent(term)}&gsrlimit=3&prop=extracts&exintro&explaintext&exchars=600&origin=*`;
        const response = await fetch(url);
        const data = await response.json();

        if (data.query?.pages) {
          for (const id in data.query.pages) {
            const extract = data.query.pages[id].extract || '';
            // Si el texto es de un lugar (contiene REQUIRED) y no es basura (NO contiene BANNED)
            if (REQUIRED.test(extract) && !BANNED.test(extract)) {
              foundExtract = extract;
              break;
            }
          }
        }
        if (foundExtract) break;
      }

      city.description = foundExtract || `Descubre ${city.name}, un destino fascinante en ${city.country} conocido por su vibrante cultura y hospitalidad. Explora sus calles y sumérgete en una experiencia única.`;

    } catch (e) {
      city.description = "La información sobre este destino se está actualizando. Es un lugar con mucho por descubrir.";
    }
  }

  closeDetails() {
    this.showModal = false;
  }

  loadSavedDestinations(): void {
    this.favoriteService.getMyFavorites()
      .subscribe(result => {
        this.savedDestinations = result;
      });
  }

  isFavorited(city: CityDto | any): boolean {
    const cityName = city.name || city.city;
    return this.savedDestinations.some(d => d.city === cityName && d.country === city.country);
  }

  getFavoriteId(city: CityDto | any): string | undefined {
    const cityName = city.name || city.city;
    return this.savedDestinations.find(d => d.city === cityName && d.country === city.country)?.id;
  }

  toggleFavorite(city: CityDto | any): void {
    const existingId = this.getFavoriteId(city);

    if (existingId) {
      // Already in DB, just toggle
      this.favoriteService.toggleFavorite(existingId).subscribe({
        next: () => {
          this.loadSavedDestinations();
        },
        error: (err) => {
          console.error('Error al cambiar favorito:', err);
          this.toasterService.error('No se pudo procesar la solicitud', 'Error');
        }
      });
    } else {
      // Not in DB yet, create it first
      const input = {
        name: city.name || city.city,
        country: city.country,
        city: city.name || city.city,
        latitude: city.latitude,
        longitude: city.longitude,
        photoUrl: city.wikiImage || ''
      };

      this.destinationService.create(input).subscribe({
        next: (newDest) => {
          // Now toggle favorite
          this.favoriteService.toggleFavorite(newDest.id).subscribe({
            next: () => {
              this.loadSavedDestinations();
            }
          });
        },
        error: (err) => {
          console.error('Error al guardar destino:', err);
          this.toasterService.error('No se pudo guardar el destino', 'Error');
        }
      });
    }
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  clearSearch(): void {
    this.searchParams.query = '';
    this.searchParams.country = '';
    this.searchParams.region = '';
    this.searchParams.minPopulation = null;
    this.cities = [];
  }

  getCityImageUrl(cityOrName: any, country?: string): string {
    const name = typeof cityOrName === 'string' ? cityOrName : (cityOrName?.name || cityOrName?.city);
    const countryName = country || cityOrName?.country || '';

    if (!name) {
      return 'https://images.unsplash.com/photo-1519501025264-65ba15a82390?auto=format&fit=crop&w=800&q=80';
    }

    // Limpiamos los términos para el buscador
    const cleanCity = name.toLowerCase().replace(/\s+/g, '-');
    const cleanCountry = countryName.toLowerCase().replace(/\s+/g, '-');

    // Usamos el nombre como 'lock' para que sea consistente pero único por ciudad
    const seed = name.length + (countryName.length * 2);

    // Etiquetas: Ciudad + País + Paisaje + Monumento
    const tags = `${cleanCity},${cleanCountry},landmark,city`;

    return `https://loremflickr.com/800/600/${tags}/all?lock=${seed}`;
  }

  formatCoordinates(lat?: string, long?: string): string {
    if (!lat || !long) return 'N/A';
    const latNum = parseFloat(lat);
    const longNum = parseFloat(long);
    if (isNaN(latNum) || isNaN(longNum)) return `${lat}, ${long}`;
    return `${latNum.toFixed(4)}, ${longNum.toFixed(4)}`;
  }

  openInMaps(lat?: string, long?: string): void {
    if (lat && long) {
      const url = `https://www.google.com/maps/search/?api=1&query=${lat},${long}`;
      window.open(url, '_blank');
    }
  }
}
