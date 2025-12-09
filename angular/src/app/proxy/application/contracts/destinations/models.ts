import type { AuditedEntityDto } from '@abp/ng.core';

export interface CityDto {
  name?: string;
  country?: string;
  countryCode?: string;
  region?: string;
  latitude?: string;
  longitude?: string;
}

export interface CitySearchRequestDto {
  partialName?: string;
  limit: number;
  countryCode?: string;
}

export interface CitySearchResultDto {
  cities: CityDto[];
}

export interface CreateUpdateDestinationDto extends AuditedEntityDto<string> {
  name?: string;
  country?: string;
  city?: string;
  photoUrl?: string;
  latitude?: string;
  longitude?: string;
}

export interface DestinationDto extends AuditedEntityDto<string> {
  name?: string;
  country?: string;
  city?: string;
  photoUrl?: string;
  lastUpdated?: string;
  latitude?: string;
  longitude?: string;
}
